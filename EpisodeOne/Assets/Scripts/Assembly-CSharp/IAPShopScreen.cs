using System;
using System.Collections;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class IAPShopScreen : MonoBehaviour
{
	[SerializeField]
	private MenuHeader _menuHeader;

	[SerializeField]
	private ParticleSystem _balanceChangeParticle;

	[SerializeField]
	private UnlockAllEffect _unlockAllEffectPrefab;

	[SerializeField]
	private List<IAPShopItem> _items = new List<IAPShopItem>();

	[SerializeField]
	private HardCurrencyBalance _hcBalance;

	[SerializeField]
	private AudioClip _buySound;

	public static string PreviousScene = "MainMenu";

	private IAPLoadingPopup _loadingPopup;

	private CameraFade fade;

	private void Start()
	{
		DarkNebulaAdsManager.Instance.ShowAd("IAPShop").Wait((string obj) =>
		{
			StartProcedure();
		});
	}

	private void StartProcedure()
	{
		_menuHeader.SetTitleID("SHOP");
		Time.timeScale = 1f;
		IAPmanager.Instance.InitIAPService();
		foreach (IAPShopItem item in _items)
		{
			item.BuyClicked = (Action<IAPItem>)Delegate.Combine(item.BuyClicked, new Action<IAPItem>(HandleBuy));
		}
		_menuHeader.BackButton.ButtonUpEvent += HandleBackButton;
		fade = base.gameObject.AddComponent<CameraFade>();
		fade.FadeIn();
	}

	private void PlayIWantItAllEffect()
	{
		PreviousScene = "MainMenu";
		Audio.Play("buy", _buySound);
		UnlockAllEffect unlockAllEffect = (UnlockAllEffect)UnityEngine.Object.Instantiate(_unlockAllEffectPrefab);
		unlockAllEffect.transform.position = Camera.main.transform.position;
		unlockAllEffect.EffectComplete = (Action)Delegate.Combine(unlockAllEffect.EffectComplete, (Action)(() =>
		{
			StartCoroutine(LoadPreviousScene());
		}));
	}

	private void HandleBuy(IAPItem pItem)
	{
		DarkNebulaMetrics.PurchaseIAPTry(pItem.ID);
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			DarkNebulaMetrics.PurchaseIAPFail(pItem.ID);
			IAPLoadingPopup.Fail("NO INTERNET".Localize());
			return;
		}
		_loadingPopup = IAPLoadingPopup.Show(string.Empty);
		IAPmanager.Instance.Purchase(pItem).Wait((InAppPurchaseResult pIAPResult) =>
		{
			if (pIAPResult.State == InAppPurchaseState.Successful)
			{
				UserPurchases.Instance.HasPurchasedIAP = true;
				DarkNebulaMetrics.PurchaseIAPSuccess(pItem.ID);
				pItem.GiveItem();
				if (pItem is IAPItemHardCurrency)
				{
					_balanceChangeParticle.Play();
					Audio.Play("buy", _buySound);
				}
				else if (pItem is IAPItemIWantItAll)
				{
					PlayIWantItAllEffect();
				}
				else
				{
					Debug.LogError("Not a valid product");
				}
				IAPLoadingPopup.HidePopup();
			}
			else if (pIAPResult.State == InAppPurchaseState.Failed)
			{
				DarkNebulaMetrics.PurchaseIAPFail(pItem.ID);
				_loadingPopup.ShowFailText(string.Empty);
				Timer.Delay(2f, () =>
				{
					IAPLoadingPopup.HidePopup();
				});
			}
			else if (pIAPResult.State == InAppPurchaseState.Aborted)
			{
				DarkNebulaMetrics.PurchaseIAPCancel(pItem.ID);
				IAPLoadingPopup.HidePopup();
			}
		});
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape) && (_loadingPopup == null || !_loadingPopup.IsVisible))
		{
			StartCoroutine(LoadPreviousScene());
		}
	}

	private void HandleBackButton(SimpleButton pButton)
	{
		StartCoroutine(LoadPreviousScene());
	}

	private IEnumerator LoadPreviousScene()
	{
		_menuHeader.BackButton.ButtonUpEvent -= HandleBackButton;
		foreach (IAPShopItem item in _items)
		{
			item.BuyClicked = (Action<IAPItem>)Delegate.Remove(item.BuyClicked, new Action<IAPItem>(HandleBuy));
		}
		fade.FadeOut();
		yield return new WaitForSeconds(0.5f);
		string sceneToLoad = "MainMenu";
		if (!string.IsNullOrEmpty(PreviousScene))
		{
			sceneToLoad = PreviousScene;
		}
		else
		{
			Debug.LogWarning("No previous scene set, going back to main menu");
		}
		PlayerPrefs.Save();
		Application.LoadLevel(sceneToLoad);
	}
}
