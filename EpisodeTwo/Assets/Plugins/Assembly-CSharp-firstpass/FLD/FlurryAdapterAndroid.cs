using System;
using System.Collections.Generic;

namespace FLD
{
	public class FlurryAdapterAndroid : FlurryAdapter
	{
		public override void AcceptOffer(long pOfferId)
		{
			throw new NotImplementedException();
		}

		public override void AddUserCookie(string pKey, string pValue)
		{
			throw new NotImplementedException();
		}

		public override void ClearUserCookies()
		{
			throw new NotImplementedException();
		}

		public override void DestroyBanner()
		{
			throw new NotImplementedException();
		}

		public override void EndTimedEvent(string pEventName)
		{
			FlurryAndroid.endTimedEvent(pEventName);
		}

		public override float GetScreenDensity()
		{
			return 0f;
		}

		public override void LaunchCatalogOnBannerClicked(bool pShouldLaunch)
		{
		}

		public override void LogEvent(string pLogEvent)
		{
			FlurryAndroid.logEvent(pLogEvent, false);
		}

		public override void LogEvent(string pLogEvent, Dictionary<string, string> pParameters)
		{
			FlurryAndroid.logEvent(pLogEvent, pParameters, false);
		}

		public override void LogEvent(string pLogEvent, bool pIsTimed)
		{
			FlurryAndroid.logEvent(pLogEvent, pIsTimed);
		}

		public override void LogEvent(string pLogEvent, Dictionary<string, string> pParameters, bool pIsTimed)
		{
			FlurryAndroid.logEvent(pLogEvent, pParameters, pIsTimed);
		}

		public override void OnEndSession()
		{
			FlurryAndroid.onEndSession();
		}

		public override void OnError(string pErrorId, string pMessage, string pErrorClass)
		{
			FlurryAndroid.onError(pErrorId, pMessage, pErrorClass);
		}

		public override void OnPageView()
		{
			FlurryAndroid.onPageView();
		}

		public override void OnStartSession(string pApiKey)
		{
			base.OnStartSession(pApiKey);
			FlurryAndroid.onStartSession(pApiKey, false, false);
		}

		public override void OpenCatalog(string pHookName)
		{
			throw new NotImplementedException("OpenCatalog");
		}

		public override void RemoveOffer(long pOfferId)
		{
			throw new NotImplementedException("RemoveOffer");
		}

		public override void SetAge(int pAge)
		{
			FlurryAndroid.setAge(pAge);
		}

		public override void SetContinueSessionMillis(int pMilliseconds)
		{
		}

		public override void SetDefaultNoAdsMessage(string pMessage)
		{
		}

		public override void SetLogEnabled(bool pEnabled)
		{
			FlurryAndroid.setLogEnabled(pEnabled);
		}

		public override void ShowBanner(string pHookName, int pLeftMargin, int pTopMargin)
		{
		}
	}
}
