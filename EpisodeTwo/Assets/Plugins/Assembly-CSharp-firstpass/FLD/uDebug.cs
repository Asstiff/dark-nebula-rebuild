using UnityEngine;

namespace FLD
{
	public static class uDebug
	{
		public enum Level
		{
			CRITICAL = 0,
			ERROR = 1,
			WARNING = 2,
			INFO = 3,
			DEBUG = 4
		}

		public static void SetLevel(Level pLevel)
		{
		}

		public static void Warning(string text, params object[] pParam)
		{
			Log(Level.WARNING, text, pParam);
		}

		public static void Info(string text, params object[] pParam)
		{
			Log(Level.INFO, text, pParam);
		}

		public static void Error(string text, params object[] pParam)
		{
			Log(Level.ERROR, text, pParam);
		}

		public static void Log(Level level, string text, params object[] pParameters)
		{
			Debug.Log(string.Format("({0}) {1}", level, string.Format(text, pParameters)));
		}
	}
}
