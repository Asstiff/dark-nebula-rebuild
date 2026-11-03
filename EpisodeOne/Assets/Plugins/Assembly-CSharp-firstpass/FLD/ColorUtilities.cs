using System.Globalization;
using FLD.JSON;
using UnityEngine;

namespace FLD
{
	public static class ColorUtilities
	{
		public static Color FromHex(string pHexString)
		{
			if (pHexString.Length > 0 && pHexString[0] == '#')
			{
				pHexString = pHexString.Substring(1);
			}
			if (pHexString.Length < 6 || pHexString.Length > 8)
			{
				return Color.black;
			}
			uint result;
			if (uint.TryParse(pHexString, NumberStyles.HexNumber, null, out result))
			{
				if ((result & 0xFF000000u) != 0)
				{
					return FromHexWithAlpha(result);
				}
				return FromHex(result);
			}
			return Color.black;
		}

		public static Color FromHex(uint pHex)
		{
			return FromHexWithAlpha(0xFF000000u | pHex);
		}

		public static Color FromHexWithAlpha(uint pHex)
		{
			uint pA = (pHex >> 24) & 0xFF;
			uint pR = (pHex >> 16) & 0xFF;
			uint pG = (pHex >> 8) & 0xFF;
			uint pB = pHex & 0xFF;
			return From8Bit((int)pR, (int)pG, (int)pB, (int)pA);
		}

		public static string ToHexString(this Color pColor)
		{
			return string.Format("{0:xx}{1:xx}{2:xx}{3:xx}", (int)(pColor.a * 255f), (int)(pColor.r * 255f), (int)(pColor.g * 255f), (int)(pColor.b * 255f));
		}

		public static Color From8Bit(int pR, int pG, int pB)
		{
			return new Color((float)pR / 255f, (float)pG / 255f, (float)pB / 255f, 1f);
		}

		public static Color From8Bit(int pR, int pG, int pB, int pA)
		{
			return new Color((float)pR / 255f, (float)pG / 255f, (float)pB / 255f, (float)pA / 255f);
		}

		public static JValue ToJson(this Color pColor)
		{
			JValue jValue = new JValue();
			jValue.Add("r", pColor.r);
			jValue.Add("g", pColor.g);
			jValue.Add("b", pColor.b);
			jValue.Add("a", pColor.a);
			return jValue;
		}

		public static Color ToColor(this JValue pJson)
		{
			float? floatValue = pJson["r"].FloatValue;
			float r = ((!floatValue.HasValue) ? 0f : floatValue.Value);
			float? floatValue2 = pJson["g"].FloatValue;
			float g = ((!floatValue2.HasValue) ? 0f : floatValue2.Value);
			float? floatValue3 = pJson["b"].FloatValue;
			float b = ((!floatValue3.HasValue) ? 0f : floatValue3.Value);
			float? floatValue4 = pJson["a"].FloatValue;
			return new Color(r, g, b, (!floatValue4.HasValue) ? 0f : floatValue4.Value);
		}
	}
}
