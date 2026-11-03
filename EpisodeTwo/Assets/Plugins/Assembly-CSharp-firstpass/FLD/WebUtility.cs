using UnityEngine;

namespace FLD
{
	public static class WebUtility
	{
		public static void OpenURL(string url, bool useInternalIfPossible)
		{
			if (useInternalIfPossible)
			{
				EtceteraAndroid.showWebView(url);
			}
			else
			{
				Application.OpenURL(url);
			}
		}
	}
}
