using System.Collections.Generic;

namespace FLD
{
	public class FlurryAdapterDummy : FlurryAdapter
	{
		public override void AcceptOffer(long pOfferId)
		{
		}

		public override void AddUserCookie(string pKey, string pValue)
		{
		}

		public override void ClearUserCookies()
		{
		}

		public override void DestroyBanner()
		{
		}

		public override void EndTimedEvent(string pEventName)
		{
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
		}

		public override void LogEvent(string pLogEvent, Dictionary<string, string> pParameters)
		{
		}

		public override void LogEvent(string pLogEvent, bool pIsTimed)
		{
		}

		public override void LogEvent(string pLogEvent, Dictionary<string, string> pParameters, bool pIsTimed)
		{
		}

		public override void OnEndSession()
		{
		}

		public override void OnError(string pErrorId, string pMessage, string pErrorClass)
		{
		}

		public override void OnPageView()
		{
		}

		public override void OnStartSession(string pApiKey)
		{
		}

		public override void OpenCatalog(string pHookName)
		{
		}

		public override void RemoveOffer(long pOfferId)
		{
		}

		public override void SetAge(int pAge)
		{
		}

		public override void SetContinueSessionMillis(int pMilliseconds)
		{
		}

		public override void SetDefaultNoAdsMessage(string pMessage)
		{
		}

		public override void SetLogEnabled(bool pEnabled)
		{
		}

		public override void ShowBanner(string pHookName, int pLeftMargin, int pTopMargin)
		{
		}
	}
}
