using System.Collections.Generic;

namespace FLD.JSON
{
	public static class Extensions
	{
		public static JValue ParseJSON(this string pString)
		{
			return JValue.Parse(pString);
		}

		public static JValue ToJson(this IEnumerable<string> pList)
		{
			JValue jValue = JValue.CreateArray();
			foreach (string p in pList)
			{
				jValue.Add(p);
			}
			return jValue;
		}
	}
}
