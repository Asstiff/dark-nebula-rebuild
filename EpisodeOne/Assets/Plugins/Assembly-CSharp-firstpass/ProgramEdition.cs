using System;
using UnityEngine;

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
		STANDALONE_WEB = 5,
		BB10 = 6
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
			Debug.Log("Platform set to: " + value);
		}
	}
}
