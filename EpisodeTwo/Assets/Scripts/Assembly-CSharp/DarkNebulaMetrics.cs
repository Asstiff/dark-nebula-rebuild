using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class DarkNebulaMetrics
{
	private enum Metrics
	{
		StartGame = 0,
		StartLevel = 1,
		EndLevel = 2,
		Bought_iAP_ = 3,
		Bought_iAP_Total = 4,
		Bought_iAP_Failed = 5,
		Bought_iAP_Cancelled = 6,
		Died = 7,
		FirstTimePlaying = 8,
		InterstitialAd = 9,
		ForcedAd = 10,
		IncentiveAd = 11,
		LevelCompletedFirstTime = 12,
		Highscore = 13,
		Ad_Impression = 14,
		RemoveAds = 15,
		GameOverAction = 16,
		Application_Paused = 17,
		Application_Resumed = 18,
		SpheroConnected = 19,
		ShareFacebook = 20,
		GamesPlayed = 21,
		MuteMusic = 22,
		MuteSound = 23,
		RestorePurchase = 24,
		FacebookClicked = 25,
		FacebookLoggedout = 26,
		FacebookPartiallyAuthorized = 27,
		FacebookAuthorized = 28,
		FacebookDidNotAuthorize = 29,
		InternetCheck = 30
	}

	public enum Screens
	{
		EpisodeTwo = 0,
		MainMenu = 1,
		MainMenu_Options = 2,
		MainMenu_Sphero = 3,
		MainMenu_Leaderboard = 4,
		MainMenu_Calibrate = 5,
		LevelSelect = 6,
		Pause = 7,
		Pause_Options = 8,
		Pause_Sphero = 9,
		Pause_Calibrate = 10,
		YouAreGood = 11,
		InterstitialAd = 12,
		ForcedAd = 13,
		InsentiveAd = 14,
		RemoveAds = 15,
		Credits = 16
	}

	public enum LevelScreens
	{
		Level_ = 0,
		PostComplete_ = 1,
		LevelComplete_ = 2,
		LoadLevel_ = 3,
		GameOver_ = 4
	}

	public enum AdType
	{
		Vungle = 0,
		ChartBoost = 1,
		RemoveAds = 2
	}

	public enum RemoveAdsAction
	{
		Bought = 0,
		Cancelled = 1,
		Failed = 2,
		Ignored = 3
	}

	public enum GameOverAction
	{
		NoChoiceButMainMenu = 0,
		Continue_Success = 1,
		Continue_Abort = 2,
		Continue_Ignored = 3
	}

	[Flags]
	public enum StandardParams
	{
		None = 0,
		All = int.MaxValue,
		Default = All
	}

	private static MetricsManager _flurry;

	private static string CurrentScreenLocation;

	static DarkNebulaMetrics()
	{
		_flurry = new DummyMetricsManager(); // Legacy Flurry Java plugin is unavailable.
		_flurry.Initialize();
		uFuga.ApplicationPausedOrResumed += OnApplicationPause;
		DarkNebulaStore.PurchaseRestored += HandleDarkNebulaStorePurchaseRestored;
		OnApplicationPause(false);
	}

	public static void LogEvent(string pEvent, Dictionary<string, string> data, StandardParams pParams = StandardParams.All)
	{
		if (pParams == StandardParams.All)
		{
			AppendStandardParameters(ref data);
		}
		_flurry.LogEvent(pEvent, data);
	}

	public static void LogEvent(string pEvent, StandardParams pParams = StandardParams.All)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		LogEvent(pEvent, data, pParams);
	}

	private static void HandleDarkNebulaStorePurchaseRestored(bool pSuccess)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["success"] = pSuccess.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.RestorePurchase.ToString(), data);
	}

	public static void SpheroConnected()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.SpheroConnected.ToString(), data);
	}

	public static void HasInternet(bool pOnline, int pLevel)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["online"] = pOnline.ToString();
		data["level"] = pLevel.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.InternetCheck.ToString(), data);
	}

	public static void FacebookLoggedout()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["location"] = CurrentScreenLocation;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.FacebookLoggedout.ToString(), data);
	}

	public static void FacebookClicked()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["location"] = CurrentScreenLocation;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.FacebookClicked.ToString(), data);
	}

	public static void FacebookAuthorized()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["location"] = CurrentScreenLocation;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.FacebookAuthorized.ToString(), data);
	}

	public static void FacebookDidNotAuthorize()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["location"] = CurrentScreenLocation;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.FacebookDidNotAuthorize.ToString(), data);
	}

	public static void FacebookPartiallyAuthorized()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["location"] = CurrentScreenLocation;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.FacebookPartiallyAuthorized.ToString(), data);
	}

	public static void OnApplicationPause(bool pPause)
	{
		int dayAndUpdateDayStats = UserSettings.I.GetDayAndUpdateDayStats();
		if (pPause)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			data["day"] = dayAndUpdateDayStats.ToString();
			AppendStandardParameters(ref data);
			_flurry.LogEvent(Metrics.Application_Paused.ToString(), data);
		}
		else
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["day"] = dayAndUpdateDayStats.ToString();
			dictionary["consecutiveDay"] = UserSettings.I.GetConsecutiveDay().ToString();
			dictionary["totalDays"] = UserSettings.I.GetTotalDays().ToString();
			_flurry.LogEvent(Metrics.Application_Resumed.ToString(), dictionary);
		}
	}

	public static void RemoveAds(int levelNum, RemoveAdsAction action)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["level"] = levelNum.ToString();
		data["action"] = action.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.RemoveAds.ToString(), data);
	}

	public static void ShareFacebook(int levelNum)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["level"] = levelNum.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.ShareFacebook.ToString(), data);
	}

	public static void GameOver(int levelNum, GameOverAction action)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["level"] = levelNum.ToString();
		data["action"] = action.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.GameOverAction.ToString(), data);
	}

	public static void LevelCompletedFirstTime(int pLevelNum)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		AppendStandardParameters(ref data);
		data.Add("level", pLevelNum.ToString());
		_flurry.LogEvent(Metrics.LevelCompletedFirstTime.ToString(), data);
	}

	public static void NewHighscore(int pLevelNum, StarValue star)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["star"] = star.ToString();
		data["level"] = pLevelNum.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.Highscore.ToString(), data);
	}

	public static void ShowAd(AdType pType)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["asType"] = pType.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.Ad_Impression.ToString(), data);
	}

	private static void AppendStandardParameters(ref Dictionary<string, string> data)
	{
		data["isPayingUser"] = UserSettings.I.GetIsPayingUser().ToString();
		data["highestPlayedLevel"] = UserSettings.I.GetHighestPlayedLevel(LevelPack.Original).ToString();
		data["timesPlayed"] = UserSettings.I.GetSegmentedTimesPlayed();
		data["connectedToFacebook"] = DarkNebulaFacebook.IsLoggedIn.ToString();
		data["isExpert"] = UserSettings.I.expert.ToString();
	}

	public static void Quit()
	{
		if (!string.IsNullOrEmpty(CurrentScreenLocation))
		{
			_flurry.EndTimedEvent(CurrentScreenLocation);
		}
	}

	public static void StartScreen(Screens pScreen)
	{
		BeginTimedEvent("Screen_" + pScreen);
	}

	public static void StartLevelScreen(LevelScreens pScreen, int pCurrentLevel)
	{
		BeginTimedEvent("Screen_" + pScreen.ToString() + pCurrentLevel);
	}

	protected static void BeginTimedEvent(string screen)
	{
		if (!(CurrentScreenLocation == screen))
		{
			if (!string.IsNullOrEmpty(CurrentScreenLocation))
			{
				_flurry.EndTimedEvent(CurrentScreenLocation);
			}
			CurrentScreenLocation = screen;
			_flurry.BeginTimedEvent(CurrentScreenLocation);
		}
	}

	public static void StartGame()
	{
		if (UserSettings.I.IsFirstTimePlaying())
		{
			FirstTimePlaying();
		}
		Dictionary<string, string> data = new Dictionary<string, string>();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.StartGame.ToString(), data);
	}

	private static void FirstTimePlaying()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["languageId"] = Language.LanguageID;
		dictionary["internetReachability"] = Application.internetReachability.ToString();
		_flurry.LogEvent(Metrics.FirstTimePlaying.ToString(), dictionary);
	}

	public static void StartLevel(int pLevelNum, bool pExpert)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		AppendStandardParameters(ref data);
		data.Add("level", pLevelNum.ToString());
		_flurry.LogEvent(Metrics.StartLevel.ToString(), data);
		UserSettings.I.IncreaseTimesPlayed();
		Dictionary<string, string> data2 = new Dictionary<string, string>();
		AppendStandardParameters(ref data2);
		_flurry.LogEvent(Metrics.GamesPlayed.ToString() + "_" + UserSettings.I.GetSegmentedTimesPlayed(), data2);
	}

	public static void EndLevel(int pLevelNum, bool pExpert)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		AppendStandardParameters(ref data);
		data.Add("level", pLevelNum.ToString());
		_flurry.LogEvent(Metrics.EndLevel.ToString(), data);
		UserSettings.I.IncreaseLevelsCompleted();
	}

	public static void BoughtIAP(string pProductID)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["highestPlayedLevel"] = UserSettings.I.GetHighestPlayedLevel(LevelPack.Original).ToString();
		data["tier"] = DarkNebulaStore.GetPriceTier(pProductID).ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(string.Concat(Metrics.Bought_iAP_, pProductID), data);
		BoughtIAPTotal();
	}

	public static void MuteMusic(bool mute)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["mute"] = mute.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.MuteMusic.ToString(), data);
	}

	public static void MuteSound(bool mute)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["mute"] = mute.ToString();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.MuteSound.ToString(), data);
	}

	private static void BoughtIAPTotal()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["productID"] = DarkNebulaStore.GetBoughtProducts();
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.Bought_iAP_Total.ToString(), data);
	}

	public static void BoughtIAPFailed(string pProductID, string pReason)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["productID"] = pProductID;
		data["reason"] = pReason;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.Bought_iAP_Failed.ToString(), data);
	}

	public static void BoughtIAPCancelled(string pProductID)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["productID"] = pProductID;
		AppendStandardParameters(ref data);
		_flurry.LogEvent(Metrics.Bought_iAP_Cancelled.ToString(), data);
	}

	public static void Died(int pLevelNum, Vector3 pPosition, bool pExpert)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		AppendStandardParameters(ref data);
		data.Add("level", pLevelNum.ToString());
		_flurry.LogEvent(Metrics.Died.ToString(), data);
	}
}
