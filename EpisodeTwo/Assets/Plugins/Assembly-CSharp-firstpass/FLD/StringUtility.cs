using System.Text;

namespace FLD
{
	public static class StringUtility
	{
		private static readonly char[] _hexCharacters = new char[16]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f'
		};

		private static char[] _formatChars = new char[20];

		private static bool _hasSeparators = false;

		private static int _digitGroupCount;

		private static char _digitGroupSeparator;

		public static string ToHexString(this byte[] pBytes)
		{
			StringBuilder stringBuilder = new StringBuilder(pBytes.Length * 2);
			foreach (byte b in pBytes)
			{
				stringBuilder.Append(_hexCharacters[(b >> 4) & 0xF]);
				stringBuilder.Append(_hexCharacters[b & 0xF]);
			}
			return stringBuilder.ToString();
		}

		public static bool IsAlphanumeric(this string pString)
		{
			foreach (char c in pString)
			{
				if (!char.IsLetterOrDigit(c))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsNumeric(this string pString)
		{
			foreach (char c in pString)
			{
				if (!char.IsDigit(c))
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
	}
}
