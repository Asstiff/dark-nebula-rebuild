using System;

public class RemoveAdsScreen : SimpleScreen
{
	public tk2dTextMesh Description;

	private DarkNebulaMetrics.RemoveAdsAction action = DarkNebulaMetrics.RemoveAdsAction.Ignored;

	private string _yesID;

	private string _noID;

	public event Action EventPlay;

	private void Awake()
	{
		SetTitle("REMOVE ADS?");
		btn0.GetComponentInChildren<SimpleText>().SetStringID("REMOVE ADS");
		btn0.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			ClickedRemoveAdsButton();
		};
		btn1.GetComponentInChildren<SimpleText>().SetStringID("PLAY");
		btn1.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			DarkNebulaMetrics.RemoveAds(World.levelNum, action);
			if (EventPlay != null)
			{
				EventPlay();
			}
		};
	}

	public void ShowRemoveAdsScreen(bool pHideButton = true)
	{
		if (!(base.gameObject == null))
		{
			DarkNebulaMetrics.ShowAd(DarkNebulaMetrics.AdType.RemoveAds);
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.RemoveAds);
			base.gameObject.SetActive(true);
			Hud.disablePauseMenu();
			btn0.SetActive(true);
			Description.text = Language.Get("REMOVE ADS TEXT");
			Description.Commit();
			Description.gameObject.SetActive(true);
			if (pHideButton)
			{
				ShowPlayButton(false);
				this.InvokeRealtime("ShowPlayButton", 5f);
			}
			else
			{
				ShowPlayButton();
			}
		}
	}

	private void ShowPlayButton()
	{
		ShowPlayButton(true);
	}

	private void ShowPlayButton(bool show)
	{
		btn1.SetActive(show);
	}

	private void ClickedRemoveAdsButton()
	{
		this.CancelInvokeRealtime("ShowPlayButton");
		ShowPlayButton(false);
		Description.text = Language.Get("CONNECTING TO STORE");
		Description.Commit();
		btn0.SetActive(false);
		DarkNebulaStore.Instance.onPurchaseFailed += OnFailedPurchase;
		DarkNebulaStore.Instance.onPurchaseFinished += OnSuccededPurchase;
		DarkNebulaStore.Instance.TryPurchaseRemoveAds();
	}

	public void OnSuccededPurchase(string pId)
	{
		Description.gameObject.SetActive(true);
		Description.text = Language.Get("THANK YOU!");
		Description.Commit();
		btn1.gameObject.SetActive(true);
		ShowPlayButton();
		DarkNebulaMetrics.BoughtIAP("darknebula.ep2.removeads");
		action = DarkNebulaMetrics.RemoveAdsAction.Bought;
	}

	public void OnFailedPurchase(bool pCanceled, string pId, string pMessage)
	{
		ShowRemoveAdsScreen(false);
		if (pCanceled)
		{
			DarkNebulaMetrics.BoughtIAPCancelled("darknebula.ep2.removeads");
			action = DarkNebulaMetrics.RemoveAdsAction.Cancelled;
		}
		else
		{
			DarkNebulaMetrics.BoughtIAPFailed("darknebula.ep2.removeads", pMessage);
			action = DarkNebulaMetrics.RemoveAdsAction.Failed;
		}
	}

	public void SetTitle(string stringID)
	{
		lblTitle.GetComponentInChildren<SimpleText>().SetStringID(stringID);
	}

	public void SetTitleText(string str)
	{
		lblTitle.GetComponentInChildren<SimpleText>().SetText(str);
	}

	public void SetYes(string stringID)
	{
		_yesID = stringID;
		btn0.GetComponentInChildren<SimpleText>().SetStringID(_yesID);
	}

	public void SetNo(string stringID)
	{
		_noID = stringID;
		btn1.GetComponentInChildren<SimpleText>().SetStringID(_noID);
	}
}
