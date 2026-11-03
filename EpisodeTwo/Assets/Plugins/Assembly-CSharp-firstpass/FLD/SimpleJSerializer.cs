using System;
using System.Collections.Generic;
using FLD.JSON;

namespace FLD
{
	public class SimpleJSerializer
	{
		public enum SerializationMode
		{
			SerializeToJSON = 0,
			SerializeFromJSON = 1
		}

		protected SerializationMode _mode;

		protected JValue _target;

		public SerializationMode Mode
		{
			get
			{
				return _mode;
			}
		}

		public JValue Object
		{
			get
			{
				return _target;
			}
		}

		public SimpleJSerializer(JValue pTarget, SerializationMode pMode)
		{
			if (pTarget == null)
			{
				throw new ArgumentNullException("pTarget");
			}
			if (!pTarget.IsObject)
			{
				throw new ArgumentException("Target is not an object");
			}
			_target = pTarget;
			_mode = pMode;
		}

		public SimpleJSerializer GetChildSerializer(string pKey)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = JValue.EmptyObject;
			}
			else if (!_target[pKey].IsObject)
			{
				return new SimpleJSerializer(JValue.EmptyObject, _mode);
			}
			return new SimpleJSerializer(_target[pKey], _mode);
		}

		public SimpleJSerializer Serialize(string pKey, ref int pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = pValue;
			}
			else if (_target.ContainsKey(pKey))
			{
				pValue = _target[pKey];
			}
			return this;
		}

		public SimpleJSerializer Serialize(string pKey, ref float pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = pValue;
			}
			else if (_target.ContainsKey(pKey))
			{
				pValue = _target[pKey];
			}
			return this;
		}

		public SimpleJSerializer Serialize(string pKey, ref string pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = pValue;
			}
			else if (_target.ContainsKey(pKey))
			{
				pValue = _target[pKey];
			}
			return this;
		}

		public SimpleJSerializer Serialize(string pKey, ref DateTime pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = pValue.ToUnixTime();
			}
			else if (_target.ContainsKey(pKey))
			{
				pValue = _target[pKey].FromJsonUnixTime();
			}
			return this;
		}

		public SimpleJSerializer Serialize(string pKey, ref TimeSpan pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = pValue.TotalSeconds;
			}
			else if (_target.ContainsKey(pKey))
			{
				pValue = TimeSpan.FromSeconds(_target[pKey]);
			}
			return this;
		}

		public SimpleJSerializer Serialize(string pKey, ref bool pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				_target[pKey] = pValue;
			}
			else if (_target.ContainsKey(pKey))
			{
				pValue = _target[pKey];
			}
			return this;
		}

		public SimpleJSerializer Serialize<T, S>(string pKey, ref Dictionary<T, S> pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				JValue emptyObject = JValue.EmptyObject;
				foreach (KeyValuePair<T, S> item in pValue)
				{
					emptyObject.Add(item.Key.ToString(), item.Value.ToString());
				}
				_target[pKey] = emptyObject;
			}
			else
			{
				pValue = new Dictionary<T, S>();
				if (_target[pKey].IsObject)
				{
					foreach (KeyValuePair<string, JValue> keyValuePair in _target[pKey].KeyValuePairs)
					{
						pValue.Add((T)Convert.ChangeType(keyValuePair.Key, typeof(T)), (S)Convert.ChangeType((string)keyValuePair.Value, typeof(S)));
					}
				}
			}
			return this;
		}

		public SimpleJSerializer Serialize<T>(string pKey, ref List<T> pValue)
		{
			if (_mode == SerializationMode.SerializeToJSON)
			{
				JValue emptyArray = JValue.EmptyArray;
				for (int i = 0; i < pValue.Count; i++)
				{
					emptyArray.Add(new JValue(pValue[i]));
				}
				_target[pKey] = emptyArray;
			}
			else
			{
				pValue = new List<T>();
				if (_target[pKey].IsArray)
				{
					foreach (JValue item in _target[pKey])
					{
						pValue.Add((T)Convert.ChangeType((string)item, typeof(T)));
					}
				}
			}
			return this;
		}
	}
}
