using System;
using UnityEngine;

namespace FLD
{
	public class ChartboostAdManager : AdManager
	{
		private Action _closeCallback;

		public override bool IsAvailable
		{
			get
			{
				return base.IsAvailable;
			}
		}

		public override void Initialize(string pAppId, string pAppSecret)
		{
			GameObject gameObject = new GameObject("ChartBoostManager");
			gameObject.AddComponent<ChartBoostAndroidManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.name = "ChartBoostAndroidManager";
			ChartBoostAndroidManager.didFinishInterstitialEvent += HandleChartBoostManagerdidCloseInterstitialEvent;
			ChartBoostAndroid.init(pAppId, pAppSecret);
			ChartBoostAndroid.onStart();
		}

		private void HandleChartBoostManagerdidCloseInterstitialEvent(string obj)
		{
			if (_closeCallback != null)
			{
				Action closeCallback = _closeCallback;
				_closeCallback = null;
				closeCallback();
			}
		}

		public override void PrepareAd(string pAdLocation, AdType pType)
		{
			if (pType == AdType.Fullscreen)
			{
				ChartBoostAndroid.cacheInterstitial(pAdLocation);
			}
		}

		public override bool HasPreparedAd(string pAdLocation, AdType pType)
		{
			if (Application.isEditor)
			{
				return true;
			}
			if (pType == AdType.Fullscreen)
			{
				return ChartBoostAndroid.hasCachedInterstitial(pAdLocation);
			}
			return false;
		}

		public override void ShowAd(string pAdLocation, AdType pType, int pPosition, Action pOnClosed)
		{
			if (Application.isEditor)
			{
				if (pOnClosed != null)
				{
					pOnClosed();
				}
				return;
			}
			if (pType == AdType.Fullscreen)
			{
				_closeCallback = pOnClosed;
				ChartBoostAndroid.showInterstitial(pAdLocation);
				return;
			}
			Debug.LogWarning(string.Format("Can't show ads of type {0} with this provider", pType));
			if (pOnClosed != null)
			{
				pOnClosed();
			}
		}

		public override void HideAd(string pAdLocation)
		{
		}

		public override void UnhideAd(string pAdLocation)
		{
		}

		public override void RemoveAd(string pAdLocation)
		{
		}
	}
}
