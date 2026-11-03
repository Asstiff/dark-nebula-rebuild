using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	[Service("Chartboost", Description = "Provides chartboost.", AvailablePlatforms = "iPhone,Android")]
	public class ChartboostAdsService : IService, IInterstitialAdsService
	{
		private Async<string> _showingInterstitialAsr;

		private Dictionary<string, Async<string>> _locationAsrs = new Dictionary<string, Async<string>>();

		[ServiceConfigurable]
		public string AppId { get; set; }

		[ServiceConfigurable]
		public string AppSignature { get; set; }

		public Async<bool> Initialize()
		{
			Chartboost.init(AppId, AppSignature, AppId, AppSignature);
			Chartboost.didFinishInterstitialEvent += HandledidFinishInterstitialEvent;
			Chartboost.didFailToCacheInterstitialEvent += HandleFailToCacheInterstitialEvent;
			Chartboost.didCacheInterstitialEvent += HandleDidCacheInterstitialEvent;
			return Async<bool>.MakePrecompleted(true);
		}

		public Async<string> ShowInterstitial(string pLocation)
		{
			if (_showingInterstitialAsr != null)
			{
				Debug.LogWarning("Busy showing other interstitial");
				return _showingInterstitialAsr;
			}
			if (HasCachedInterstitial(pLocation))
			{
				Chartboost.showInterstitial(pLocation);
				_showingInterstitialAsr = new Async<string>();
				return _showingInterstitialAsr;
			}
			CacheInterstitial(pLocation);
			return Async<string>.MakePrecompleted(string.Empty);
		}

		public Async<string> CacheInterstitial(string pLocation)
		{
			if (HasCachedInterstitial(pLocation))
			{
				return Async<string>.MakePrecompleted(string.Empty);
			}
			if (_locationAsrs.ContainsKey(pLocation))
			{
				return _locationAsrs[pLocation];
			}
			Async<string> async = new Async<string>();
			_locationAsrs.Add(pLocation, async);
			Chartboost.cacheInterstitial(pLocation);
			return async;
		}

		public bool HasCachedInterstitial(string pLocation)
		{
			return Chartboost.hasCachedInterstitial(pLocation);
		}

		private void HandleDidCacheInterstitialEvent(string pLocation)
		{
			if (_locationAsrs.ContainsKey(pLocation))
			{
				Async<string> async = _locationAsrs[pLocation];
				_locationAsrs.Remove(pLocation);
				if (async == null)
				{
					Debug.LogWarning("Interstitial cache async object is null when finished caching interstitial for location " + pLocation);
				}
				else
				{
					async.Complete(pLocation);
				}
			}
		}

		private void HandledidFinishInterstitialEvent(string pLocation, string pReason)
		{
			if (_showingInterstitialAsr == null)
			{
				Debug.LogWarning("Interstitial async object is null when finished showing interstitial");
				return;
			}
			Async<string> showingInterstitialAsr = _showingInterstitialAsr;
			_showingInterstitialAsr = null;
			showingInterstitialAsr.Complete(pReason);
		}

		private void HandleFailToCacheInterstitialEvent(string pLocation)
		{
			Debug.LogWarning("Failed to cache interstitial location " + pLocation);
			if (_locationAsrs.ContainsKey(pLocation))
			{
				Async<string> async = _locationAsrs[pLocation];
				_locationAsrs.Remove(pLocation);
				async.Complete(pLocation);
			}
		}
	}
}
