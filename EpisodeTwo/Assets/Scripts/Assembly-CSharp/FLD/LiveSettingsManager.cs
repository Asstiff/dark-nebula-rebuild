using System;
using System.Collections;
using System.Text;
using FLD.JSON;
using UnityEngine;

namespace FLD
{
	public class LiveSettingsManager
	{
		public struct AdSpotSettings
		{
			public int First;

			public int Interval;

			internal void Serialize(SimpleJSerializer s)
			{
				s.Serialize("first", ref First);
				s.Serialize("interval", ref Interval);
			}
		}

		private int _lastUpdatedTimestamp;

		private int _daysBetweenUpdates;

		private string _lastInstalledVersionString;

		private VersionNumber _latestLiveVersion;

		private Action _loadedCallback;

		public string ABCohort;

		public string CrossPromoURL;

		public bool CrossPromo;

		public Action SettingsUpdated;

		public VersionNumber LatestLiveVersion
		{
			get
			{
				return _latestLiveVersion;
			}
		}

		public bool IsLiveVersion
		{
			get
			{
				return BuildVersion.Version <= LatestLiveVersion;
			}
		}

		public static LiveSettingsManager Instance { get; private set; }

		private LiveSettingsManager()
		{
		}

		static LiveSettingsManager()
		{
			Instance = new LiveSettingsManager();
		}

		private void Serialize(SimpleJSerializer s)
		{
			s.Serialize("_lastUpdated", ref _lastUpdatedTimestamp);
			s.Serialize("_lastInstalledVersion", ref _lastInstalledVersionString);
			if (s.Mode == SimpleJSerializer.SerializationMode.SerializeFromJSON)
			{
				string pValue = string.Empty;
				s.Serialize("latest_live_version", ref pValue);
				_latestLiveVersion = new VersionNumber(pValue);
			}
			else if (s.Mode == SimpleJSerializer.SerializationMode.SerializeToJSON)
			{
				string pValue2 = _latestLiveVersion.ToString();
				s.Serialize("latest_live_version", ref pValue2);
			}
			s.Serialize("ab_cohort", ref ABCohort).Serialize("crosspromo_url", ref CrossPromoURL).Serialize("crosspromo", ref CrossPromo)
				.Serialize("callhome_freq", ref _daysBetweenUpdates);
		}

		public void DeleteLocalData()
		{
			uFuga.Storage.persistent.Delete("liveSettings");
		}

		public void Clear()
		{
		}

		public void Preload(Action pCallback)
		{
			Clear();
			LoadFromStorage();
			if (new VersionNumber(_lastInstalledVersionString) < BuildVersion.Version || _daysBetweenUpdates == 0 || DateTime.Now.Day - DateTimeExtensions.FromUnixTime(_lastUpdatedTimestamp).Day >= _daysBetweenUpdates)
			{
				_loadedCallback = pCallback;
				LoadFromInternet();
			}
			else if (pCallback != null)
			{
				pCallback();
			}
		}

		private void LoadFromInternet()
		{
			uFuga.Instance.StartCoroutine(LoadFromInternet_CR());
		}

		private IEnumerator LoadFromInternet_CR()
		{
			WWWForm form = new WWWForm();
			form.AddField("abid", ABTestManager.Instance.DeviceIdentifier);
			form.AddField("s", 1);
			WWW www = new WWW(BuildSettings.Current.ServerSettingsURL, form);
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				Debug.LogWarning("LiveSettings error: " + www.error);
				yield break;
			}
			if (!string.IsNullOrEmpty(www.text))
			{
				try
				{
					string[] parts = www.text.Split(':');
					if (parts.Length != 2)
					{
						Debug.LogError("LiveSettings: Unexpected file format - missing separator");
						yield break;
					}
					string base64JsonString = parts[1];
					string signature = parts[0];
					string hmacSecret = BuildSettings.Current.ServerSettingsSecret + ABTestManager.Instance.DeviceIdentifier;
					string hashString = base64JsonString.HMACSHA1(hmacSecret);
					if (hashString != signature)
					{
						Debug.LogError("LiveSettings: Signature check failed");
						yield break;
					}
					string jsonString = Encoding.UTF8.GetString(Convert.FromBase64String(base64JsonString));
					JValue json = JValue.Decode(jsonString);
					if (json != null && json.IsObject)
					{
						Serialize(new SimpleJSerializer(json, SimpleJSerializer.SerializationMode.SerializeFromJSON));
						_lastUpdatedTimestamp = (int)DateTime.Now.ToUnixTime();
						_lastInstalledVersionString = BuildVersion.Version.ToString();
						SaveToStorage();
						if (SettingsUpdated != null)
						{
							SettingsUpdated();
						}
					}
					else
					{
						Debug.LogWarning("LiveSettings: Server response did not decode to anything useful");
					}
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Debug.LogWarning("LiveSettings: Couldn't decode server response: " + e.Message);
				}
			}
			else
			{
				Debug.LogWarning("LiveSettings: Did not get a useful response from " + BuildSettings.Current.ServerSettingsURL);
			}
			if (_loadedCallback != null)
			{
				_loadedCallback();
				_loadedCallback = null;
			}
		}

		private void LoadFromStorage()
		{
			string text = uFuga.Storage.persistent.GetString("liveSettings");
			if (!string.IsNullOrEmpty(text))
			{
				JValue jValue = JValue.Decode(text);
				if (jValue.IsObject)
				{
					Serialize(new SimpleJSerializer(jValue, SimpleJSerializer.SerializationMode.SerializeFromJSON));
				}
			}
		}

		private void SaveToStorage()
		{
			SimpleJSerializer simpleJSerializer = new SimpleJSerializer(JValue.EmptyObject, SimpleJSerializer.SerializationMode.SerializeToJSON);
			Serialize(simpleJSerializer);
			uFuga.Storage.persistent.Set("liveSettings", simpleJSerializer.Object.Encode());
		}
	}
}
