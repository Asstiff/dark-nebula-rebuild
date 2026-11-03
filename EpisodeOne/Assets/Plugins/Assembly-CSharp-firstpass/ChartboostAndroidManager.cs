using System;
using Prime31;

public class ChartboostAndroidManager : AbstractManager
{
	public static event Action<string> didCacheInterstitialEvent;

	public static event Action<string> didFailToCacheInterstitialEvent;

	public static event Action<string, string> didFinishInterstitialEvent;

	public static event Action<string> didCacheMoreAppsEvent;

	public static event Action<string> didFailToCacheMoreAppsEvent;

	public static event Action<string, string> didFinishMoreAppsEvent;

	public static event Action<string> didCacheRewardedVideoEvent;

	public static event Action<string> didFailToLoadRewardedVideoEvent;

	public static event Action<string, string> didFinishRewardedVideoEvent;

	public static event Action<int> didCompleteRewardedVideoEvent;

	public static event Action<string> didFailToLoadUrlEvent;

	static ChartboostAndroidManager()
	{
		AbstractManager.initialize(typeof(ChartboostAndroidManager));
	}

	public void didCacheInterstitial(string location)
	{
		if (didCacheInterstitialEvent != null)
		{
			didCacheInterstitialEvent(location);
		}
	}

	public void didFailToLoadInterstitial(string location)
	{
		if (didFailToCacheInterstitialEvent != null)
		{
			didFailToCacheInterstitialEvent(location);
		}
	}

	public void didDismissInterstitial(string location)
	{
		if (didFinishInterstitialEvent != null)
		{
			didFinishInterstitialEvent(location, "dismiss");
		}
	}

	public void didClickInterstitial(string location)
	{
		if (didFinishInterstitialEvent != null)
		{
			didFinishInterstitialEvent(location, "click");
		}
	}

	public void didCloseInterstitial(string location)
	{
		if (didFinishInterstitialEvent != null)
		{
			didFinishInterstitialEvent(location, "close");
		}
	}

	public void didCacheMoreApps(string location)
	{
		if (didCacheMoreAppsEvent != null)
		{
			didCacheMoreAppsEvent(location);
		}
	}

	public void didFailToLoadMoreApps(string location)
	{
		if (didFailToCacheMoreAppsEvent != null)
		{
			didFailToCacheMoreAppsEvent(location);
		}
	}

	public void didDismissMoreApps(string location)
	{
		if (didFinishMoreAppsEvent != null)
		{
			didFinishMoreAppsEvent(location, "dismiss");
		}
	}

	public void didCloseMoreApps(string location)
	{
		if (didFinishMoreAppsEvent != null)
		{
			didFinishMoreAppsEvent(location, "close");
		}
	}

	public void didClickMoreApps(string location)
	{
		if (didFinishMoreAppsEvent != null)
		{
			didFinishMoreAppsEvent(location, "click");
		}
	}

	public void didCacheRewardedVideo(string location)
	{
		if (didCacheRewardedVideoEvent != null)
		{
			didCacheRewardedVideoEvent(location);
		}
	}

	public void didFailToLoadRewardedVideo(string location)
	{
		if (didFailToLoadRewardedVideoEvent != null)
		{
			didFailToLoadRewardedVideoEvent(location);
		}
	}

	public void didDismissRewardedVideo(string location)
	{
		if (didFinishRewardedVideoEvent != null)
		{
			didFinishRewardedVideoEvent(location, "dismiss");
		}
	}

	public void didCloseRewardedVideo(string location)
	{
		if (didFinishRewardedVideoEvent != null)
		{
			didFinishRewardedVideoEvent(location, "close");
		}
	}

	public void didClickRewardedVideo(string location)
	{
		if (didFinishRewardedVideoEvent != null)
		{
			didFinishRewardedVideoEvent(location, "click");
		}
	}

	public void didCompleteRewardedVideo(string reward)
	{
		if (didCompleteRewardedVideoEvent != null)
		{
			didCompleteRewardedVideoEvent(int.Parse(reward));
		}
	}

	public void didFailToLoadUrl(string url)
	{
		if (didFailToLoadUrlEvent != null)
		{
			didFailToLoadUrlEvent(url);
		}
	}
}
