using System.Collections.Generic;

public static class FugaJsonExtensions
{
	public static string toJson(this Dictionary<string, object> obj)
	{
		return FugaJSON.jsonEncode(obj);
	}

	public static List<object> listFromJson(this string json)
	{
		return FugaJSON.jsonDecode(json) as List<object>;
	}

	public static Dictionary<string, object> dictionaryFromJson(this string json)
	{
		return FugaJSON.jsonDecode(json) as Dictionary<string, object>;
	}
}
