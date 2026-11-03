using System;
using Prime31;

public class ChartBoostAndroidManager : AbstractManager
{
	public static event Action didFailToLoadMoreAppsEvent;

	public static event Action<string> didCacheInterstitialEvent;

	public static event Action didCacheMoreAppsEvent;

	public static event Action<string> didFinishInterstitialEvent;

	public static event Action<string> didFinishMoreAppsEvent;

	public static event Action didCloseMoreAppsEvent;

	public static event Action<string> didFailToLoadInterstitialEvent;

	public static event Action<string> didShowInterstitialEvent;

	public static event Action didShowMoreAppsEvent;

	static ChartBoostAndroidManager()
	{
		AbstractManager.initialize(typeof(ChartBoostAndroidManager));
	}

	public void didFailToLoadMoreApps(string empty)
	{
		if (didFailToLoadMoreAppsEvent != null)
		{
			didFailToLoadMoreAppsEvent();
		}
	}

	public void didCacheInterstitial(string location)
	{
		if (didCacheInterstitialEvent != null)
		{
			didCacheInterstitialEvent(location);
		}
	}

	public void didCacheMoreApps(string empty)
	{
		if (didCacheMoreAppsEvent != null)
		{
			didCacheMoreAppsEvent();
		}
	}

	public void didFinishInterstitial(string param)
	{
		if (didFinishInterstitialEvent != null)
		{
			didFinishInterstitialEvent(param);
		}
	}

	public void didFinishMoreApps(string param)
	{
		if (didFinishMoreAppsEvent != null)
		{
			didFinishMoreAppsEvent(param);
		}
	}

	public void didCloseMoreApps(string empty)
	{
		if (didCloseMoreAppsEvent != null)
		{
			didCloseMoreAppsEvent();
		}
	}

	public void didFailToLoadInterstitial(string location)
	{
		if (didFailToLoadInterstitialEvent != null)
		{
			didFailToLoadInterstitialEvent(location);
		}
	}

	public void didShowInterstitial(string location)
	{
		if (didShowInterstitialEvent != null)
		{
			didShowInterstitialEvent(location);
		}
	}

	public void didShowMoreApps(string empty)
	{
		if (didShowMoreAppsEvent != null)
		{
			didShowMoreAppsEvent();
		}
	}
}
