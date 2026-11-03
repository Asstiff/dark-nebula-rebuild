using UnityEngine;

public class SleepTimeoutManager : MonoBehaviour
{
	public enum OverrideAction
	{
		None = 0,
		NeverSleep = 1
	}

	public static OverrideAction Override;

	private static SleepTimeoutManager _instance;

	public string SleepMode;

	private static SleepTimeoutManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("!SleepTimeoutManager");
				Object.DontDestroyOnLoad(gameObject);
				_instance = gameObject.AddComponent<SleepTimeoutManager>();
			}
			return _instance;
		}
	}

	public static void NeverSleep()
	{
		Instance._neverSleep();
	}

	public static void AllowSleepIn(float pSeconds = 60f)
	{
		if (Override == OverrideAction.NeverSleep)
		{
			Instance._neverSleep();
		}
		else
		{
			Instance._allowSleepIn(pSeconds);
		}
	}

	private void _neverSleep()
	{
		if (Instance != null)
		{
			Instance.CancelInvokeRealtime("_allowSleep");
		}
		Screen.sleepTimeout = -1;
		SleepMode = "NeverSleep";
		Debug.Log(SleepMode);
	}

	private void _allowSleepIn(float pSeconds)
	{
		Instance.CancelInvokeRealtime("_allowSleep");
		Instance.InvokeRealtime("_allowSleep", pSeconds);
		SleepMode = "Sleep in " + pSeconds;
		Debug.Log(SleepMode);
	}

	private void _allowSleep()
	{
		Screen.sleepTimeout = -2;
		SleepMode = "SystemSetting";
		Debug.Log(SleepMode);
	}
}
