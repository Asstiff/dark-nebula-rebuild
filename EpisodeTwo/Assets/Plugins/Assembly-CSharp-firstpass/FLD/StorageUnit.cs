using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public class StorageUnit
	{
		private string _key;

		private bool _persistent;

		private Dictionary<string, string> _variables;

		public string StorageKey
		{
			get
			{
				return _key;
			}
		}

		public StorageUnit(string pKey, bool pPersistent)
		{
			_key = pKey;
			_persistent = pPersistent;
			_variables = new Dictionary<string, string>();
			Load();
		}

		public void Set(string pID, object value, bool overwrite = true)
		{
			if (!Has(pID) || overwrite)
			{
				_variables[pID] = value.ToString();
			}
			if (_persistent)
			{
				SaveValue(pID);
			}
		}

		public void SetTyped(string pID, Vector3 value, bool overwrite = true)
		{
			Set(pID + "_x", value.x, overwrite);
			Set(pID + "_y", value.y, overwrite);
			Set(pID + "_z", value.z, overwrite);
		}

		public void SetTyped(string pID, Quaternion value, bool overwrite = true)
		{
			Set(pID + "_x", value.x, overwrite);
			Set(pID + "_y", value.y, overwrite);
			Set(pID + "_z", value.z, overwrite);
			Set(pID + "_w", value.w, overwrite);
		}

		public Quaternion GetQuaternion(string pID)
		{
			return new Quaternion(GetFloat(pID + "_x"), GetFloat(pID + "_y"), GetFloat(pID + "_z"), GetFloat(pID + "_w"));
		}

		public Vector3 GetVector3(string pID)
		{
			return new Vector3(GetFloat(pID + "_x"), GetFloat(pID + "_y"), GetFloat(pID + "_z"));
		}

		public string GetString(string pID)
		{
			if (Has(pID))
			{
				return _variables[pID];
			}
			return string.Empty;
		}

		public int GetInt(string pID)
		{
			if (!Has(pID))
			{
				return 0;
			}
			int result = 0;
			int.TryParse(_variables[pID], out result);
			return result;
		}

		public long GetLong(string pID)
		{
			if (!Has(pID))
			{
				return 0L;
			}
			long result = 0L;
			long.TryParse(_variables[pID], out result);
			return result;
		}

		public float GetFloat(string pID)
		{
			if (!Has(pID))
			{
				return 0f;
			}
			float result = 0f;
			float.TryParse(_variables[pID], out result);
			return result;
		}

		public bool GetBool(string pID)
		{
			if (!Has(pID))
			{
				return false;
			}
			bool result = false;
			bool.TryParse(_variables[pID], out result);
			return result;
		}

		public void Delete()
		{
			if (!_persistent)
			{
				return;
			}
			foreach (KeyValuePair<string, string> variable in _variables)
			{
				PlayerPrefs.DeleteKey(GetSaveKey(variable.Key));
			}
			_variables = new Dictionary<string, string>();
		}

		public void Delete(string pID)
		{
			if (Has(pID) && _persistent)
			{
				PlayerPrefs.DeleteKey(GetSaveKey(pID));
				_variables.Remove(pID);
			}
		}

		private void SaveValue(string pID)
		{
			if (!_persistent)
			{
				return;
			}
			PlayerPrefs.SetString(GetSaveKey(pID), _variables[pID]);
			string text = string.Empty;
			foreach (KeyValuePair<string, string> variable in _variables)
			{
				text = text + variable.Key + "%";
			}
			PlayerPrefs.SetString("_farrokh_allkeys_" + _key, text);
		}

		private void Load()
		{
			if (!_persistent)
			{
				return;
			}
			string key = "_farrokh_allkeys_" + _key;
			if (!PlayerPrefs.HasKey(key))
			{
				return;
			}
			string[] array = PlayerPrefs.GetString(key).Split('%');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string saveKey = GetSaveKey(text);
				if (PlayerPrefs.HasKey(saveKey))
				{
					_variables.Add(text, PlayerPrefs.GetString(saveKey));
				}
			}
		}

		public bool Has(string pID)
		{
			return _variables.ContainsKey(pID);
		}

		private string GetSaveKey(string pID)
		{
			return "_farrokh_" + _key + "_" + pID;
		}

		public Dictionary<string, string> GetAll()
		{
			return _variables;
		}
	}
}
