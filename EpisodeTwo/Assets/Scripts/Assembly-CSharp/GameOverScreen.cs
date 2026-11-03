using FLD;
using UnityEngine;

public class GameOverScreen : SimpleScreen
{
	public tk2dTextMesh CountdownLabel;

	public tk2dTextMesh Description;

	private string _yesID = "YES";

	private string _noID = "NO";

	private int countdown = 5;

	private DarkNebulaMetrics.GameOverAction action;

	public event HandlerDelegate EventResume;

	public event HandlerDelegate EventMainMenu;

	public event HandlerDelegate EventContinue;

	private void Awake()
	{
		btn0.GetComponentInChildren<SimpleText>().SetStringID(_yesID);
		// The former rewarded-ad button is disabled in this build.
		btn1.GetComponentInChildren<SimpleText>().SetStringID(_noID);
		btn1.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventMainMenu != null)
			{
				EventMainMenu();
			}
		};
		btn2.GetComponentInChildren<SimpleText>().SetStringID("CONTINUE");
		btn2.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			DarkNebulaMetrics.GameOver(World.levelNum, action);
			World.vessel.GainLives();
			base.gameObject.SetActive(false);
			if (EventResume != null)
			{
				EventResume();
			}
		};
		btn2.gameObject.SetActive(false);
		btn3.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventContinue != null)
			{
				EventContinue();
			}
		};
		btn3.gameObject.SetActive(false);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		DarkNebulaMetrics.StartLevelScreen(DarkNebulaMetrics.LevelScreens.GameOver_, World.levelNum);
		ShowOnlyMainMenuButton();
		action = DarkNebulaMetrics.GameOverAction.NoChoiceButMainMenu;
	}

	protected void ShowGainLivesScreen()
	{
		SetTitle("CONTINUE?");
		SetNo("MAIN MENU");
		SetYes("GET 3 LIVES!");
		Description.text = Language.Get("GAIN LIVES");
		Description.Commit();
		btn2.SetActive(false);
		countdown = 5;
		this.InvokeRealtime("Countdown", 1f);
	}

	private void ShowMainMenuButton()
	{
		ShowMainMenuButton(true);
	}

	private void ShowMainMenuButton(bool show)
	{
		btn1.SetActive(show);
	}

	private void ShowOnlyMainMenuButton()
	{
		btn0.SetActive(false);
		btn1.SetActive(true);
		btn2.SetActive(false);
		btn3.SetActive(BuildSettings.Current.GameOverAllowContinue);
		Description.gameObject.SetActive(false);
		CountdownLabel.gameObject.SetActive(false);
		SetTitle("GAME OVER");
		SetNo("MAIN MENU");
	}

	private void OnDestroy()
	{
	}

	private void OnViewVideoAndContinue()
	{
		ShowOnlyMainMenuButton();
	}

	private void ShowReward()
	{
		btn0.SetActive(false);
		btn1.SetActive(false);
		btn2.SetActive(true);
		btn3.SetActive(false);
		CountdownLabel.gameObject.SetActive(false);
		Description.gameObject.SetActive(false);
		Hud.disablePauseMenu();
	}

	private void OnViewAd(double watched, double length)
	{
		VungleManager.Instance.AdView -= OnViewAd;
		DarkNebulaMetrics.StartLevelScreen(DarkNebulaMetrics.LevelScreens.GameOver_, World.levelNum);
		DarkNebulaAds.ReportHaveSeenAd();
		if (watched / length >= 0.8)
		{
			Debug.Log("User watched >= 80% of the ad, add 3 lives!");
			action = DarkNebulaMetrics.GameOverAction.Continue_Success;
			ShowReward();
		}
		else
		{
			Debug.Log("User did not watch 80% of the ad, goto main menu");
			action = DarkNebulaMetrics.GameOverAction.Continue_Abort;
			OnGotoMainMenu();
		}
	}

	private void Countdown()
	{
		countdown--;
		if (countdown < 0)
		{
			OnGotoMainMenu();
			return;
		}
		CountdownLabel.text = countdown.ToString();
		CountdownLabel.Commit();
		this.CancelInvokeRealtime("Countdown");
		this.InvokeRealtime("Countdown", 1f);
	}

	private void OnGotoMainMenu()
	{
		this.CancelInvokeRealtime("Countdown");
		DarkNebulaMetrics.GameOver(World.levelNum, action);
		if (EventMainMenu != null)
		{
			EventMainMenu();
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
