using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ArabicSupport;
using UnityEngine;

namespace FLD
{
	public class LocalizationManager
	{
		private const string FallbackLanguageId = "en";

		private string _primaryLanguageId = string.Empty;

		private Dictionary<string, Dictionary<string, string[]>> _wordLists = new Dictionary<string, Dictionary<string, string[]>>();

		private string _platform;

		private Dictionary<string, string[]> _primaryWords;

		private Dictionary<string, string[]> _fallbackWords;

		private bool _hasFileForPrimaryLanguage;

		private CultureInfo _culture;

		private static string _filenameForLocalization = string.Empty;

		private static LocalizationManager _instance;

		public string PrimaryLanguageId
		{
			get
			{
				if (_primaryLanguageId == string.Empty || !HasFileForPrimaryLanguage)
				{
					return "en";
				}
				return _primaryLanguageId;
			}
			set
			{
				SetPrimaryLanguage(value);
			}
		}

		public bool HasFileForPrimaryLanguage
		{
			get
			{
				return _hasFileForPrimaryLanguage;
			}
		}

		public bool IsRightToLeftLanguage
		{
			get
			{
				return PrimaryLanguageId == "ar";
			}
		}

		public static LocalizationManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new LocalizationManager();
				}
				return _instance;
			}
		}

		private LocalizationManager()
		{
			_culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
			SetPrimaryLanguage("en");
			if (string.IsNullOrEmpty(_filenameForLocalization))
			{
				if (!TryLoadLanguage("en"))
				{
					Debug.LogError("Failed to load fallback language; strings will be wrong");
				}
			}
			else if (!TryLoadLanguage("en", _filenameForLocalization))
			{
				Debug.LogError("Failed to load fallback language; strings will be wrong");
			}
			_wordLists.TryGetValue("en", out _fallbackWords);
		}

		public static void SetToLoadFromResx(string pFileName)
		{
			_filenameForLocalization = pFileName;
		}

		public static void SetToLoadFromText()
		{
			_filenameForLocalization = string.Empty;
		}

		public static string GetResourcePathToLanguage(string pFile)
		{
			return "Languages/" + pFile;
		}

		public string GetSystemLanguage()
		{
			string iso639Code = GetIso639Code(Application.systemLanguage);
			if (string.IsNullOrEmpty(iso639Code))
			{
				return "en";
			}
			return iso639Code;
		}

		public static string GetIso639Code(SystemLanguage pLanguage)
		{
			switch (pLanguage)
			{
			case SystemLanguage.Afrikaans:
				return "af";
			case SystemLanguage.Arabic:
				return "ar";
			case SystemLanguage.Basque:
				return "eu";
			case SystemLanguage.Belarusian:
				return "be";
			case SystemLanguage.Bulgarian:
				return "bg";
			case SystemLanguage.Catalan:
				return "ca";
			case SystemLanguage.Chinese:
				return "zh";
			case SystemLanguage.Czech:
				return "cs";
			case SystemLanguage.Danish:
				return "da";
			case SystemLanguage.Dutch:
				return "nl";
			case SystemLanguage.English:
				return "en";
			case SystemLanguage.Estonian:
				return "ee";
			case SystemLanguage.Faroese:
				return "fo";
			case SystemLanguage.Finnish:
				return "fi";
			case SystemLanguage.French:
				return "fr";
			case SystemLanguage.German:
				return "de";
			case SystemLanguage.Greek:
				return "el";
			case SystemLanguage.Hebrew:
				return "he";
			case SystemLanguage.Hungarian:
				return "hu";
			case SystemLanguage.Icelandic:
				return "is";
			case SystemLanguage.Indonesian:
				return "id";
			case SystemLanguage.Italian:
				return "it";
			case SystemLanguage.Japanese:
				return "ja";
			case SystemLanguage.Korean:
				return "ko";
			case SystemLanguage.Latvian:
				return "lv";
			case SystemLanguage.Lithuanian:
				return "lt";
			case SystemLanguage.Norwegian:
				return "no";
			case SystemLanguage.Polish:
				return "pl";
			case SystemLanguage.Portuguese:
				return "pt";
			case SystemLanguage.Romanian:
				return "ro";
			case SystemLanguage.Russian:
				return "ru";
			case SystemLanguage.SerboCroatian:
				return "hr";
			case SystemLanguage.Slovak:
				return "sk";
			case SystemLanguage.Slovenian:
				return "sl";
			case SystemLanguage.Spanish:
				return "es";
			case SystemLanguage.Swedish:
				return "sv";
			case SystemLanguage.Thai:
				return "th";
			case SystemLanguage.Turkish:
				return "tr";
			case SystemLanguage.Ukrainian:
				return "uk";
			case SystemLanguage.Vietnamese:
				return "vi";
			default:
				return string.Empty;
			}
		}

		private Dictionary<string, string[]> GetWordList(string pLanguage)
		{
			Dictionary<string, string[]> value;
			if (!_wordLists.TryGetValue(pLanguage, out value))
			{
				return null;
			}
			return value;
		}

		public void SetPrimaryLanguage(string pLanguageId)
		{
			_primaryLanguageId = pLanguageId;
			if (!_wordLists.ContainsKey(_primaryLanguageId))
			{
				_hasFileForPrimaryLanguage = TryLoadLanguage(_primaryLanguageId);
				if (string.IsNullOrEmpty(_filenameForLocalization))
				{
					TryLoadLanguage("en");
				}
				else
				{
					TryLoadLanguage("en", _filenameForLocalization);
				}
			}
			_wordLists.TryGetValue(_primaryLanguageId, out _primaryWords);
		}

		public void SetPlatform(string pPlatform)
		{
			_platform = pPlatform;
		}

		private string[] GetTextArray(string pStringId)
		{
			string key = NormalizeStringId(pStringId);
			string[] value;
			if ((_primaryWords == null || !_primaryWords.TryGetValue(key, out value)) && (_fallbackWords == null || !_fallbackWords.TryGetValue(key, out value)))
			{
				return null;
			}
			return value;
		}

		public bool TryGetText(int pIndex, string pStringId, out string pOutText)
		{
			if (pIndex == 0 && GameLocalization.TryGet(pStringId, out pOutText)) return true;
			string[] textArray = GetTextArray(pStringId);
			if (textArray == null)
			{
				pOutText = string.Empty;
				return false;
			}
			if (pIndex < 0 || pIndex >= textArray.Length)
			{
				pOutText = string.Empty;
				return false;
			}
			pOutText = textArray[pIndex];
			return true;
		}

		public bool TryGetText(int pIndex, string pStringId, out string pOutText, params object[] pParameters)
		{
			if (TryGetText(pIndex, pStringId, out pOutText))
			{
				if (pParameters.Length > 0)
				{
					try
					{
						pOutText = string.Format(_culture, pOutText, pParameters);
					}
					catch (FormatException ex)
					{
						Debug.LogError("String format error: " + ex.Message);
						pOutText = string.Empty;
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public string GetText(string pStringId)
		{
			return GetText(0, pStringId);
		}

		public string GetText(string pStringId, params object[] pParameters)
		{
			return GetText(0, pStringId, pParameters);
		}

		public string GetText(int pIndex, string pStringId, params object[] pParameters)
		{
			string pOutText;
			if (!TryGetText(pIndex, pStringId, out pOutText, pParameters))
			{
				return string.Format("#{0}#", pStringId);
			}
			return pOutText;
		}

		public string GetText(int pIndex, string pStringId)
		{
			string pOutText;
			if (!TryGetText(pIndex, pStringId, out pOutText))
			{
				return string.Format("#{0}#", pStringId);
			}
			return pOutText;
		}

		public string ConvertForRendering(string pString)
		{
			if (PrimaryLanguageId == "ar")
			{
				return ConvertArabicForRendering(pString);
			}
			return pString;
		}

		public string ConvertArabicForRendering(string pString, bool pUseHinduNumerals = false)
		{
			return ArabicFixer.Fix(pString, true, pUseHinduNumerals);
		}

		public int GetCount(string pStringId)
		{
			string[] textArray = GetTextArray(pStringId);
			if (textArray == null)
			{
				return 0;
			}
			return textArray.Length;
		}

		private string NormalizeStringId(string pStringId)
		{
			string text = pStringId.ToLower();
			if (!string.IsNullOrEmpty(_platform) && _primaryWords.ContainsKey(_platform + text))
			{
				text = _platform + text;
			}
			return text.Replace(" ", ".");
		}

		public void ResetForTesting()
		{
			_instance = null;
		}

		public bool TryLoadLanguage(string pLanguageId, string pFilename)
		{
			if (_wordLists.ContainsKey(pLanguageId))
			{
				return true;
			}
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
			AddWordsFromResX(GetResourcePathToLanguage(pFilename + "_" + pLanguageId), dictionary);
			if (dictionary.Count > 0)
			{
				_wordLists.Add(pLanguageId, dictionary);
				Debug.Log(string.Format("Loaded language \"{0}\" from ResX ok", pLanguageId));
				return true;
			}
			return false;
		}

		public bool TryLoadLanguage(string pLanguageId)
		{
			if (_wordLists.ContainsKey(pLanguageId))
			{
				return true;
			}
			Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
			AddFileToWordList(GetResourcePathToLanguage(pLanguageId), dictionary);
			if (dictionary.Count > 0)
			{
				_wordLists.Add(pLanguageId, dictionary);
				Debug.Log(string.Format("Loaded language \"{0}\" from textfile ok", pLanguageId));
				return true;
			}
			return false;
		}

		private void ProcessMetaCommand(string pCommand, string pArguments, Dictionary<string, string[]> pWordList)
		{
			switch (pCommand)
			{
			case "include":
				AddFileToWordList(GetResourcePathToLanguage(pArguments), pWordList);
				break;
			case "decimal_separator":
				_culture.NumberFormat.NumberDecimalSeparator = pArguments;
				break;
			case "group_separator":
				_culture.NumberFormat.NumberGroupSeparator = pArguments;
				StringExtensions.DigitGroupSeparator = pArguments[0];
				break;
			case "group_size":
			{
				int result;
				if (int.TryParse(pArguments, out result))
				{
					_culture.NumberFormat.NumberGroupSizes = new int[1] { result };
					StringExtensions.DigitGroupCount = result;
				}
				break;
			}
			}
		}

		private void AddFileToWordList(string pFilename, Dictionary<string, string[]> pWordList)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(pFilename);
			if (textAsset == null)
			{
				return;
			}
			using (StringReader stringReader = new StringReader(textAsset.text))
			{
				string text;
				while ((text = stringReader.ReadLine()) != null)
				{
					text = text.Trim();
					if (text.Length <= 0)
					{
						continue;
					}
					if (text[0] == '#')
					{
						if (text.Length > 1 && text[1] == '!')
						{
							string[] array = text.Split(' ');
							ProcessMetaCommand(array[0].Substring(2), array[1], pWordList);
						}
						continue;
					}
					string[] array2 = text.Split('=');
					if (array2 == null || array2.Length != 2)
					{
						continue;
					}
					string key = NormalizeStringId(array2[0]);
					string text2 = array2[1];
					if (text2 == "@array")
					{
						List<string> list = new List<string>();
						while ((text = stringReader.ReadLine()) != null)
						{
							text = text.Trim();
							if (text == "@end")
							{
								break;
							}
							list.Add(text.Unescape());
						}
						pWordList[key] = list.ToArray();
					}
					else
					{
						pWordList[key] = new string[1] { text2.Unescape() };
					}
				}
			}
		}

		private void AddWordsFromResX(string pFilename, Dictionary<string, string[]> pWordList)
		{
			TextAsset textAsset = (TextAsset)Resources.Load(pFilename);
			if (textAsset == null)
			{
				Debug.LogError("Localization file not found!");
				return;
			}
			NanoXMLDocument nanoXMLDocument = new NanoXMLDocument(textAsset.text);
			bool flag = false;
			List<string> list = new List<string>();
			string text = string.Empty;
			foreach (NanoXMLNode subNode in nanoXMLDocument.RootNode.SubNodes)
			{
				if (subNode["value"].Value == null)
				{
					continue;
				}
				string value = subNode.GetAttribute("name").Value;
				string value2 = subNode["value"].Value;
				if (value.Contains("@"))
				{
					if (flag)
					{
						list.Add(value2);
						continue;
					}
					int num = value.IndexOf('@');
					if (num > 0)
					{
						text = value.Substring(0, num);
					}
					else
					{
						Debug.LogError("Error with an array");
					}
					flag = true;
					list.Clear();
					list.Add(value2);
				}
				else if (!flag)
				{
					pWordList[value] = new string[1] { value2.Unescape() };
				}
				else
				{
					pWordList[text] = list.ToArray();
					flag = false;
				}
			}
			if (flag)
			{
				Debug.Log("Added array with " + list.Count + " texts, id: " + text);
				pWordList[text] = list.ToArray();
			}
		}
	}
}
