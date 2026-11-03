public class LanguageManager
{
	private static LanguageManager _instance;

	public static LanguageManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LanguageManager();
			}
			return _instance;
		}
	}

	public string GetText(string pStringID)
	{
		return Language.Get(pStringID);
	}
}
