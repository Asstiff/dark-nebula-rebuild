using System;
using System.Collections.Generic;
using System.IO;
using FLD.JSON;
using UnityEngine;

public class LevelManager
{
	public static LevelManager Current;

	private bool IsLoaded;

	public List<string> Scenes = new List<string>();

	static LevelManager()
	{
		Current = new LevelManager();
	}

	public string GetSceneName(int intLevel)
	{
		string text = UserSettings.Instance.GetSceneName(intLevel);
		string text2 = text;
		if (!Scenes.Contains(text))
		{
			Debug.Log("Level not found " + text);
			text = text.Replace("level1-", "level1b-");
		}
		if (!Scenes.Contains(text))
		{
			Debug.Log("Level not found " + text);
			text = text2;
		}
		return text;
	}

	public void LoadScenes()
	{
		if (IsLoaded)
		{
			return;
		}
		IsLoaded = true;
		TextAsset textAsset = Resources.Load("buildsettings") as TextAsset;
		if (textAsset == null)
		{
			return;
		}
		string text = string.Empty;
		try
		{
			text = textAsset.text;
		}
		catch (Exception)
		{
		}
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Nothing to load, verify buildsettings.txt");
			return;
		}
		string key = "Scenes";
		JValue jValue = JValue.Parse(text);
		Dictionary<string, JValue> asDictionary = jValue.AsDictionary;
		if (asDictionary == null || !asDictionary.ContainsKey(key))
		{
			return;
		}
		Scenes = new List<string>();
		if (asDictionary[key] != null)
		{
			string[] asStringArray = asDictionary[key].AsStringArray;
			string[] array = asStringArray;
			foreach (string item in array)
			{
				Scenes.Add(item);
			}
		}
	}

	public void SaveScenesFile()
	{
		JValue jValue = JValue.CreateObject();
		JValue jValue2 = JValue.CreateArray();
		foreach (string scene in Scenes)
		{
			jValue2.Add(scene);
		}
		jValue.Add("Scenes", jValue2);
		string contents = jValue.Format(JValue.FormatMode.Verbose);
		File.WriteAllText(Path.Combine(Path.Combine(Application.dataPath, "Resources"), "buildsettings.txt"), contents);
	}

	private List<object> toListObject(List<string> listString)
	{
		List<object> list = new List<object>();
		foreach (string item in listString)
		{
			list.Add(item);
		}
		return list;
	}
}
