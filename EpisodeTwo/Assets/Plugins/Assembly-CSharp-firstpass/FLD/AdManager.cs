using System;
using System.Collections.Generic;

namespace FLD
{
	public abstract class AdManager
	{
		public enum AdType
		{
			Banner = 0,
			Fullscreen = 1
		}

		public enum UserGender
		{
			Unknown = 0,
			Male = 1,
			Female = 2
		}

		public struct UserDemographics
		{
			public UserGender Gender;

			public int Age;
		}

		public const int PositionTop = -10000;

		public const int PositionBottom = -10001;

		protected List<string> _keywords = new List<string>();

		protected Dictionary<string, int> _showAdInvocationCounters = new Dictionary<string, int>();

		protected UserDemographics _userDemographics = default(UserDemographics);

		private static AdManager _instance;

		public virtual bool IsAvailable
		{
			get
			{
				if (uFuga.Storage.persistent.Has("DisableAds"))
				{
					return !uFuga.Storage.persistent.GetBool("DisableAds");
				}
				return true;
			}
		}

		public virtual IEnumerable<string> Keywords
		{
			get
			{
				return _keywords;
			}
			set
			{
				_keywords = new List<string>(value);
			}
		}

		public static AdManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DummyAdManager();
				}
				return _instance;
			}
		}

		public virtual void Enable()
		{
			uFuga.Storage.persistent.Set("DisableAds", false, true);
		}

		public virtual void Disable()
		{
			uFuga.Storage.persistent.Set("DisableAds", true, true);
		}

		public bool ShowAdEveryXTimes(int pStartCounter, int pHowOften, string pAdLocation, AdType pType, int pPosition, Action pOnClosed)
		{
			if (pHowOften == 0)
			{
				if (pOnClosed != null)
				{
					pOnClosed();
				}
				return false;
			}
			int value = 0;
			if (!_showAdInvocationCounters.TryGetValue(pAdLocation, out value))
			{
				value = pStartCounter;
			}
			value--;
			bool result = false;
			if (value <= 0)
			{
				if (HasPreparedAd(pAdLocation, pType))
				{
					value = pHowOften;
					ShowAd(pAdLocation, pType, pPosition, pOnClosed);
					result = true;
				}
				else
				{
					value = 1;
					if (pOnClosed != null)
					{
						pOnClosed();
					}
				}
			}
			else if (pOnClosed != null)
			{
				pOnClosed();
			}
			_showAdInvocationCounters[pAdLocation] = value;
			return result;
		}

		public void SetUserDemographics(UserDemographics pDemographics)
		{
			_userDemographics = pDemographics;
		}

		public void ShowAd(string pAdLocation, AdType pType, Action pOnClosed)
		{
			ShowAd(pAdLocation, pType, 0, pOnClosed);
		}

		public void ShowAd(string pAdLocation, AdType pType)
		{
			ShowAd(pAdLocation, pType, 0, null);
		}

		public void ShowAd(string pAdLocation, AdType pType, int pPosition)
		{
			ShowAd(pAdLocation, pType, pPosition, null);
		}

		public abstract void Initialize(string pAppId, string pAppSecret);

		public abstract void RemoveAd(string pAdLocation);

		public abstract void HideAd(string pAdLocation);

		public abstract void UnhideAd(string pAdLocation);

		public abstract void PrepareAd(string pAdLocation, AdType pType);

		public abstract bool HasPreparedAd(string pAdLocation, AdType pType);

		public abstract void ShowAd(string pAdLocation, AdType pType, int pPosition, Action pOnClosed);
	}
}
