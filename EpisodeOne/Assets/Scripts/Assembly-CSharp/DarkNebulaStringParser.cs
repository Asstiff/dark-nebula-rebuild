using System.Collections;
using UnityEngine;

public class DarkNebulaStringParser
{
	public static Hashtable Parse(string s)
	{
		Hashtable hashtable = new Hashtable();
		string[] array = s.Split('\n');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.StartsWith("\""))
			{
				continue;
			}
			string[] array3 = text.Split('=');
			if (array3.Length > 1)
			{
				string text2 = array3[0];
				text2 = text2.Split('"')[1];
				text2 = text2.Replace("\\n", "\n");
				string text3 = array3[1];
				text3 = text3.Split('"')[1];
				text3 = text3.Replace("\\n", "\n");
				if (!hashtable.ContainsKey(text2))
				{
					hashtable.Add(text2, text3);
				}
				else
				{
					Debug.LogWarning("Duplicate stringID " + text2 + " " + text3);
				}
			}
		}
		return hashtable;
	}
}
