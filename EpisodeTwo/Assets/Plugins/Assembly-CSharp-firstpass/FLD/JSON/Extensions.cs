namespace FLD.JSON
{
	public static class Extensions
	{
		public static JValue DecodeJSON(this string pString)
		{
			return JValue.Decode(pString);
		}
	}
}
