using System.Collections;
using UnityEngine;

public class Language
{
	private static Language _current;

	public Hashtable table = new Hashtable();

	public SystemLanguage language;

	public static string LanguageID
	{
		get
		{
			if (_current == null)
			{
				SetLanguage(SystemLanguage.English);
			}
			return GetLanguageFileName(_current.language);
		}
	}

	public Language(SystemLanguage pLanguage)
	{
		language = pLanguage;
		string languageFileName = GetLanguageFileName(pLanguage);
		TextAsset textAsset = (TextAsset)Resources.Load("Languages/" + languageFileName, typeof(TextAsset));
		table = DarkNebulaStringParser.Parse(textAsset.text);
	}

	public static void SetLanguage(SystemLanguage language)
	{
		_current = new Language(language);
	}

	public static string Get(string id)
	{
        string translated;
        if (GameLocalization.TryGet(id, out translated)) return translated;
		if (_current == null)
		{
			SetLanguage(SystemLanguage.English);
		}
		string text = (string)_current.table[id];
		if (string.IsNullOrEmpty(text))
		{
			return "?" + id;
		}
		return text;
	}

	public static string Get(string id, string replacement)
	{
		return Get(id).Replace("%d", replacement);
	}

	public static string Get(string id, int replacement)
	{
		return Get(id).Replace("%d", replacement.ToString());
	}

	private static string GetLanguageFileName(SystemLanguage pLanguage)
	{
		string result = "en";
		switch (pLanguage.ToString().ToLower())
		{
		case "spanish":
			result = "es";
			break;
		case "italian":
			result = "it";
			break;
		case "french":
			result = "fr";
			break;
		case "german":
			result = "de";
			break;
		}
		return result;
	}
}
