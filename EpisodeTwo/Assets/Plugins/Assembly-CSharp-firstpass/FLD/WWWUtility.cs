using System.Collections;
using UnityEngine;

namespace FLD
{
	public static class WWWUtility
	{
		public delegate void URLHandler(bool pSuccess, WWW pWWW);

		public static void OpenURL(string pURL, URLHandler pCallback)
		{
			OpenURL(new WWW(pURL), pCallback);
		}

		public static void OpenURL(string pURL, WWWForm pForm, URLHandler pCallback)
		{
			OpenURL(new WWW(pURL, pForm), pCallback);
		}

		private static void OpenURL(WWW pWWW, URLHandler pCallback)
		{
			uFuga.Instance.StartCoroutine(OpenURLCoroutine(pWWW, pCallback));
		}

		private static IEnumerator OpenURLCoroutine(WWW pWWW, URLHandler pCallback)
		{
			yield return pWWW;
			if (pCallback != null)
			{
				if (!string.IsNullOrEmpty(pWWW.error))
				{
					Debug.LogError(string.Format("Error opening URL ({0}): {1}", pWWW.url, pWWW.error));
					pCallback(false, pWWW);
				}
				else
				{
					pCallback(true, pWWW);
				}
			}
		}
	}
}
