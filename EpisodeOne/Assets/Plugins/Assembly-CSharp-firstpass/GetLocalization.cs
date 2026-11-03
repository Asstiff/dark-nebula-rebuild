using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class GetLocalization : MonoBehaviour
{
	public class Phrase
	{
		public string phrase_id;

		public string phrase;

		public string comment;

		public bool hasChanged;

		public Phrase(string phrase_id, string phrase, string comment)
		{
			this.phrase_id = phrase_id;
			this.phrase = phrase;
			this.comment = comment;
		}

		public bool Equals(Phrase p)
		{
			if (p == null)
			{
				return false;
			}
			return phrase == p.phrase && phrase_id == p.phrase_id && comment == p.comment;
		}
	}

	private const string password = "";

	private const string userName = "";

	public static readonly string defaultPhraseId = "game_phrase_id";

	public static readonly string defaultPhrase = "game_phrase";

	public static readonly string defaultComment = "game_phrase_comment";

	private string currentFilename = string.Empty;

	private string currentLanguageCode = "en";

	private static bool allowDuplicatePhraseIds;

	private static bool allowEmptyPhrases;

	private static bool allowPlaceHoldersWithoutComments = true;

	private static bool alwaysSaveMasterFileToDisk = true;

	private static WWW www;

	private static Action<bool, string> wwwCallback;

	public void GetMasterFileForLanguage(string pFilename, string pLanguageCode, Action<bool, string> pCallback)
	{
		wwwCallback = pCallback;
		wwwCallback = (Action<bool, string>)Delegate.Combine(wwwCallback, new Action<bool, string>(SaveFileToDisk));
		currentFilename = pFilename;
		string url = "https://api.getlocalization.com/translatepalringo/api/translations/file/" + pFilename + "/" + pLanguageCode + "/";
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(":"));
		www = new WWW(url, null, dictionary);
	}

	public void UpdateMasterFileForLanguage(string pContent, string pFilename, Action<bool, string> pCallback)
	{
		Debug.Log("Trying to update master file: " + pFilename + " " + pContent + "\n\nwith usrname/pwd " + "".ToString() + "/" + "".ToString());
		string url = "https://api.getlocalization.com/translatepalringo/api/update-master/";
		byte[] bytes = Encoding.UTF8.GetBytes(pContent);
		wwwCallback = pCallback;
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddBinaryData("file", bytes, pFilename, "text/plain");
		var headers = wWWForm.headers;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (var item in headers)
		{
			dictionary.Add(item.Key.ToString(), item.Value.ToString());
		}
		dictionary.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(":")));
		www = new WWW(url, wWWForm.data, dictionary);
	}

	public void UploadNewMasterFile(string pContent, string pFilename, Action<bool, string> pCallback)
	{
		wwwCallback = pCallback;
		Debug.Log("Trying to upload new master file: " + pFilename + " " + pContent + "\n\nwith usrname/pwd " + "".ToString() + "/" + "".ToString());
		string url = "https://api.getlocalization.com/translatepalringo/api/create-master/resx/en/";
		WWWForm wWWForm = new WWWForm();
		byte[] bytes = Encoding.UTF8.GetBytes(pContent);
		wWWForm.AddBinaryData("file", bytes, pFilename, "text/plain");
		var headers = wWWForm.headers;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (var item in headers)
		{
			dictionary.Add(item.Key.ToString(), item.Value.ToString());
		}
		dictionary.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(":")));
		www = new WWW(url, wWWForm.data, dictionary);
		if (alwaysSaveMasterFileToDisk)
		{
			int num = pFilename.IndexOf('.');
			if (num > 0)
			{
				StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/Languages/" + pFilename.Substring(0, num) + "_en.txt");
				streamWriter.WriteLine(pContent);
				streamWriter.Close();
			}
			else
			{
				Debug.LogError("Something wrong with filename, missing extension");
			}
		}
	}

	public void GetTranslationForAllSupportedLanguages(string pFilename, Action<bool, string> pCallbackWhenDone)
	{
	}

	public static float GetDownloadOrUploadProgress()
	{
		if (www != null)
		{
			return www.progress;
		}
		return 1f;
	}

	private static void UpdateDownloadAndUpload()
	{
		if (www == null || !www.isDone)
		{
			return;
		}
		if (www.error != null)
		{
			string error = www.error;
			RemoveWWW();
			Debug.Log("Error: " + error);
			if (wwwCallback != null)
			{
				wwwCallback(false, error);
			}
		}
		else
		{
			string text = www.text;
			RemoveWWW();
			if (wwwCallback != null)
			{
				wwwCallback(true, text);
			}
		}
	}

	private static void RemoveWWW()
	{
		www.Dispose();
		www = null;
	}

	public static void UpdateInEditor()
	{
		UpdateDownloadAndUpload();
	}

	public static string BuildResxFromPhraselist(List<Phrase> pPhrases)
	{
		string text = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<root>\n\n";
		foreach (Phrase pPhrase in pPhrases)
		{
			text += GetPhraseElementString(pPhrase.phrase_id, pPhrase.phrase, pPhrase.comment);
		}
		return text + "\n</root>";
	}

	private void SaveFileToDisk(bool pSuccess, string pContent)
	{
		if (pSuccess)
		{
			int num = currentFilename.IndexOf('.');
			string text = currentFilename;
			if (num > 0)
			{
				text = currentFilename.Substring(0, num) + "_" + currentLanguageCode + ".txt";
			}
			else
			{
				Debug.LogError("Error with filename");
			}
			StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/Languages/" + text);
			streamWriter.WriteLine(pContent);
			streamWriter.Close();
		}
	}

	private static string GetPhraseElementString(string pPhraseId, string pPhrase, string pComment)
	{
		string text = " <data name= \"" + pPhraseId + "\">\n  <value>" + pPhrase + "</value>\n";
		if (pComment != string.Empty)
		{
			text = text + "  <comment>" + pComment + "</comment>\n";
		}
		return text + " </data>\n\n";
	}

	public static KeyValuePair<bool, string> ArePhrasesOk(List<Phrase> pPhrases)
	{
		foreach (Phrase pPhrase in pPhrases)
		{
			if (string.IsNullOrEmpty(pPhrase.phrase_id) || (!allowEmptyPhrases && string.IsNullOrEmpty(pPhrase.phrase)))
			{
				return new KeyValuePair<bool, string>(false, "Empty phrase or phrase id found! Not allowed!");
			}
			if (!allowDuplicatePhraseIds)
			{
				foreach (Phrase pPhrase2 in pPhrases)
				{
					if (pPhrase != pPhrase2 && pPhrase.phrase_id == pPhrase2.phrase_id)
					{
						return new KeyValuePair<bool, string>(false, "Duplicate phrase_ids found! Not allowed!");
					}
				}
			}
			if (!allowPlaceHoldersWithoutComments && pPhrase.phrase.Contains("{0}") && string.IsNullOrEmpty(pPhrase.comment))
			{
				return new KeyValuePair<bool, string>(false, "Phrase with placeholder without comment found! Not allowed!");
			}
		}
		return new KeyValuePair<bool, string>(true, string.Empty);
	}
}
