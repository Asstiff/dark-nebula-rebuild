using System;
using FLD;

public class RemoveAdsScreen : SimpleScreen
{
	public enum RemoveAdsAction
	{
		Bought = 0,
		Cancelled = 1,
		Failed = 2,
		Ignored = 3
	}

	public tk2dTextMesh Description;

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
			if (EventPlay != null)
			{
				EventPlay();
			}
		};
	}

	public void ShowRemoveAdsScreen(bool pHideButton = true)
	{
		base.gameObject.SetActive(true);
		Hud.disablePauseMenu();
		btn0.SetActive(true);
		Description.text = Language.Get("TIRED OF ADS?\nWANT TO SUPPORT THE DEVELOPERS?");
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
		Description.text = "CONNECTING TO STORE".Localize();
		Description.Commit();
		btn0.SetActive(false);
	}

	public void OnSuccededPurchase(string pId)
	{
		if (!(base.gameObject == null))
		{
			Description.gameObject.SetActive(true);
			Description.text = "THANK YOU!".Localize();
			Description.Commit();
			btn1.gameObject.SetActive(true);
			ShowPlayButton();
		}
	}

	public void OnFailedPurchase(bool pCanceled, string pId, string pMessage)
	{
		if (!(base.gameObject == null))
		{
			ShowRemoveAdsScreen(false);
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
