using System;
using Prime31;

public class Chartboost
{
	public static event Action<string> didCacheInterstitialEvent;

	public static event Action<string> didFailToCacheInterstitialEvent;

	public static event Action<string, string> didFinishInterstitialEvent;

	public static event Action<string> didCacheMoreAppsEvent;

	public static event Action<string> didFailToCacheMoreAppsEvent;

	public static event Action<string, string> didFinishMoreAppsEvent;

	static Chartboost()
	{
		ChartboostAndroidManager.didCacheInterstitialEvent += (string location) =>
		{
			didCacheInterstitialEvent.fire(location);
		};
		ChartboostAndroidManager.didFailToCacheInterstitialEvent += (string location) =>
		{
			didFailToCacheInterstitialEvent.fire(location);
		};
		ChartboostAndroidManager.didFinishInterstitialEvent += (string location, string reason) =>
		{
			didFinishInterstitialEvent.fire(location, reason);
		};
		ChartboostAndroidManager.didCacheMoreAppsEvent += (string location) =>
		{
			didCacheMoreAppsEvent.fire(location);
		};
		ChartboostAndroidManager.didFailToCacheMoreAppsEvent += (string location) =>
		{
			didFailToCacheMoreAppsEvent.fire(location);
		};
		ChartboostAndroidManager.didFinishMoreAppsEvent += (string location, string reason) =>
		{
			didFinishMoreAppsEvent.fire(location, reason);
		};
	}

	public static void init(string androidAppId, string androidAppSignature, string iosAppId, string iosAppSignature, bool shouldRequestInterstitialsInFirstSession = true)
	{
		ChartboostAndroid.init(androidAppId, androidAppSignature, shouldRequestInterstitialsInFirstSession);
	}

	public static void cacheInterstitial(string location = "None")
	{
		ChartboostAndroid.cacheInterstitial(location);
	}

	public static bool hasCachedInterstitial(string location)
	{
		return ChartboostAndroid.hasCachedInterstitial(location);
	}

	public static void showInterstitial(string location)
	{
		ChartboostAndroid.showInterstitial(location);
	}

	public static void cacheMoreApps(string location)
	{
		ChartboostAndroid.cacheMoreApps(location);
	}

	public static bool hasCachedMoreApps(string location)
	{
		return ChartboostAndroid.hasCachedMoreApps(location);
	}

	public static void showMoreApps(string location)
	{
		ChartboostAndroid.showMoreApps(location);
	}
}
