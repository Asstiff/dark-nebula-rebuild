using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FLD
{
	public class PersistentStorage
	{
		private string secretKey = "herpderp";

		private string _storageKey;

		private bool _keepBetweenSessions;

		private Dictionary<string, string> _variables;

		public string StorageKey
		{
			get
			{
				return _storageKey;
			}
		}

		public PersistentStorage(string pStorageKey)
			: this(pStorageKey, false)
		{
		}

		public PersistentStorage(string pStorageKey, bool pKeepBetweenSessions)
		{
			if (secretKey == "secret")
			{
				Debug.LogWarning("Please change the secret key for the persistent storage system (see PersistentStorage.cs)!");
			}
			_storageKey = pStorageKey;
			_keepBetweenSessions = pKeepBetweenSessions;
			_variables = new Dictionary<string, string>();
			Load();
			uFuga.ApplicationPausedOrResumed += OnApplicationPause;
			uFuga.ApplicationQuit += OnApplicationQuit;
		}

		public bool Has(string id)
		{
			return _variables.ContainsKey(id);
		}

		public void Set(string id, object value, bool overwrite)
		{
			if (!Has(id) || overwrite)
			{
				_variables[id] = value.ToString();
			}
			if (_keepBetweenSessions)
			{
				SaveValue(id);
			}
		}

		public void Set(string id, object value)
		{
			Set(id, value, true);
		}

		public string GetString(string id)
		{
			if (Has(id))
			{
				return _variables[id];
			}
			return string.Empty;
		}

		public int GetInt(string id)
		{
			string text = GetString(id);
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public float GetFloat(string id)
		{
			string text = GetString(id);
			if (string.IsNullOrEmpty(text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		public bool GetBool(string id)
		{
			return GetString(id).ToLower() == "true";
		}

		public void Delete()
		{
			if (_keepBetweenSessions)
			{
				foreach (KeyValuePair<string, string> variable in _variables)
				{
					PlayerPrefs.DeleteKey(GetSaveKey(variable.Key));
				}
			}
			_variables = new Dictionary<string, string>();
		}

		public void Delete(string id)
		{
			if (Has(id))
			{
				if (_keepBetweenSessions)
				{
					PlayerPrefs.DeleteKey(id);
				}
				_variables.Remove(id);
			}
		}

		public void Save()
		{
			if (!_keepBetweenSessions)
			{
				return;
			}
			string text = string.Empty;
			foreach (KeyValuePair<string, string> variable in _variables)
			{
				SaveValue(variable.Key);
				text = text + variable.Key + "%";
			}
			PlayerPrefs.SetString("_fps_allkeys_" + _storageKey, text);
		}

		private void SaveValue(string pKey)
		{
			string pStringToHash = string.Format("{0}{1}{2}", secretKey, pKey, _variables[pKey]);
			string text = pStringToHash.MD5Sum();
			PlayerPrefs.SetString(GetSaveKey(pKey), text + "|" + _variables[pKey]);
		}

		private void Load()
		{
			if (!_keepBetweenSessions)
			{
				return;
			}
			string key = "_fps_allkeys_" + _storageKey;
			if (!PlayerPrefs.HasKey(key))
			{
				return;
			}
			string text = PlayerPrefs.GetString(key);
			string[] array = text.Split('%');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (PlayerPrefs.HasKey(GetSaveKey(text2)))
				{
					string text3 = PlayerPrefs.GetString(GetSaveKey(text2));
					string text4;
					string text5;
					string text6;
					string text7;
					if (text3.IndexOf('|') > 0)
					{
						text4 = text3.Split('|')[0];
						text5 = text3.Split('|')[1];
						string pStringToHash = string.Format("{0}{1}{2}", secretKey, text2, text5);
						text6 = pStringToHash.MD5Sum();
						text7 = FUBARLegacyHexString(pStringToHash.MD5SumBytes());
					}
					else
					{
						text4 = string.Empty;
						text7 = string.Empty;
						text6 = string.Empty;
						text5 = text3;
					}
					if (text6 == text4)
					{
						_variables.Add(text2, text5);
					}
					else if (text7 == text4)
					{
						_variables.Add(text2, text5);
					}
					else
					{
						Debug.LogWarning("Saved and generated hash did not match for: " + text2 + ". Could be the result of cheat");
					}
				}
			}
		}

		private static string FUBARLegacyHexString(byte[] pBytes)
		{
			char[] array = new char[16]
			{
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
				'a', 'b', 'c', 'd', 'e', 'f'
			};
			StringBuilder stringBuilder = new StringBuilder(pBytes.Length * 2);
			foreach (byte b in pBytes)
			{
				stringBuilder.Append('0');
				stringBuilder.Append(array[b & 0xF]);
			}
			return stringBuilder.ToString();
		}

		private string GetSaveKey(string id)
		{
			return "fps_" + _storageKey + "_" + id;
		}

		public Dictionary<string, string> GetAll()
		{
			return _variables;
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (isPaused)
			{
				Save();
			}
		}

		private void OnApplicationQuit()
		{
			Save();
		}
	}
}
