using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatScreen : MonoBehaviour
{
	private const int ButtonsPerRow = 4;

	private const int HorizontalMargin = 20;

	private const int VerticalMargin = 40;

	public static bool InfiniteRetriesEnabled;

	private bool _showing;

	private Queue<string> _messages = new Queue<string>();

	private int _buttonCount;

	private int _lastButtonY;

	public event Action Hidden;

	public void Show()
	{
		_showing = true;
		_messages.Clear();
	}

	public void Hide()
	{
		_showing = false;
		if (Hidden != null)
		{
			Hidden();
		}
	}

	private void Message(string pText)
	{
		Debug.Log(pText);
		_messages.Enqueue(pText);
		if (_messages.Count > 8)
		{
			_messages.Dequeue();
		}
	}

	private bool Button(string pText)
	{
		int num = (Screen.width - 40) / 4;
		int num2 = (Screen.height - 80) / 10;
		int num3 = _buttonCount % 4 * num + 20;
		int num4 = _buttonCount / 4 * num2 + 40;
		_lastButtonY = num4 + num2 + 40;
		_buttonCount++;
		return GUI.Button(new Rect(num3, num4, num, num2), pText);
	}

	private void ErasePlayerProgress()
	{
		for (int i = 1; i <= 23; i++)
		{
			UserProgress.Instance.SetLevelStar(i, 0, false);
			UserProgress.Instance.SetLevelStar(i, 0, true);
			UserProgress.Instance.RemoveBestTime(i, false);
			UserProgress.Instance.RemoveBestTime(i, true);
		}
		BoosterManager.DeleteAllBoosters();
		UserPurchases.Instance.HasUnlockAll = false;
		UserProgress.Instance.ResetProgress();
		UserSettings.Instance.SetLastSelectedLevel(1, true);
		UserSettings.Instance.SetLastSelectedLevel(1, false);
		DarkNebulaAchievements.Instance.EraseProgress();
	}
}
