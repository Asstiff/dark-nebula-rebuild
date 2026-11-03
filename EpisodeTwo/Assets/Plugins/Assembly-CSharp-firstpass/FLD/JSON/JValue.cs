using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FLD.JSON
{
	public class JValue
	{
		public enum EncodeMode
		{
			Compact = 0,
			Verbose = 1
		}

		public enum Type
		{
			String = 0,
			Number = 1,
			Object = 2,
			Array = 3,
			True = 4,
			False = 5,
			Null = 6
		}

		private Type _type;

		private object _value;

		public static JValue EmptyObject
		{
			get
			{
				return new JValue(new Dictionary<string, JValue>());
			}
		}

		public static JValue EmptyArray
		{
			get
			{
				return new JValue(new List<JValue>());
			}
		}

		public static JValue NullValue
		{
			get
			{
				return new JValue();
			}
		}

		public Type ValueType
		{
			get
			{
				return _type;
			}
		}

		public bool IsObject
		{
			get
			{
				return _type == Type.Object;
			}
		}

		public bool IsArray
		{
			get
			{
				return _type == Type.Array;
			}
		}

		public bool IsString
		{
			get
			{
				return _type == Type.String;
			}
		}

		public bool IsNull
		{
			get
			{
				return _type == Type.Null;
			}
		}

		public bool IsNumber
		{
			get
			{
				return _type == Type.Number;
			}
		}

		public bool IsBoolean
		{
			get
			{
				return _type == Type.True || _type == Type.False;
			}
		}

		public Dictionary<string, JValue> AsDictionary
		{
			get
			{
				if (!IsObject)
				{
					throw new InvalidOperationException("Value is not an object");
				}
				return (Dictionary<string, JValue>)_value;
			}
		}

		public List<JValue> AsList
		{
			get
			{
				if (!IsArray)
				{
					throw new InvalidOperationException("Value is not an array");
				}
				return (List<JValue>)_value;
			}
		}

		public bool BoolValue
		{
			get
			{
				switch (_type)
				{
				case Type.False:
				case Type.Null:
					return false;
				case Type.Number:
					return (int)this > 0;
				default:
					return true;
				}
			}
		}

		public object NativeValue
		{
			get
			{
				if (IsObject || IsArray)
				{
					throw new InvalidOperationException("Cannot convert array or object to a native type");
				}
				if (_type == Type.False || _type == Type.True)
				{
					return (bool)this;
				}
				return _value;
			}
		}

		public JValue this[string pKey]
		{
			get
			{
				if (!IsObject)
				{
					throw new InvalidOperationException("Value is not an object");
				}
				Dictionary<string, JValue> asDictionary = AsDictionary;
				if (asDictionary.ContainsKey(pKey))
				{
					return asDictionary[pKey];
				}
				return NullValue;
			}
			set
			{
				if (!IsObject)
				{
					throw new InvalidOperationException("Value is not an object");
				}
				AsDictionary[pKey] = value;
			}
		}

		public IEnumerable<KeyValuePair<string, JValue>> KeyValuePairs
		{
			get
			{
				return AsDictionary;
			}
		}

		public bool IsEnumerable
		{
			get
			{
				return IsObject || IsArray;
			}
		}

		public JValue(string pString)
		{
			_value = pString;
			_type = Type.String;
		}

		public JValue(float pNumber)
		{
			_value = (double)pNumber;
			_type = Type.Number;
		}

		public JValue(double pNumber)
		{
			_value = pNumber;
			_type = Type.Number;
		}

		public JValue(Dictionary<string, JValue> pObject)
		{
			_value = pObject;
			_type = Type.Object;
		}

		public JValue(List<JValue> pObject)
		{
			_value = pObject;
			_type = Type.Array;
		}

		public JValue(bool pValue)
		{
			_type = ((!pValue) ? Type.False : Type.True);
		}

		public JValue()
		{
			_type = Type.Null;
		}

		public JValue(object pObject)
		{
			_value = pObject;
			if (pObject == null)
			{
				_type = Type.Null;
				return;
			}
			if (pObject.GetType() == typeof(int) || pObject.GetType() == typeof(float) || pObject.GetType() == typeof(double))
			{
				_type = Type.Number;
				_value = Convert.ToDouble(_value);
				return;
			}
			if (pObject.GetType() == typeof(bool))
			{
				_type = ((!(bool)pObject) ? Type.False : Type.True);
				_value = null;
				return;
			}
			if (pObject.GetType() == typeof(string))
			{
				_type = Type.String;
				return;
			}
			throw new ArgumentException("Invalid object type");
		}

		public static JValue Decode(string pString)
		{
			return DecodeValue(new TextTokenizer(pString));
		}

		protected static void ThrowError(TextTokenizer pTokenizer, string pMessage)
		{
			throw new FormatException(pMessage + " near \"" + pTokenizer.NearContext + "\"");
		}

		protected static JValue DecodeValue(TextTokenizer pTokenizer)
		{
			string text = pTokenizer.TakeToken();
			switch (text)
			{
			case "true":
				return new JValue(true);
			case "false":
				return new JValue(false);
			case "null":
				return new JValue();
			default:
				if (!text.IsNumeric())
				{
					switch (text)
					{
					case "\"":
						return new JValue(DecodeString(pTokenizer));
					case "{":
					{
						Dictionary<string, JValue> dictionary = new Dictionary<string, JValue>();
						if (pTokenizer.PeekToken() == "}")
						{
							pTokenizer.TakeToken();
						}
						else
						{
							while (true)
							{
								pTokenizer.ExpectToken("\"");
								string key = DecodeString(pTokenizer);
								pTokenizer.ExpectToken(":");
								JValue value = DecodeValue(pTokenizer);
								dictionary.Add(key, value);
								char c2 = pTokenizer.TakePunctuationToken();
								switch (c2)
								{
								case ',':
									continue;
								default:
									goto IL_01c3;
								case '}':
									break;
								}
								break;
								IL_01c3:
								ThrowError(pTokenizer, "Expected comma or }, got " + c2);
							}
						}
						return new JValue(dictionary);
					}
					case "[":
					{
						List<JValue> list = new List<JValue>();
						if (pTokenizer.PeekToken() == "]")
						{
							pTokenizer.TakeToken();
						}
						else
						{
							while (true)
							{
								JValue item = DecodeValue(pTokenizer);
								list.Add(item);
								char c = pTokenizer.TakePunctuationToken();
								switch (c)
								{
								case ',':
									continue;
								default:
									goto IL_024e;
								case ']':
									break;
								}
								break;
								IL_024e:
								ThrowError(pTokenizer, "Expected comma or ], got " + c);
							}
						}
						return new JValue(list);
					}
					default:
						ThrowError(pTokenizer, "Couldn't decode value");
						return null;
					}
				}
				goto case "-";
			case "-":
			{
				string text2 = text;
				if (text == "-")
				{
					text2 += pTokenizer.TakeToken();
				}
				if (pTokenizer.PeekToken() == ".")
				{
					text2 += pTokenizer.TakeToken();
					text2 += pTokenizer.TakeToken();
				}
				if (pTokenizer.PeekToken().ToLower() == "e")
				{
					text2 += pTokenizer.TakeToken();
					text = pTokenizer.TakeToken();
					text2 += text;
					if (text == "+" || text == "-")
					{
						text2 += pTokenizer.TakeToken();
					}
				}
				return new JValue(double.Parse(text2));
			}
			}
		}

		protected static string DecodeString(TextTokenizer pTokenizer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				char c = pTokenizer.TakeCharacter();
				switch (c)
				{
				case '"':
					return stringBuilder.ToString();
				case '\\':
					c = pTokenizer.TakeCharacter();
					switch (c)
					{
					case '"':
					case '/':
					case '\\':
						stringBuilder.Append(c);
						break;
					case 'b':
						stringBuilder.Append('\b');
						break;
					case 'n':
						stringBuilder.Append('\n');
						break;
					case 'r':
						stringBuilder.Append('\r');
						break;
					case 'f':
						stringBuilder.Append('\f');
						break;
					case 't':
						stringBuilder.Append('\t');
						break;
					case 'u':
					{
						string s = pTokenizer.TakeCharacters(4);
						stringBuilder.Append(char.ConvertFromUtf32(int.Parse(s)));
						break;
					}
					default:
						ThrowError(pTokenizer, "Bad escape character");
						break;
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
		}

		public string Encode(EncodeMode pMode = EncodeMode.Compact)
		{
			return Encode(this, pMode);
		}

		public static string Encode(JValue pValue, EncodeMode pMode = EncodeMode.Compact)
		{
			SourceStringBuilder sourceStringBuilder = new SourceStringBuilder();
			EncodeInto(sourceStringBuilder, pValue, pMode);
			return sourceStringBuilder.ToString();
		}

		protected static void EncodeInto(SourceStringBuilder pStringBuilder, JValue pValue, EncodeMode pMode)
		{
			bool flag = true;
			switch (pValue._type)
			{
			case Type.Null:
				pStringBuilder.Append("null");
				break;
			case Type.True:
			case Type.False:
				pStringBuilder.Append(pValue.ToString());
				break;
			case Type.Number:
			{
				double num = pValue;
				if (double.IsNaN(num) || double.IsInfinity(num))
				{
					pStringBuilder.Append("0");
					Debug.LogError("Number JValue is NaN or INF");
				}
				else
				{
					pStringBuilder.Append(num);
				}
				break;
			}
			case Type.String:
				EncodeString(pValue.ToString(), pStringBuilder);
				break;
			case Type.Array:
				pStringBuilder.Append("[");
				flag = true;
				foreach (JValue item in pValue)
				{
					if (!flag)
					{
						pStringBuilder.Append(',');
					}
					flag = false;
					EncodeInto(pStringBuilder, item, pMode);
				}
				pStringBuilder.Append(']');
				break;
			case Type.Object:
				pStringBuilder.Append('{');
				if (pMode == EncodeMode.Verbose)
				{
					pStringBuilder.AppendLine();
					pStringBuilder.Indent();
				}
				flag = true;
				foreach (KeyValuePair<string, JValue> keyValuePair in pValue.KeyValuePairs)
				{
					if (!flag)
					{
						pStringBuilder.Append(',');
						if (pMode == EncodeMode.Verbose)
						{
							pStringBuilder.AppendLine();
						}
					}
					flag = false;
					EncodeString(keyValuePair.Key, pStringBuilder);
					pStringBuilder.Append(':');
					EncodeInto(pStringBuilder, keyValuePair.Value, pMode);
				}
				if (pMode == EncodeMode.Verbose)
				{
					pStringBuilder.AppendLine();
					pStringBuilder.Unindent();
				}
				pStringBuilder.Append('}');
				break;
			}
		}

		protected static void EncodeString(string pString, SourceStringBuilder pStringBuilder)
		{
			pStringBuilder.Append('"');
			foreach (char c in pString)
			{
				switch (c)
				{
				case '\b':
					pStringBuilder.Append("\\b");
					break;
				case '\n':
					pStringBuilder.Append("\\n");
					break;
				case '\t':
					pStringBuilder.Append("\\t");
					break;
				case '\r':
					pStringBuilder.Append("\\r");
					break;
				case '\f':
					pStringBuilder.Append("\\f");
					break;
				case '\\':
					pStringBuilder.Append("\\\\");
					break;
				case '"':
					pStringBuilder.Append("\\\"");
					break;
				default:
					pStringBuilder.Append(c);
					break;
				}
			}
			pStringBuilder.Append('"');
		}

		public static JValue FromEnumerable(IEnumerable<int> pValue)
		{
			JValue emptyArray = EmptyArray;
			emptyArray.Add(pValue);
			return emptyArray;
		}

		public static JValue FromEnumerable(IEnumerable<float> pValue)
		{
			JValue emptyArray = EmptyArray;
			emptyArray.Add(pValue);
			return emptyArray;
		}

		public static JValue FromEnumerable(IEnumerable<string> pValue)
		{
			JValue emptyArray = EmptyArray;
			emptyArray.Add(pValue);
			return emptyArray;
		}

		public static JValue FromEnumerable(IEnumerable<bool> pValue)
		{
			JValue emptyArray = EmptyArray;
			emptyArray.Add(pValue);
			return emptyArray;
		}

		public override string ToString()
		{
			return this;
		}

		public void Add(JValue pValue)
		{
			if (!IsArray)
			{
				throw new InvalidOperationException("Value is not an array");
			}
			AsList.Add(pValue);
		}

		protected void Add(IEnumerable<JValue> pEnumerable)
		{
			if (!IsArray)
			{
				throw new InvalidOperationException("Value is not an array");
			}
			foreach (JValue item in pEnumerable)
			{
				AsList.Add(item);
			}
		}

		protected void Add(IEnumerable<int> pEnumerable)
		{
			if (!IsArray)
			{
				throw new InvalidOperationException("Value is not an array");
			}
			foreach (int item in pEnumerable)
			{
				AsList.Add(item);
			}
		}

		protected void Add(IEnumerable<float> pEnumerable)
		{
			if (!IsArray)
			{
				throw new InvalidOperationException("Value is not an array");
			}
			foreach (float item in pEnumerable)
			{
				float num = item;
				AsList.Add(num);
			}
		}

		protected void Add(IEnumerable<string> pEnumerable)
		{
			if (!IsArray)
			{
				throw new InvalidOperationException("Value is not an array");
			}
			foreach (string item in pEnumerable)
			{
				AsList.Add(item);
			}
		}

		protected void Add(IEnumerable<bool> pEnumerable)
		{
			if (!IsArray)
			{
				throw new InvalidOperationException("Value is not an array");
			}
			foreach (bool item in pEnumerable)
			{
				AsList.Add(item);
			}
		}

		public void Add(string pKey, JValue pValue)
		{
			if (!IsObject)
			{
				throw new InvalidOperationException("Value is not an object");
			}
			AsDictionary.Add(pKey, pValue);
		}

		public bool ContainsKey(string pKey)
		{
			if (IsObject)
			{
				return AsDictionary.ContainsKey(pKey);
			}
			return false;
		}

		public IEnumerator<JValue> GetEnumerator()
		{
			if (IsObject)
			{
				return AsDictionary.Values.GetEnumerator();
			}
			if (IsArray)
			{
				return AsList.GetEnumerator();
			}
			throw new InvalidOperationException("Value is not enumerable");
		}

		public static implicit operator JValue(int pValue)
		{
			return new JValue(pValue);
		}

		public static implicit operator JValue(float pValue)
		{
			return new JValue(pValue);
		}

		public static implicit operator JValue(double pValue)
		{
			return new JValue(pValue);
		}

		public static implicit operator JValue(string pValue)
		{
			return new JValue(pValue);
		}

		public static implicit operator JValue(bool pValue)
		{
			return new JValue(pValue);
		}

		public static implicit operator int(JValue pValue)
		{
			switch (pValue._type)
			{
			case Type.Number:
				try
				{
					return Convert.ToInt32(pValue._value);
				}
				catch (OverflowException)
				{
					Debug.LogError(string.Concat("Failed to convert ", pValue._value, " to integer"));
					return 0;
				}
			case Type.True:
				return 1;
			default:
				return 0;
			}
		}

		public static implicit operator double(JValue pValue)
		{
			switch (pValue._type)
			{
			case Type.Number:
				return Convert.ToDouble(pValue._value);
			case Type.True:
				return 1.0;
			default:
				return 0.0;
			}
		}

		public static implicit operator float(JValue pValue)
		{
			double num = pValue;
			return (float)num;
		}

		public static implicit operator string(JValue pValue)
		{
			switch (pValue._type)
			{
			case Type.True:
				return "true";
			case Type.False:
				return "false";
			case Type.Null:
				return string.Empty;
			case Type.Array:
				return "<Array>";
			case Type.Object:
				return "<Object>";
			default:
				if (pValue._value != null)
				{
					return pValue._value.ToString();
				}
				return string.Empty;
			}
		}

		public static implicit operator bool(JValue pValue)
		{
			return pValue.BoolValue;
		}
	}
}
