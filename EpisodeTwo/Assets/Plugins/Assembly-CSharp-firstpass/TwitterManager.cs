using System;
using Prime31;
using UnityEngine;

public class TwitterManager : MonoBehaviour
{
	public static event Action<string> loginSucceededEvent;

	public static event Action<string> loginFailedEvent;

	public static event Action<object> requestDidFinishEvent;

	public static event Action<string> requestDidFailEvent;

	public static event Action<bool> tweetSheetCompletedEvent;

	static TwitterManager()
	{
		AbstractManager.initialize(typeof(TwitterManager));
	}

	public static void noop()
	{
	}

	public void loginSucceeded(string screenname)
	{
		if (loginSucceededEvent != null)
		{
			loginSucceededEvent(screenname);
		}
	}

	public void loginFailed(string error)
	{
		if (loginFailedEvent != null)
		{
			loginFailedEvent(error);
		}
	}

	public void requestSucceeded(string results)
	{
		if (requestDidFinishEvent != null)
		{
			requestDidFinishEvent(Json.decode(results));
		}
	}

	public void requestFailed(string error)
	{
		if (requestDidFailEvent != null)
		{
			requestDidFailEvent(error);
		}
	}

	public void tweetSheetCompleted(string oneOrZero)
	{
		if (tweetSheetCompletedEvent != null)
		{
			tweetSheetCompletedEvent(oneOrZero == "1");
		}
	}
}
