using System;

namespace FLD
{
	public class DummyAdManager : AdManager
	{
		public override void Initialize(string pAppId, string pAppSecret)
		{
		}

		public override void RemoveAd(string pAdLocation)
		{
		}

		public override void HideAd(string pAdLocation)
		{
		}

		public override void UnhideAd(string pAdLocation)
		{
		}

		public override void PrepareAd(string pAdLocation, AdType pType)
		{
		}

		public override bool HasPreparedAd(string pAdLocation, AdType pType)
		{
			return false;
		}

		public override void ShowAd(string pAdLocation, AdType pType, int pPosition, Action pOnClosed)
		{
		}
	}
}
