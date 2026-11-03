using UnityEngine;

public class VungleAndroid
{
	private static AndroidJavaObject _plugin;

	static VungleAndroid()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.prime31.VunglePlugin"))
		{
			_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
		}
	}

	public static void init(string appId)
	{
		init(appId, -1, VungleGender.None);
	}

	public static void init(string appId, int age, VungleGender gender)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("init", appId, age, (int)gender);
		}
	}

	public static void onPause()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("onPause");
		}
	}

	public static void onResume()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("onResume");
		}
	}

	public static bool isVideoAvailable()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return _plugin.Call<bool>("isVideoAvailable", new object[0]);
	}

	public static void setSoundEnabled(bool isEnabled)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setSoundEnabled", isEnabled);
		}
	}

	public static bool isSoundEnabled()
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return true;
		}
		return _plugin.Call<bool>("isSoundEnabled", new object[0]);
	}

	public static void displayAdvert()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("displayAdvert");
		}
	}

	public static void displayIncentivizedAdvert(bool showCloseButton)
	{
		displayIncentivizedAdvert(showCloseButton, string.Empty);
	}

	public static void displayIncentivizedAdvert(bool showCloseButton, string name)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("displayIncentivizedAdvert", showCloseButton, name);
		}
	}
}
