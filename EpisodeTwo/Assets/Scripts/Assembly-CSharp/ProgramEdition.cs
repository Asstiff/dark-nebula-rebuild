using System;
using FLD;

public static class ProgramEdition
{
	[Serializable]
	public enum Platforms
	{
		IOS_GENERIC = 0,
		ANDROID_GENERIC = 1,
		ANDROID_AMAZON = 2,
		STANDALONE_MAC = 3,
		STANDALONE_WIN = 4,
		STANDALONE_WEB = 5
	}

	private static Platforms _platform;

	public static Platforms platform
	{
		get
		{
			return _platform;
		}
		set
		{
			_platform = value;
			uDebug.Info("Platform set to: " + value);
		}
	}
}
