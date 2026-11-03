using Debug = UnityEngine.Debug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace FLD.JSON
{
	public class JValue : IEnumerable<JValue>, IEnumerable
	{
		[Flags]
		public enum ParseOptions
		{
			SuppressErrorMessages = 1,
			Default = 0
		}

		public enum FormatMode
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

		[CompilerGenerated]
		private sealed class _003C_003Ec__Iterator5 : IDisposable, IEnumerator, IEnumerable<KeyValuePair<string, JValue>>, IEnumerator<KeyValuePair<string, JValue>>, IEnumerable
		{
			internal Dictionary<string, JValue>.Enumerator _003C_0024s_73_003E__0;

			internal KeyValuePair<string, JValue> _003Ckvp_003E__1;

			internal int _0024PC;

			internal KeyValuePair<string, JValue> _0024current;

			internal JValue _003C_003Ef__this;

			KeyValuePair<string, JValue> IEnumerator<KeyValuePair<string, JValue>>.Current
			{
				[DebuggerHidden]
				get
				{
					return System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CFLD_002EJSON_002EJValue_003E_003E_002Eget_Current();
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _0024current;
				}
			}

			[DebuggerHidden]
			private KeyValuePair<string, JValue> System_002ECollections_002EGeneric_002EIEnumerator_003CSystem_002ECollections_002EGeneric_002EKeyValuePair_003Cstring_002CFLD_002EJSON_002EJValue_003E_003E_002Eget_Current()
			{
				return _0024current;
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<KeyValuePair<string, JValue>>)this).GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<KeyValuePair<string, JValue>> IEnumerable<KeyValuePair<string, JValue>>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref _0024PC, 0, -2) == -2)
				{
					return this;
				}
				return new _003C_003Ec__Iterator5
				{
					_003C_003Ef__this = _003C_003Ef__this
				};
			}

			public bool MoveNext()
			{
				uint num = (uint)_0024PC;
				_0024PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					if (_003C_003Ef__this.IsObject)
					{
						_003C_0024s_73_003E__0 = _003C_003Ef__this.AsDictionary.GetEnumerator();
						num = 4294967293u;
						goto case 1u;
					}
					goto IL_00b2;
				case 1u:
					try
					{
						switch (num)
						{
						default:
							if (_003C_0024s_73_003E__0.MoveNext())
							{
								_003Ckvp_003E__1 = _003C_0024s_73_003E__0.Current;
								_0024current = _003Ckvp_003E__1;
								_0024PC = 1;
								flag = true;
								goto IL_00bb;
							}
							break;
						}
					}
					finally
					{
						if (!flag)
						{
							((IDisposable)_003C_0024s_73_003E__0).Dispose();
						}
					}
					goto IL_00b2;
				default:
					{
						return false;
					}
					IL_00bb:
					return true;
					IL_00b2:
					_0024PC = -1;
					goto default;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)_0024PC;
				_0024PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
						break;
					}
					finally
					{
						((IDisposable)_003C_0024s_73_003E__0).Dispose();
					}
				case 0u:
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}
		}

		private Type _type;

		private object _value;

		public static JValue Null
		{
			get
			{
				JValue jValue = new JValue();
				jValue._type = Type.Null;
				return jValue;
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

		public int[] AsIntArray
		{
			get
			{
				if (!IsArray)
				{
					return new int[0];
				}
				List<JValue> asList = AsList;
				int[] array = new int[asList.Count];
				for (int i = 0; i < asList.Count; i++)
				{
					int num = i;
					int? intValue = asList[i].IntValue;
					array[num] = (intValue.HasValue ? intValue.Value : 0);
				}
				return array;
			}
		}

		public string[] AsStringArray
		{
			get
			{
				if (!IsArray)
				{
					return new string[0];
				}
				List<JValue> asList = AsList;
				string[] array = new string[asList.Count];
				for (int i = 0; i < asList.Count; i++)
				{
					array[i] = asList[i].StringValue;
				}
				return array;
			}
		}

		public bool? BoolValue
		{
			get
			{
				if (_type == Type.True)
				{
					return true;
				}
				if (_type == Type.False)
				{
					return false;
				}
				return null;
			}
		}

		public int? IntValue
		{
			get
			{
				if (_type == Type.Number)
				{
					try
					{
						return Convert.ToInt32(_value);
					}
					catch (OverflowException)
					{
						return null;
					}
				}
				return null;
			}
		}

		public double? DoubleValue
		{
			get
			{
				if (_type == Type.Number)
				{
					return Convert.ToDouble(_value);
				}
				return null;
			}
		}

		public float? FloatValue
		{
			get
			{
				if (_type == Type.Number)
				{
					return (float)Convert.ToDouble(_value);
				}
				return null;
			}
		}

		public string StringValue
		{
			get
			{
				if (_type == Type.String)
				{
					return (string)_value;
				}
				return null;
			}
		}

		public object NativeObjectValue
		{
			get
			{
				if (IsObject || IsArray)
				{
					throw new InvalidOperationException("Cannot convert array or object to a native type");
				}
				if (_type == Type.False || _type == Type.True)
				{
					return BoolValue.Value;
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
				return Null;
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

		public IEnumerable<KeyValuePair<string, JValue>> Fields
		{
			get
			{
				//yield-return decompiler failed: Could not find currentField
				_003C_003Ec__Iterator5 obj = new _003C_003Ec__Iterator5();
				obj._003C_003Ef__this = this;
				obj._0024PC = -2;
				return obj;
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

		public JValue(IEnumerable<string> pList)
		{
			_value = new List<JValue>();
			_type = Type.Array;
			Add(pList);
		}

		public JValue(IEnumerable<int> pList)
		{
			_value = new List<JValue>();
			_type = Type.Array;
			Add(pList);
		}

		public JValue(IEnumerable<float> pList)
		{
			_value = new List<JValue>();
			_type = Type.Array;
			Add(pList);
		}

		public JValue()
			: this(Type.Object)
		{
		}

		public JValue(Type pType)
		{
			_type = pType;
			switch (_type)
			{
			case Type.Object:
				_value = new Dictionary<string, JValue>();
				break;
			case Type.Array:
				_value = new List<JValue>();
				break;
			case Type.Number:
				_value = 0.0;
				break;
			case Type.String:
				_value = string.Empty;
				break;
			}
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

		IEnumerator<JValue> IEnumerable<JValue>.GetEnumerator()
		{
			if (IsObject)
			{
				return AsDictionary.Values.GetEnumerator();
			}
			if (IsArray)
			{
				return AsList.GetEnumerator();
			}
			return new List<JValue>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (IsObject)
			{
				return AsDictionary.Values.GetEnumerator();
			}
			if (IsArray)
			{
				return AsList.GetEnumerator();
			}
			return new List<JValue>().GetEnumerator();
		}

		public static JValue ParseFile(string pPath, ParseOptions pOptions = ParseOptions.Default)
		{
			if (!File.Exists(pPath))
			{
				return null;
			}
			using (FileStream pStream = File.Open(pPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Parse(pStream, pOptions);
			}
		}

		public static JValue Parse(string pString, ParseOptions pOptions = ParseOptions.Default)
		{
			return ParseValue(new TokenStream(pString), pOptions);
		}

		public static JValue Parse(Stream pStream, ParseOptions pOptions = ParseOptions.Default)
		{
			return ParseValue(new TokenStream(pStream), pOptions);
		}

		protected static void ReportError(TokenStream pTokenizer, ParseOptions pOptions, string pMessage)
		{
			if ((pOptions & ParseOptions.SuppressErrorMessages) == 0)
			{
				Debug.LogError(pMessage + " near \"" + pTokenizer.NearContext + "\"");
			}
		}

		protected static JValue ParseValue(TokenStream pTokenizer, ParseOptions pOptions)
		{
			string text = pTokenizer.Read();
			if (text == null)
			{
				ReportError(pTokenizer, pOptions, "Unexpected end of stream");
				return null;
			}
			switch (text)
			{
			case "true":
				return new JValue(true);
			case "false":
				return new JValue(false);
			case "null":
				return Null;
			default:
				if (!text.IsNumeric())
				{
					switch (text)
					{
					case "\"":
					{
						string text3 = ParseString(pTokenizer, pOptions);
						if (text3 == null)
						{
							return null;
						}
						return new JValue(text3);
					}
					case "{":
					{
						Dictionary<string, JValue> dictionary = new Dictionary<string, JValue>();
						if (pTokenizer.Peek() == "}")
						{
							pTokenizer.Read();
						}
						else
						{
							while (true)
							{
								if (!pTokenizer.ExpectToken("\""))
								{
									ReportError(pTokenizer, pOptions, "Expected open-quote");
									return null;
								}
								string text2 = ParseString(pTokenizer, pOptions);
								if (text2 == null)
								{
									return null;
								}
								if (!pTokenizer.ExpectToken(":"))
								{
									ReportError(pTokenizer, pOptions, "Expected colon");
									return null;
								}
								JValue jValue2 = ParseValue(pTokenizer, pOptions);
								if (jValue2 == null)
								{
									return null;
								}
								dictionary.Add(text2, jValue2);
								char c2 = pTokenizer.ReadPunctuationToken();
								if (c2 == '}')
								{
									break;
								}
								if (c2 != ',')
								{
									ReportError(pTokenizer, pOptions, "Expected comma or close-curly-bracket, got " + c2);
									return null;
								}
							}
						}
						return new JValue(dictionary);
					}
					case "[":
					{
						List<JValue> list = new List<JValue>();
						if (pTokenizer.Peek() == "]")
						{
							pTokenizer.Read();
						}
						else
						{
							while (true)
							{
								JValue jValue = ParseValue(pTokenizer, pOptions);
								if (jValue == null)
								{
									return null;
								}
								list.Add(jValue);
								char c = pTokenizer.ReadPunctuationToken();
								if (c == ']')
								{
									break;
								}
								if (c != ',')
								{
									ReportError(pTokenizer, pOptions, "Expected comma or close-square-bracket, got " + c);
									return null;
								}
							}
						}
						return new JValue(list);
					}
					default:
						ReportError(pTokenizer, pOptions, "Couldn't decode value");
						return null;
					}
				}
				goto case "-";
			case "-":
			{
				string text4 = text;
				if (text == "-")
				{
					text4 += pTokenizer.Read();
				}
				if (pTokenizer.Peek() == ".")
				{
					string pToken;
					string pToken2;
					if (!pTokenizer.TryRead(out pToken) || !pTokenizer.TryRead(out pToken2))
					{
						ReportError(pTokenizer, pOptions, "Unexpected end of stream");
						return null;
					}
					text4 = text4 + pToken + pToken2;
				}
				if (pTokenizer.Peek().ToLower() == "e")
				{
					string pToken3;
					string pToken4;
					if (!pTokenizer.TryRead(out pToken3) || !pTokenizer.TryRead(out pToken4))
					{
						return null;
					}
					text4 += pToken3;
					text4 += pToken4;
					if (pToken4 == "+" || pToken4 == "-")
					{
						if (!pTokenizer.TryRead(out pToken4))
						{
							ReportError(pTokenizer, pOptions, "Unexpected end of stream");
							return null;
						}
						text4 += pToken4;
					}
				}
				return new JValue(double.Parse(text4));
			}
			}
		}

		protected static string ParseString(TokenStream pTokenizer, ParseOptions pOptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				char c = pTokenizer.ReadSingleCharacter();
				switch (c)
				{
				case '"':
					return stringBuilder.ToString();
				case '\\':
					c = pTokenizer.ReadSingleCharacter();
					switch (c)
					{
					case '\0':
						ReportError(pTokenizer, pOptions, "Unexpected end of stream");
						return null;
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
						string s = pTokenizer.ReadCharacters(4);
						ushort value = ushort.Parse(s, NumberStyles.HexNumber);
						stringBuilder.Append((char)value);
						break;
					}
					default:
						ReportError(pTokenizer, pOptions, "Bad escape character");
						return null;
					}
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
			}
		}

		public string Format(FormatMode pMode = FormatMode.Compact)
		{
			return Format(this, pMode);
		}

		public static string Format(JValue pValue, FormatMode pMode = FormatMode.Compact)
		{
			SourceStringBuilder sourceStringBuilder = new SourceStringBuilder();
			FormatInto(sourceStringBuilder, pValue, pMode);
			return sourceStringBuilder.ToString();
		}

		protected static void FormatInto(SourceStringBuilder pStringBuilder, JValue pValue, FormatMode pMode)
		{
			bool flag = true;
			switch (pValue._type)
			{
			case Type.Null:
				pStringBuilder.Append("null");
				break;
			case Type.True:
				pStringBuilder.Append("true");
				break;
			case Type.False:
				pStringBuilder.Append("false");
				break;
			case Type.Number:
				pStringBuilder.Append(pValue.DoubleValue.Value);
				break;
			case Type.String:
				FormatString(pValue.StringValue, pStringBuilder);
				break;
			case Type.Array:
				pStringBuilder.Append("[");
				flag = true;
				foreach (JValue item in pValue)
				{
					if (!flag)
					{
						pStringBuilder.Append(',');
						if (pMode == FormatMode.Verbose)
						{
							pStringBuilder.Append(' ');
						}
					}
					flag = false;
					FormatInto(pStringBuilder, item, pMode);
				}
				pStringBuilder.Append(']');
				break;
			case Type.Object:
				pStringBuilder.Append('{');
				if (pMode == FormatMode.Verbose)
				{
					pStringBuilder.AppendLine();
					pStringBuilder.Indent();
				}
				flag = true;
				foreach (KeyValuePair<string, JValue> field in pValue.Fields)
				{
					if (!flag)
					{
						pStringBuilder.Append(',');
						if (pMode == FormatMode.Verbose)
						{
							pStringBuilder.AppendLine();
						}
					}
					flag = false;
					FormatString(field.Key, pStringBuilder);
					pStringBuilder.Append(':');
					if (pMode == FormatMode.Verbose)
					{
						pStringBuilder.Append(' ');
					}
					FormatInto(pStringBuilder, field.Value, pMode);
				}
				if (pMode == FormatMode.Verbose)
				{
					pStringBuilder.AppendLine();
					pStringBuilder.Unindent();
				}
				pStringBuilder.Append('}');
				break;
			}
		}

		protected static void FormatString(string pString, SourceStringBuilder pStringBuilder)
		{
			pStringBuilder.Append('"');
			foreach (char c in pString)
			{
				switch (c)
				{
				case '\b':
					pStringBuilder.Append("\\b");
					continue;
				case '\n':
					pStringBuilder.Append("\\n");
					continue;
				case '\t':
					pStringBuilder.Append("\\t");
					continue;
				case '\r':
					pStringBuilder.Append("\\r");
					continue;
				case '\f':
					pStringBuilder.Append("\\f");
					continue;
				case '\\':
					pStringBuilder.Append("\\\\");
					continue;
				case '"':
					pStringBuilder.Append("\\\"");
					continue;
				}
				if (c >= '膧')
				{
					pStringBuilder.AppendFormat("\\u{0:x4}", (int)c);
				}
				else
				{
					pStringBuilder.Append(c);
				}
			}
			pStringBuilder.Append('"');
		}

		public JValue Clone()
		{
			switch (_type)
			{
			case Type.Array:
			{
				List<JValue> list = new List<JValue>(AsList.Count);
				for (int i = 0; i < AsList.Count; i++)
				{
					list.Add(AsList[i].Clone());
				}
				return new JValue(list);
			}
			case Type.Object:
			{
				Dictionary<string, JValue> dictionary = new Dictionary<string, JValue>(AsDictionary.Count);
				foreach (KeyValuePair<string, JValue> item in AsDictionary)
				{
					dictionary.Add(item.Key, item.Value.Clone());
				}
				return new JValue(dictionary);
			}
			default:
				return this;
			}
		}

		public static JValue CreateObject()
		{
			return new JValue(new Dictionary<string, JValue>());
		}

		public static JValue CreateArray()
		{
			return new JValue(new List<JValue>());
		}

		public static JValue FromEnumerable(IEnumerable<int> pValue)
		{
			JValue jValue = CreateArray();
			jValue.Add(pValue);
			return jValue;
		}

		public static JValue FromEnumerable(IEnumerable<float> pValue)
		{
			JValue jValue = CreateArray();
			jValue.Add(pValue);
			return jValue;
		}

		public static JValue FromEnumerable(IEnumerable<string> pValue)
		{
			JValue jValue = CreateArray();
			jValue.Add(pValue);
			return jValue;
		}

		public static JValue FromEnumerable(IEnumerable<bool> pValue)
		{
			JValue jValue = CreateArray();
			jValue.Add(pValue);
			return jValue;
		}

		public override string ToString()
		{
			switch (_type)
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
				if (_value != null)
				{
					return _value.ToString();
				}
				return string.Empty;
			}
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
	}
}
