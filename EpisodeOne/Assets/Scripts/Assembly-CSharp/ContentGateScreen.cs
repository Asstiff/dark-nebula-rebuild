using System;
using FLD;
using Palringo;
using UnityEngine;

public class ContentGateScreen : MonoBehaviour
{
	[SerializeField]
	private GameObject _cannotUnlockNowContainer;

	[SerializeField]
	private GameObject _rewardContainer;

	[SerializeField]
	private GameObject _waitContainer;

	[SerializeField]
	private MenuHeader _menuHeader;

	[SerializeField]
	private SimpleButton _purchaseIapButton;

	[SerializeField]
	private SimpleButton _joinPalringoButton;

	[SerializeField]
	private PalringoExperienceBar _palringoExperienceBar;

	[SerializeField]
	private SimpleButton _acceptRewardButton;

	[SerializeField]
	private UnlockAllEffect _acceptRewardEffect;

	[SerializeField]
	private Transform _acceptRewardBackground;

	[SerializeField]
	private GameObject _gatePreunlockLabel;

	[SerializeField]
	private DynamicFont _levelSpanLabel;

	[SerializeField]
	private DynamicFont _iapPriceTagLabel;

	private CameraFade _cameraFade;

	private void Start()
	{
		DarkNebulaAdsManager.Instance.ShowAd("GateLock").Wait((string obj) =>
		{
			StartProcedure();
		});
	}

	private void StartProcedure()
	{
		_acceptRewardButton.Text = "COLLECT UNLOCK REWARD BUTTON".Localize();
		_menuHeader.SetTitleID("GATE LOCK HEADER", true);
		_menuHeader.UpdateCurrentLevel();
		_cameraFade = base.gameObject.AddComponent<CameraFade>();
		_cameraFade.FadeIn();
		if (!ContentGatingManager.Instance.IsLevelGated(LoadLevelScreen.LevelToLoad))
		{
			Debug.LogError("This level is not gated; continuing immediately");
			ContinueToLevel();
			return;
		}
		if (ContentGatingManager.Instance.IsGateUnlocked(LoadLevelScreen.LevelToLoad))
		{
			Debug.LogError("This level is already unlocked; continuing immediately");
			ContinueToLevel();
			return;
		}
		_menuHeader.BackButton.ButtonUpEvent += HandleBackButton;
		_joinPalringoButton.ButtonUpEvent += HandlePalringoButton;
		_purchaseIapButton.ButtonUpEvent += HandlePurchaseIAPButton;
		_acceptRewardButton.ButtonUpEvent += HandleAcceptRewardButton;
		int pMinLevel;
		int pMaxLevel;
		ContentGatingManager.Instance.GetGateLevelSpan(LoadLevelScreen.LevelToLoad, out pMinLevel, out pMaxLevel);
		_levelSpanLabel.Text = "GATE LOCK EXPLANATION".Localize(pMinLevel, pMaxLevel);
		string keyPrice = ContentGatingManager.Instance.GetKeyPrice();
		_iapPriceTagLabel.Text = keyPrice.ToUpper();
		_palringoExperienceBar.TargetReputationLevel = 5;
		_palringoExperienceBar.ExplanationLabel.Text = "REACH_LEVEL_EXPLANATION".Localize(_palringoExperienceBar.TargetReputationLevel);
		UpdateLockStatus();
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			GotoPreviousScene();
		}
	}

	private void GotoPreviousScene()
	{
		_cameraFade.FadeComplete += () =>
		{
			Application.LoadLevel("LevelSelect");
		};
		_cameraFade.FadeOut();
	}

	private void HandleBackButton(SimpleButton source)
	{
		GotoPreviousScene();
	}

	private void ContinueToLevel()
	{
		_cameraFade.FadeComplete += () =>
		{
			Application.LoadLevel("BoosterSelection");
		};
		_cameraFade.FadeOut();
	}

	private Palringo.Async<bool> UpdateLockStatus()
	{
		ShowWaitScreen();
		return ContentGatingManager.Instance.UpdateLocalInformation().Then(() =>
		{
			bool flag = ContentGatingManager.Instance.CanUnlockGate(LoadLevelScreen.LevelToLoad);
			ShowContainer((!flag) ? _cannotUnlockNowContainer : _rewardContainer);
			if (!flag)
			{
				_joinPalringoButton.gameObject.SetActive(!DarkNebulaPalringo.IsAuthorized);
				_palringoExperienceBar.gameObject.SetActive(DarkNebulaPalringo.IsAuthorized);
			}
			return flag;
		});
	}

	private void ShowWaitScreen()
	{
		ShowContainer(_waitContainer);
	}

	private void ShowContainer(GameObject pContainer)
	{
		_cannotUnlockNowContainer.SetActive(pContainer == _cannotUnlockNowContainer);
		_rewardContainer.SetActive(pContainer == _rewardContainer);
		_waitContainer.SetActive(pContainer == _waitContainer);
	}

	private void HandlePalringoButton(SimpleButton button)
	{
		ShowWaitScreen();
		DarkNebulaPalringo.Connect.Authorize().Then((AuthorizationResult _) => UpdateLockStatus()).Then((bool canUnlock) =>
		{
			if (canUnlock)
			{
				DarkNebulaMetrics.UnlockGate(DarkNebulaMetrics.PassGateReason.Palringo, ContentGatingManager.Instance.GetGateIndex(LoadLevelScreen.LevelToLoad));
				ShowContainer(_rewardContainer);
				_gatePreunlockLabel.SetActive(false);
			}
		});
	}

	private void HandlePurchaseIAPButton(SimpleButton source)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			IAPLoadingPopup.Fail("NO INTERNET".Localize());
			return;
		}
		ShowWaitScreen();
		IAPLoadingPopup.Show(string.Empty);
		ContentGatingManager.Instance.PurchaseKey().Wait((bool purchasedOk) =>
		{
			if (!purchasedOk)
			{
				IAPLoadingPopup.Fail();
				ShowContainer(_cannotUnlockNowContainer);
			}
			else
			{
				UpdateLockStatus().Wait((bool canUnlock) =>
				{
					if (canUnlock)
					{
						DarkNebulaMetrics.UnlockGate(DarkNebulaMetrics.PassGateReason.IAP, ContentGatingManager.Instance.GetGateIndex(LoadLevelScreen.LevelToLoad));
						UserPurchases.Instance.HasPurchasedIAP = true;
						ShowContainer(_rewardContainer);
						_gatePreunlockLabel.SetActive(false);
						IAPLoadingPopup.HidePopup();
					}
					else
					{
						IAPLoadingPopup.Fail();
						ShowContainer(_cannotUnlockNowContainer);
					}
				});
			}
		});
	}

	private void HandleUnlockGateButton(SimpleButton source)
	{
		ShowContainer(_rewardContainer);
	}

	private void HandleAcceptRewardButton(SimpleButton source)
	{
		HardCurrencyManager.Instance.Earn(25);
		ContentGatingManager.Instance.UnlockGate(LoadLevelScreen.LevelToLoad);
		_acceptRewardBackground.gameObject.SetActive(false);
		_acceptRewardButton.gameObject.SetActive(false);
		_acceptRewardEffect.StartAnimation();
		UnlockAllEffect acceptRewardEffect = _acceptRewardEffect;
		acceptRewardEffect.EffectComplete = (Action)Delegate.Combine(acceptRewardEffect.EffectComplete, (Action)(() =>
		{
			ContinueToLevel();
		}));
	}
}
