using FLD;
using UnityEngine;

public class DarkNebulaAds
{
	public enum AdAction
	{
		None = 0,
		ShowRemoveAds = 1,
		ShowAdSkippable = 2,
		ShowAdForced = 3
	}

	public const int ShowGainLivesAdsFromLevel = 1;

	private const int SkipableAdInterval = 4;

	public const string GainLives = "GainLives";

	public const string BeforeNewLevel = "BeforeNewLevel";

	private static VungleManagerAbstract _vungle;

	private static AdManager _chartBoost;

	private static bool _initialized;

	private static int _skipableAdCounter;

	public static bool IsAboutToLeaveAppAndComeBack;

	public static AdManager ChartBoost
	{
		get
		{
			Init();
			return _chartBoost;
		}
		private set
		{
			_chartBoost = value;
		}
	}

	public static VungleManagerAbstract Vungle
	{
		get
		{
			Init();
			return _vungle;
		}
		private set
		{
			_vungle = value;
		}
	}

	public static void ReportAboutToSeeAd()
	{
		IsAboutToLeaveAppAndComeBack = true;
	}

	public static void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			Vungle = VungleManager.Instance;
			Vungle.init(BuildSettings.Current.VungleAppId);
			if (BuildSettings.Current.ChartBoostEnabled)
			{
				ChartBoost = new ChartboostAdManager();
			}
			if (ChartBoost == null)
			{
				ChartBoost = new DummyAdManager();
			}
			ChartBoost.Initialize(BuildSettings.Current.ChartBoostAppId, BuildSettings.Current.ChartBoostAppSignature);
			PrepareChartBoostAds();
		}
	}

	public static void PrepareChartBoostAds(int levelNum = 0)
	{
		Debug.Log("Prepare ChartBoost Ads");
		ChartBoost.PrepareAd("BeforeNewLevel", AdManager.AdType.Fullscreen);
	}

	public static AdAction GetShowAdBeforePlayingLevel(int pLevelNum)
	{
		_skipableAdCounter = (_skipableAdCounter + 1) % 4;
		LevelPack packFromLevelNum = UserSettings.I.GetPackFromLevelNum(pLevelNum);
		int num = UserSettings.I.GetPackMinLevel(packFromLevelNum) - 1;
		int relativeLevel = UserSettings.I.GetRelativeLevel(pLevelNum, packFromLevelNum);
		if (BuildSettings.Current.ChartBoostEnabled || BuildSettings.Current.VungleEnabled)
		{
			if (relativeLevel < 3)
			{
				return AdAction.None;
			}
			if (UserSettings.I.GetHighestPlayedLevel(packFromLevelNum) - num < relativeLevel)
			{
				if ((relativeLevel == 5 || relativeLevel == 9) && DarkNebulaStore.IsReadyForPurchase)
				{
					return AdAction.ShowRemoveAds;
				}
				return AdAction.ShowAdForced;
			}
			if (_skipableAdCounter == 2)
			{
				int num2 = UserSettings.I.GetSkipableAdsSeen() + 1;
				UserSettings.I.SetSkipableAdsSeen(num2);
				if (num2 % 4 == 2 && DarkNebulaStore.IsReadyForPurchase)
				{
					return AdAction.ShowRemoveAds;
				}
				return AdAction.ShowAdSkippable;
			}
		}
		return AdAction.None;
	}

	public static void ReportHaveSeenAd()
	{
		_skipableAdCounter = 0;
		IsAboutToLeaveAppAndComeBack = false;
	}
}
