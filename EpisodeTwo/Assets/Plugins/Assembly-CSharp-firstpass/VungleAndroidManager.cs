using System;
using UnityEngine;

public class VungleAndroidManager : MonoBehaviour
{
	public static event Action onVungleAdStartEvent;

	public static event Action onVungleAdEndEvent;

	public static event Action<double, double> onVungleViewEvent;

	public static event Action<bool> onPauseChanged;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationPause(bool pause)
	{
		if (onPauseChanged != null)
		{
			onPauseChanged(pause);
		}
	}

	private void OnApplicationEnd()
	{
		onPauseChanged(false);
	}

	public void onVungleAdStart(string empty)
	{
		if (onVungleAdStartEvent != null)
		{
			onVungleAdStartEvent();
		}
	}

	public void onVungleAdEnd(string empty)
	{
		if (onVungleAdEndEvent != null)
		{
			onVungleAdEndEvent();
		}
	}

	public void onVungleView(string str)
	{
		if (onVungleViewEvent != null)
		{
			string[] array = str.Split('-');
			if (array.Length == 2)
			{
				onVungleViewEvent(double.Parse(array[0]), double.Parse(array[1]));
			}
		}
	}
}
