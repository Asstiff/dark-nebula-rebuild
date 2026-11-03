using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FLD.JSON;
using UnityEngine;

namespace FLD
{
	public static class Http
	{
		public delegate void URLHandler(bool pSuccess, WWW pWWW);

		private const string UrlEncodeReservedCharacters = "!*'();:@&=+$,/?#[]";

		private const string UrlEncodeUnreservedCharacters = "abcdefghijklmnopqrstuvwxyzABCEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		private static void LogDebugInfo(string pURL, byte[] pPayload, Dictionary<string, string> pHeaders)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("HTTP ").Append(pURL).AppendLine();
			if (pHeaders != null)
			{
				foreach (KeyValuePair<string, string> pHeader in pHeaders)
				{
					stringBuilder.AppendFormat("{0}: {1}\n", pHeader.Key, pHeader.Value);
				}
			}
			if (pPayload != null)
			{
				stringBuilder.AppendLine(Encoding.UTF8.GetString(pPayload));
			}
			Debug.Log(stringBuilder.ToString());
		}

		public static Async<WWW> OpenURL(string pURL)
		{
			LogDebugInfo(pURL, null, null);
			return OpenURL(new WWW(pURL));
		}

		public static Async<WWW> OpenURL(string pURL, Dictionary<string, string> pHeaders)
		{
			if (pHeaders == null)
			{
				return OpenURL(pURL);
			}
			LogDebugInfo(pURL, null, pHeaders);
			return OpenURL(new WWW(pURL, null, pHeaders));
		}

		public static Async<WWW> OpenURL(string pURL, WWWForm pForm)
		{
			LogDebugInfo(pURL, pForm.data, null);
			return OpenURL(new WWW(pURL, pForm));
		}

		public static Async<WWW> OpenURL(string pURL, WWWForm pForm, Dictionary<string, string> pHeaders)
		{
			if (pHeaders == null)
			{
				return OpenURL(pURL, pForm);
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(pHeaders);
			foreach (var header in pForm.headers)
			{
				dictionary[(string)header.Key] = (string)header.Value;
			}
			LogDebugInfo(pURL, pForm.data, dictionary);
			return OpenURL(new WWW(pURL, pForm.data, dictionary));
		}

		public static Async<WWW> OpenURL(string pURL, JValue pJson)
		{
			string s = pJson.Format();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			return OpenURL(pURL, bytes, "application/json", null);
		}

		public static Async<WWW> OpenURL(string pURL, JValue pJson, Dictionary<string, string> pHeaders)
		{
			string s = pJson.Format();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			return OpenURL(pURL, bytes, "application/json", pHeaders);
		}

		public static Async<WWW> OpenURL(string pURL, byte[] pPayload, string pContentType)
		{
			return OpenURL(pURL, pPayload, pContentType, null);
		}

		public static Async<WWW> OpenURL(string pURL, byte[] pPayload, string pContentType, Dictionary<string, string> pExtraHeaders)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Content-Type"] = pContentType;
			if (pExtraHeaders != null)
			{
				foreach (KeyValuePair<string, string> pExtraHeader in pExtraHeaders)
				{
					dictionary[pExtraHeader.Key] = pExtraHeader.Value;
				}
			}
			LogDebugInfo(pURL, pPayload, dictionary);
			return OpenURL(new WWW(pURL, pPayload, dictionary));
		}

		private static Async<WWW> OpenURL(WWW pWWW)
		{
			return Timer.WaitFor(pWWW).Then((WWW www) =>
			{
				if (!string.IsNullOrEmpty(www.error))
				{
					Debug.LogError(string.Format("HTTP: {0} ({1})", www.error, www.url));
				}
				return www;
			});
		}

		public static string UrlDecode(this string pString)
		{
			byte[] array = new byte[pString.Length];
			int count = 0;
			for (int i = 0; i < pString.Length; i++)
			{
				if (pString[i] == '+')
				{
					array[count++] = 32;
				}
				else if (pString[i] == '%')
				{
					if (pString.Length < i + 1)
					{
						Debug.LogError("Bad urlencoded string");
						break;
					}
					string text = pString.Substring(i + 1, 2);
					i += 2;
					byte result;
					if (!byte.TryParse(text, NumberStyles.HexNumber, null, out result))
					{
						Debug.LogError("Bad urlencode entity: " + text);
						break;
					}
					array[count++] = result;
				}
				else
				{
					if (pString[i] >= '膧')
					{
						Debug.LogError("Character out of range");
						break;
					}
					array[count++] = (byte)pString[i];
				}
			}
			return Encoding.UTF8.GetString(array, 0, count);
		}

		private static void UrlEncode(string pString, StringBuilder pSB)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(pString);
			for (int i = 0; i < bytes.Length; i++)
			{
				if ("!*'();:@&=+$,/?#[]".IndexOf((char)bytes[i]) >= 0 || "abcdefghijklmnopqrstuvwxyzABCEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~".IndexOf((char)bytes[i]) < 0)
				{
					pSB.AppendFormat("%{0:x2}", bytes[i]);
				}
				else
				{
					pSB.Append((char)bytes[i]);
				}
			}
		}

		public static string UrlEncode(this string pString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			UrlEncode(pString, stringBuilder);
			return stringBuilder.ToString();
		}

		public static string UrlEncode(this Dictionary<string, string> pParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> pParameter in pParameters)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append("&");
				}
				UrlEncode(pParameter.Key, stringBuilder);
				stringBuilder.Append("=");
				UrlEncode(pParameter.Value, stringBuilder);
			}
			return stringBuilder.ToString();
		}

		public static Dictionary<string, string> URLDecodeParameters(string pParameters)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = pParameters.Split('&');
			foreach (string text in array)
			{
				string[] array2 = text.Split('=');
				dictionary.Add(array2[0].UrlDecode(), array2[1].UrlDecode());
			}
			return dictionary;
		}

		public static string MakeUrl(string pBaseURL, Dictionary<string, string> pParameters)
		{
			string text = pBaseURL;
			if (pParameters != null && pParameters.Count > 0)
			{
				text = text + "?" + pParameters.UrlEncode();
			}
			return text;
		}

		public static void OpenWebBrowser(string pUrl, bool pUseInternalIfPossible)
		{
			Application.OpenURL(pUrl);
		}

		public static string GetHeader(this WWW www, string header)
		{
			foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
			{
				if (responseHeader.Key.Equals(header, StringComparison.InvariantCultureIgnoreCase))
				{
					return responseHeader.Value;
				}
			}
			return null;
		}
	}
}
