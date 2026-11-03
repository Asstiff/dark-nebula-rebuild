using Random = UnityEngine.Random;
using System;
using UnityEngine;

namespace FLD
{
	public static class DeviceInfo
	{
		private static string _macAddress;

		private static string _imei;

		private static string _androidId;

		private static AndroidJavaObject _currentActivity;

		public static string UniqueIdentifier
		{
			get
			{
				if (PlayerPrefs.HasKey("UniqueDeviceIdentifier"))
				{
					return PlayerPrefs.GetString("UniqueDeviceIdentifier");
				}
				string text = ((int)(Random.value * 9f + 1f)).ToString();
				for (int i = 0; i < 8; i++)
				{
					text += (int)(Random.value * 10f);
				}
				PlayerPrefs.SetString("UniqueDeviceIdentifier", text);
				return text;
			}
		}

		public static string MACAddress
		{
			get
			{
				if (_macAddress == null)
				{
					_macAddress = string.Empty;
					try
					{
						using (AndroidJavaObject androidJavaObject = GetAndroidService("wifi"))
						{
							if (androidJavaObject != null)
							{
								_macAddress = androidJavaObject.Call<AndroidJavaObject>("getConnectionInfo", new object[0]).Call<string>("getMacAddress", new object[0]);
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to fetch MAC address: " + ex.Message);
					}
					if (_macAddress == null)
					{
						_macAddress = string.Empty;
					}
				}
				return _macAddress;
			}
		}

		public static string IMEI
		{
			get
			{
				if (_imei == null)
				{
					_imei = string.Empty;
					try
					{
						using (AndroidJavaObject androidJavaObject = GetAndroidService("phone"))
						{
							if (androidJavaObject != null)
							{
								_imei = androidJavaObject.Call<string>("getDeviceId", new object[0]);
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to fetch IMEI: " + ex.Message);
					}
					if (_imei == null)
					{
						_imei = string.Empty;
					}
				}
				return _imei;
			}
		}

		public static string AndroidId
		{
			get
			{
				if (_androidId == null)
				{
					_androidId = string.Empty;
					try
					{
						using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.provider.Settings$Secure"))
						{
							AndroidJavaObject androidContext = GetAndroidContext();
							if (androidContext != null && androidJavaClass != null)
							{
								_androidId = androidJavaClass.CallStatic<string>("getString", new object[2]
								{
									androidContext.Call<AndroidJavaObject>("getContentResolver", new object[0]),
									androidJavaClass.GetStatic<string>("ANDROID_ID")
								});
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Failed to fetch Android ID: " + ex.Message);
					}
					if (_androidId == null)
					{
						_androidId = string.Empty;
					}
				}
				return _androidId;
			}
		}

		public static bool IsDeviceTablet
		{
			get
			{
				float num = 0f;
				if (Screen.dpi > 0f)
				{
					float num2 = Mathf.Max(Screen.width, Screen.height);
					num = num2 / Screen.dpi;
					if (num >= 5f)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static bool IsDeviceDesktop
		{
			get
			{
				if (Application.isEditor)
				{
					return true;
				}
				switch (Application.platform)
				{
				case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.LinuxPlayer:
					return true;
				default:
					return false;
				}
			}
		}

		public static bool IsTouchSupported
		{
			get
			{
				return true;
			}
		}

		public static bool IsTiltSupported
		{
			get
			{
				if (Application.isEditor)
				{
					return true;
				}
				switch (Application.platform)
				{
				case RuntimePlatform.IPhonePlayer:
				case RuntimePlatform.Android:
					return true;
				default:
					return false;
				}
			}
		}

		public static VersionNumber OSXVersion
		{
			get
			{
				return default(VersionNumber);
			}
		}

		private static AndroidJavaObject GetAndroidContext()
		{
			if (Application.isEditor)
			{
				return null;
			}
			if (_currentActivity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				if (androidJavaClass != null)
				{
					_currentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}
			return _currentActivity;
		}

		private static AndroidJavaObject GetAndroidService(string pId)
		{
			if (Application.isEditor)
			{
				return null;
			}
			AndroidJavaObject androidContext = GetAndroidContext();
			if (androidContext != null)
			{
				return androidContext.Call<AndroidJavaObject>("getSystemService", new object[1] { pId });
			}
			return null;
		}
	}
}
