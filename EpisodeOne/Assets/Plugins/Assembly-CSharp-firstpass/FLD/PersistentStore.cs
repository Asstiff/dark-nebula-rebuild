using System.Collections.Generic;
using System.IO;
using FLD.JSON;
using UnityEngine;

namespace FLD
{
	public class PersistentStore
	{
		private enum StoreValueType : byte
		{
			String = 1,
			Integer = 2,
			Float = 3,
			Bool = 4
		}

		private static readonly byte[] ScrambleKey = new byte[8] { 89, 238, 113, 21, 154, 42, 140, 20 };

		private static readonly byte[] MagicBytes = new byte[2] { 36, 104 };

		private static Dictionary<string, PersistentStore> _cachedStores = new Dictionary<string, PersistentStore>();

		private Dictionary<string, object> _values = new Dictionary<string, object>();

		private string _storeId;

		private bool _isDirty;

		private string PlayerPrefsKey
		{
			get
			{
				return "uFuga2.PStore." + _storeId;
			}
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return _values.Keys;
			}
		}

		private PersistentStore(string pStoreId)
		{
			_storeId = pStoreId;
			Load();
			if (!Application.isEditor || Application.isPlaying)
			{
				Timer.ApplicationPaused += Save;
				Timer.ApplicationQuit += Save;
			}
		}

		public static PersistentStore GetInstance(string pId)
		{
			PersistentStore value;
			if (!_cachedStores.TryGetValue(pId, out value))
			{
				value = new PersistentStore(pId);
				_cachedStores.Add(pId, value);
			}
			return value;
		}

		public void Save()
		{
			if (!_isDirty)
			{
				return;
			}
			_isDirty = false;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Seek(16, SeekOrigin.Begin);
					binaryWriter.Write(MagicBytes);
					SaveTo(binaryWriter);
					byte[] buffer = memoryStream.GetBuffer().MD5Bytes(16, (int)memoryStream.Length - 16);
					memoryStream.Seek(0L, SeekOrigin.Begin);
					binaryWriter.Write(buffer);
				}
				byte[] pData = memoryStream.ToArray();
				Scramble(pData);
				SaveToNativeStorage(PlayerPrefsKey, pData);
			}
		}

		public void Load()
		{
			_values.Clear();
			byte[] array = LoadFromNativeStorage(PlayerPrefsKey);
			if (array == null)
			{
				return;
			}
			if (array.Length < 22)
			{
				Debug.LogError("Stream too short: Pstore " + _storeId + " is corrupted");
				return;
			}
			Unscramble(array);
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					byte[] pLeft = binaryReader.ReadBytes(16);
					byte[] pRight = array.MD5Bytes(16, (int)memoryStream.Length - 16);
					if (!pLeft.ArrayEquals(pRight))
					{
						Debug.LogError("MD5 mismatch: Pstore " + _storeId + " is corrupted");
						return;
					}
					byte[] pLeft2 = binaryReader.ReadBytes(MagicBytes.Length);
					if (!pLeft2.ArrayEquals(MagicBytes))
					{
						Debug.LogError("Magic byte mismatch: Pstore " + _storeId + " is corrupted");
					}
					else
					{
						LoadFrom(binaryReader);
					}
				}
			}
		}

		private void SaveTo(BinaryWriter pWriter)
		{
			pWriter.Write(_values.Count);
			foreach (KeyValuePair<string, object> value in _values)
			{
				pWriter.Write(value.Key);
				if (value.Value is string)
				{
					pWriter.Write((byte)1);
					pWriter.Write((string)value.Value);
				}
				else if (value.Value is int)
				{
					pWriter.Write((byte)2);
					pWriter.Write((int)value.Value);
				}
				else if (value.Value is float)
				{
					pWriter.Write((byte)3);
					pWriter.Write((float)value.Value);
				}
				else if (value.Value is bool)
				{
					pWriter.Write((byte)4);
					pWriter.Write((bool)value.Value);
				}
			}
		}

		private void LoadFrom(BinaryReader pReader)
		{
			int num = pReader.ReadInt32();
			while (num-- > 0)
			{
				string key = pReader.ReadString();
				object value = null;
				switch ((StoreValueType)pReader.ReadByte())
				{
				case StoreValueType.String:
					value = pReader.ReadString();
					break;
				case StoreValueType.Integer:
					value = pReader.ReadInt32();
					break;
				case StoreValueType.Float:
					value = pReader.ReadSingle();
					break;
				case StoreValueType.Bool:
					value = pReader.ReadBoolean();
					break;
				}
				_values.Add(key, value);
			}
		}

		private void Scramble(byte[] pData)
		{
			pData[0] ^= (byte)((pData.Length & 0xFF) ^ ScrambleKey[0]);
			for (int i = 1; i < pData.Length; i++)
			{
				pData[i] ^= (byte)(pData[i - 1] ^ ScrambleKey[i % ScrambleKey.Length]);
			}
		}

		private void Unscramble(byte[] pData)
		{
			for (int num = pData.Length - 1; num >= 1; num--)
			{
				pData[num] ^= (byte)(pData[num - 1] ^ ScrambleKey[num % ScrambleKey.Length]);
			}
			pData[0] ^= (byte)((pData.Length & 0xFF) ^ ScrambleKey[0]);
		}

		public void Clear()
		{
			_values.Clear();
		}

		public void Remove(string pKey)
		{
			_values.Remove(pKey);
			_isDirty = true;
		}

		public object Get(string pKey)
		{
			object value;
			_values.TryGetValue(pKey, out value);
			return value;
		}

		public string GetString(string pKey)
		{
			object obj = Get(pKey);
			return (obj as string) ?? string.Empty;
		}

		public int GetInt(string pKey)
		{
			object obj = Get(pKey);
			return (obj is int) ? ((int)obj) : 0;
		}

		public float GetFloat(string pKey)
		{
			object obj = Get(pKey);
			return (!(obj is float)) ? 0f : ((float)obj);
		}

		public bool GetBool(string pKey)
		{
			object obj = Get(pKey);
			return obj is bool && (bool)obj;
		}

		public JValue GetJson(string pKey)
		{
			string text = GetString(pKey);
			if (!string.IsNullOrEmpty(text))
			{
				return JValue.Parse(text);
			}
			return null;
		}

		public void Set(string pKey, string pValue)
		{
			_values[pKey] = pValue;
			_isDirty = true;
		}

		public void Set(string pKey, int pValue)
		{
			_values[pKey] = pValue;
			_isDirty = true;
		}

		public void Set(string pKey, float pValue)
		{
			_values[pKey] = pValue;
			_isDirty = true;
		}

		public void Set(string pKey, bool pValue)
		{
			_values[pKey] = pValue;
			_isDirty = true;
		}

		public void Set(string pKey, JValue pValue)
		{
			Set(pKey, pValue.Format());
			_isDirty = true;
		}

		private static byte[] LoadFromNativeStorage(string pKey)
		{
			if (PlayerPrefs.HasKey(pKey))
			{
				string text = PlayerPrefs.GetString(pKey);
				if (!string.IsNullOrEmpty(text))
				{
					return text.Base64Decode();
				}
			}
			return null;
		}

		private static void SaveToNativeStorage(string pKey, byte[] pData)
		{
			PlayerPrefs.SetString(pKey, pData.Base64Encode());
			PlayerPrefs.Save();
		}
	}
}
