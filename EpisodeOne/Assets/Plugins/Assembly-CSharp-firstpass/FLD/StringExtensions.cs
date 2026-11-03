using System;
using System.Text;

namespace FLD
{
	public static class StringExtensions
	{
		private static readonly char[] _hexCharacters = new char[16]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f'
		};

		private static readonly int[] _romanNumeralValues = new int[13]
		{
			1000, 900, 500, 400, 100, 90, 50, 40, 10, 9,
			5, 4, 1
		};

		private static readonly string[] _romanNumerals = new string[13]
		{
			"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX",
			"V", "IV", "I"
		};

		public static char DigitGroupSeparator = ',';

		public static int DigitGroupCount = 3;

		public static string FormatInteger(this int pValue)
		{
			return ((long)pValue).FormatInteger();
		}

		public static string FormatIntegerWithExplicitSign(this int pValue)
		{
			return FormatInteger(pValue, true);
		}

		public static string FormatInteger(this long pValue)
		{
			return pValue.FormatInteger(false);
		}

		private static string FormatInteger(this long pValue, bool pAddSignForPositiveNumbers)
		{
			if (pValue == 0L)
			{
				return "0";
			}
			char[] array = new char[32];
			long num = Math.Abs(pValue);
			int num2 = array.Length - 1;
			while (num > 0 && num2 >= 0)
			{
				if ((array.Length - num2) % DigitGroupCount == 0)
				{
					array[num2] = DigitGroupSeparator;
				}
				else
				{
					array[num2] = (char)(48 + num % 10);
					num /= 10;
				}
				num2--;
			}
			if (pValue < 0)
			{
				array[num2--] = '-';
			}
			else if (pValue > 0 && pAddSignForPositiveNumbers)
			{
				array[num2--] = '+';
			}
			return new string(array, num2 + 1, array.Length - 1 - num2);
		}

		public static string ToRomanNumerals(this int pInput)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _romanNumeralValues.Length; i++)
			{
				while (pInput >= _romanNumeralValues[i])
				{
					pInput -= _romanNumeralValues[i];
					stringBuilder.Append(_romanNumerals[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToHexString(this byte[] pBytes)
		{
			char[] array = new char[pBytes.Length * 2];
			for (int i = 0; i < pBytes.Length; i++)
			{
				array[i * 2] = _hexCharacters[(pBytes[i] >> 4) & 0xF];
				array[i * 2 + 1] = _hexCharacters[pBytes[i] & 0xF];
			}
			return new string(array);
		}

		public static bool IsAlphanumeric(this string pString)
		{
			if (string.IsNullOrEmpty(pString))
			{
				return false;
			}
			for (int i = 0; i < pString.Length; i++)
			{
				if (!char.IsLetterOrDigit(pString[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNumeric(this string pString)
		{
			if (string.IsNullOrEmpty(pString))
			{
				return false;
			}
			for (int i = 0; i < pString.Length; i++)
			{
				if (!char.IsDigit(pString[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsValidCSharpIdentifier(this string pString)
		{
			if (string.IsNullOrEmpty(pString) || char.IsDigit(pString[0]) || !pString.IsAlphanumeric())
			{
				return false;
			}
			return true;
		}

		public static string WordWrap(this string pString, int pMaxCharactersPerLine)
		{
			if (string.IsNullOrEmpty(pString))
			{
				return string.Empty;
			}
			int num = pMaxCharactersPerLine;
			string[] array = pString.Split(' ', '\t', '\n');
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Length + 1 > num)
				{
					stringBuilder.Append('\n');
					num = pMaxCharactersPerLine - text.Length;
				}
				else
				{
					num -= text.Length + 1;
				}
				stringBuilder.Append(text).Append(" ");
			}
			return stringBuilder.ToString().Trim();
		}

		public static string Base64Encode(this byte[] pBytes)
		{
			return Convert.ToBase64String(pBytes);
		}

		public static string Base64Encode(this byte[] pBytes, int pLength)
		{
			return Convert.ToBase64String(pBytes, 0, pLength);
		}

		public static byte[] Base64Decode(this string pString)
		{
			return Convert.FromBase64String(pString);
		}

		public static string Unescape(this string pString)
		{
			StringBuilder stringBuilder = new StringBuilder(pString.Length);
			for (int i = 0; i < pString.Length; i++)
			{
				if (pString[i] == '\\')
				{
					i++;
					if (i < pString.Length)
					{
						switch (pString[i])
						{
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 'n':
							stringBuilder.Append('\n');
							break;
						case '\\':
							stringBuilder.Append('\\');
							break;
						case '"':
							stringBuilder.Append('"');
							break;
						case '\'':
							stringBuilder.Append('\'');
							break;
						}
					}
				}
				else
				{
					stringBuilder.Append(pString[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
