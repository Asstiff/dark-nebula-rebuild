namespace FLD
{
	public static class LocalizationExtensions
	{
		public static string Localize(this string pStringId)
		{
			return LocalizationManager.Instance.GetText(pStringId);
		}

		public static string Localize(this string pStringId, params object[] pArgs)
		{
			return LocalizationManager.Instance.GetText(pStringId, pArgs);
		}
	}
}
