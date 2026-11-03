using System.Collections;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class AvatarShopScreen : MonoBehaviour
{
	[SerializeField]
	private AvatarButton _avatarButtonPrefab;

	[SerializeField]
	private SimpleButton _purchaseButton;

	[SerializeField]
	private SimpleButton _equipButton;

	[SerializeField]
	private SimpleButton _backButton;

	[SerializeField]
	private Alert _alertScreen;

	[SerializeField]
	private DynamicFont _price;

	[SerializeField]
	private GameObject _priceInfo;

	[SerializeField]
	private DynamicFont _avatarName;

	[SerializeField]
	private AudioClip _swapSound;

	[SerializeField]
	private MenuHeader _menuHeader;

	[SerializeField]
	private SwipeSelector _swiper;

	public static string PreviousScene = "BoosterSelection";

	private List<AvatarButton> _avatarButtons = new List<AvatarButton>();

	private CameraFade fade;

	private bool _isEnabled = true;

	private Avatar _visibleAvatar;

	private void Start()
	{
		DarkNebulaAdsManager.Instance.ShowAd("AvatarSelect").Wait((string obj) =>
		{
			StartProcedure();
		});
	}

	private void StartProcedure()
	{
		foreach(HardCurrencyBalance balance in Object.FindObjectsByType<HardCurrencyBalance>())
			if(balance.gameObject.scene==gameObject.scene)balance.gameObject.SetActive(false);
		_equipButton.Text = "EQUIP VESSEL BUTTON".Localize();
		_purchaseButton.Text = "BUY".Localize();
		_menuHeader.SetTitleID("AVATAR HEADER");
		_alertScreen.gameObject.SetActive(false);
		_isEnabled = true;
		fade = base.gameObject.AddComponent<CameraFade>();
		fade.FadeIn();
		_purchaseButton.ButtonUpEvent += HandlePurchaseEvent;
		_menuHeader.BackButton.ButtonUpEvent += HandleBackEvent;
		_equipButton.ButtonUpEvent += HandleEquipEvent;
		InitAvatars();
		_swiper.SelectedIndexChanged += delegate
		{
			UpdateInfoText();
		};
	}

	private void InitAvatars()
	{
		float num = (Camera.main.ViewportToWorldPoint(Vector3.one).x - Camera.main.ViewportToWorldPoint(Vector3.zero).x) / 2f;
		List<string> list = new List<string>();
		int num2 = 0;
		foreach (Avatar allAvatar in AvatarManager.Instance.AllAvatars)
		{
			AvatarButton avatarButton = (AvatarButton)Object.Instantiate(_avatarButtonPrefab);
			_avatarButtons.Add(avatarButton);
			avatarButton.Avatar = allAvatar;
			avatarButton.name = allAvatar.name;
			avatarButton.transform.parent = base.transform;
			list.Add(allAvatar.LocalisedName);
			_swiper.Add(avatarButton.transform, new Vector3((float)num2 * num, 0f, 0f));
			num2++;
		}
		_swiper.SelectedIndex = _avatarButtons.FindIndex((AvatarButton x) => x.Avatar == AvatarManager.Instance.ActiveAvatar);
		UpdateInfoText();
	}

	private void UpdateInfoText()
	{
		Audio.Play("swap" + Time.realtimeSinceStartup, _swapSound);
		_visibleAvatar = _avatarButtons[_swiper.SelectedIndex].Avatar;
		_price.Text = _visibleAvatar.Price.ToString();
		_avatarName.Text = _visibleAvatar.LocalisedName;
		UpdateButtonsAndPrice();
	}

	private void BounceActiveButton()
	{
		foreach (AvatarButton avatarButton in _avatarButtons)
		{
			if (avatarButton.Avatar == _visibleAvatar)
			{
				avatarButton.Bounce();
			}
		}
	}

	private void UpdateButtonsAndPrice()
	{
		bool owned = _visibleAvatar.Owned;
		_priceInfo.SetActive(!owned);
		_purchaseButton.gameObject.SetActive(!owned);
		_equipButton.gameObject.SetActive(owned);
		foreach (AvatarButton avatarButton in _avatarButtons)
		{
			avatarButton.UpdateLockStatus();
		}
	}

	private void MoveButtonToPosition(AvatarButton pAvatarButton, Vector3 pNewPosition, bool pSnap)
	{
		if (pSnap)
		{
			pAvatarButton.transform.localPosition = pNewPosition;
		}
		else
		{
			Go.to(pAvatarButton.transform, 0.2f, new GoTweenConfig().localPosition(pNewPosition).onComplete(delegate
			{
			}));
		}
	}

	private void ShowConfirmBuyHCScreen()
	{
		_alertScreen.gameObject.SetActive(true);
		_alertScreen.SetTitle("HC NEEDED");
		_alertScreen.SetBody("HC NEEDED TEXT");
		_alertScreen.EventYES += GotoHCShop;
		_alertScreen.EventNO += HideConfirmBuyHCScreen;
	}

	private void HideConfirmBuyHCScreen()
	{
		_alertScreen.gameObject.SetActive(false);
		_alertScreen.EventYES -= GotoHCShop;
		_alertScreen.EventNO -= HideConfirmBuyHCScreen;
	}

	private void GotoHCShop()
	{
		HideConfirmBuyHCScreen();
		StartCoroutine(LoadStore());
	}

	private IEnumerator LoadStore()
	{
		IAPShopScreen.PreviousScene = Application.loadedLevelName;
		CameraFade fade = Object.FindObjectOfType<CameraFade>();
		if (fade != null)
		{
			fade.FadeOut();
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			Debug.LogWarning("Found no camera fade. There should be one...");
		}
		Application.LoadLevel("IAPShop");
	}

	private void HandlePurchaseEvent(SimpleButton pButton)
	{
		if (HardCurrencyManager.Instance.Spend(_visibleAvatar.Price))
		{
			DarkNebulaMetrics.CurrencyPurchase(_visibleAvatar.ID);
			AvatarManager.UnlockAvatar(_visibleAvatar);
			UpdateButtonsAndPrice();
			BounceActiveButton();
		}
		else
		{
			ShowConfirmBuyHCScreen();
		}
	}

	private void HandleEquipEvent(SimpleButton source)
	{
		AvatarManager.Instance.ActiveAvatar = _visibleAvatar;
		StartCoroutine(GotoPreviousScene());
	}

	private void HandleBackEvent(SimpleButton pButton)
	{
		StartCoroutine(GotoPreviousScene());
	}

	private void Update()
	{
		if (_isEnabled && Input.GetKeyUp(KeyCode.Escape))
		{
			StartCoroutine(GotoPreviousScene());
		}
	}

	private IEnumerator GotoPreviousScene()
	{
		_purchaseButton.ButtonUpEvent -= HandlePurchaseEvent;
		_menuHeader.BackButton.ButtonUpEvent -= HandleBackEvent;
		_isEnabled = false;
		fade.FadeOut();
		yield return new WaitForSeconds(0.5f);
		Application.LoadLevel(PreviousScene);
	}
}
