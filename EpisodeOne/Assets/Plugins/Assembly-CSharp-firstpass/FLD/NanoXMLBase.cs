using System.Collections.Generic;

namespace FLD
{
	public class NanoXMLBase
	{
		protected static bool IsSpace(char c)
		{
			return c == ' ' || c == '\t' || c == '\n' || c == '\r';
		}

		protected static void SkipSpaces(string str, ref int i)
		{
			while (i < str.Length)
			{
				if (!IsSpace(str[i]))
				{
					if (str[i] != '<' || i + 4 >= str.Length || str[i + 1] != '!' || str[i + 2] != '-' || str[i + 3] != '-')
					{
						break;
					}
					i += 4;
					while (i + 2 < str.Length && (str[i] != '-' || str[i + 1] != '-'))
					{
						i++;
					}
					i += 2;
				}
				i++;
			}
		}

		protected static string GetValue(string str, ref int i, char endChar, char endChar2, bool stopOnSpace)
		{
			int num = i;
			while ((!stopOnSpace || !IsSpace(str[i])) && str[i] != endChar && str[i] != endChar2)
			{
				i++;
			}
			return str.Substring(num, i - num);
		}

		protected static bool IsQuote(char c)
		{
			return c == '"' || c == '\'';
		}

		protected static string ParseAttributes(string str, ref int i, List<NanoXMLAttribute> attributes, char endChar, char endChar2)
		{
			SkipSpaces(str, ref i);
			string value = GetValue(str, ref i, endChar, endChar2, true);
			SkipSpaces(str, ref i);
			while (str[i] != endChar && str[i] != endChar2)
			{
				string value2 = GetValue(str, ref i, '=', '\0', true);
				SkipSpaces(str, ref i);
				i++;
				SkipSpaces(str, ref i);
				char c = str[i];
				if (!IsQuote(c))
				{
					throw new XMLParsingException("Unexpected token after " + value2);
				}
				i++;
				string value3 = GetValue(str, ref i, c, '\0', false);
				i++;
				attributes.Add(new NanoXMLAttribute(value2, value3));
				SkipSpaces(str, ref i);
			}
			return value;
		}
	}
}
