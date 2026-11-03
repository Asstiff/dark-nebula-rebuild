using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public abstract class FlurryAdapter
	{
		private static GameObject _gameObject;

		private static FlurryAdapter _instance;

		private string _apiKey = BuildSettings.Current.MetricKey;

		public static FlurryAdapter Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FlurryAdapterAndroid();
				}
				return _instance;
			}
		}

		protected FlurryAdapter()
		{
			uFuga.Instance.onApplicationPause += OnPaused;
			uFuga.Instance.onApplicationQuit += OnQuit;
			OnPaused(false);
		}

		protected void OnPaused(bool paused)
		{
			if (!paused)
			{
				OnStartSession(_apiKey);
			}
			else
			{
				OnEndSession();
			}
		}

		protected void OnQuit()
		{
			OnEndSession();
		}

		public virtual void OnStartSession(string pApiKey)
		{
			_apiKey = pApiKey;
		}

		public void OnStartSession()
		{
			if (!string.IsNullOrEmpty(_apiKey))
			{
				OnStartSession(_apiKey);
			}
		}

		public abstract void AcceptOffer(long pOfferId);

		public abstract void AddUserCookie(string pKey, string pValue);

		public abstract void ClearUserCookies();

		public abstract void DestroyBanner();

		public abstract void EndTimedEvent(string pEventName);

		public abstract float GetScreenDensity();

		public abstract void LaunchCatalogOnBannerClicked(bool pShouldLaunch);

		public abstract void LogEvent(string pLogEvent);

		public abstract void LogEvent(string pLogEvent, Dictionary<string, string> pParameters);

		public abstract void LogEvent(string pLogEvent, Dictionary<string, string> pParameters, bool pIsTimed);

		public abstract void LogEvent(string pLogEvent, bool pIsTimed);

		public abstract void OnEndSession();

		public abstract void OnError(string pErrorId, string pMessage, string pErrorClass);

		public abstract void OnPageView();

		public abstract void OpenCatalog(string pHookName);

		public abstract void RemoveOffer(long pOfferId);

		public abstract void SetAge(int pAge);

		public abstract void SetContinueSessionMillis(int pMilliseconds);

		public abstract void SetDefaultNoAdsMessage(string pMessage);

		public abstract void SetLogEnabled(bool pEnabled);

		public abstract void ShowBanner(string pHookName, int pLeftMargin, int pTopMargin);
	}
}
