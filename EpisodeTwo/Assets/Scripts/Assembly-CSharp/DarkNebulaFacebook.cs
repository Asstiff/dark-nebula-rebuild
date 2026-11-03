using System;
using FLD;
using UnityEngine;

public class DarkNebulaFacebook
{
	private static string AppID = "268776953205134";

	private static bool _initialized;

	private static int Episode = 2;

	public static bool IsLoggedIn
	{
		get
		{
			Initalize();
			if (FLD.FacebookManager.Instance.IsAvailable)
			{
				return FLD.FacebookManager.Instance.HasPermission("publish_stream");
			}
			return false;
		}
	}

	private static void Initalize()
	{
		if (!_initialized)
		{
			_initialized = true;
			FLD.FacebookManager.Instance.Initialize(AppID, "user_birthday".Split(','), "publish_stream".Split(','));
		}
	}

	private static void Login(Action pCallback = null)
	{
		DarkNebulaMetrics.FacebookClicked();
		Debug.Log("Try login");
		FLD.FacebookManager.Instance.Authorize(() =>
		{
			if (IsSessionValid())
			{
				DarkNebulaMetrics.FacebookAuthorized();
				Debug.Log("Facebook: authorized");
				if (pCallback != null)
				{
					pCallback();
				}
			}
			else if (FLD.FacebookManager.Instance.HasBasicInformation)
			{
				DarkNebulaMetrics.FacebookPartiallyAuthorized();
				Debug.Log("Facebook: partially authorized");
				if (pCallback != null)
				{
					pCallback();
				}
			}
			else
			{
				DarkNebulaMetrics.FacebookDidNotAuthorize();
				Debug.Log("Facebook: NotAuthorized");
				if (pCallback != null)
				{
					pCallback();
				}
			}
		});
	}

	public static void Logout()
	{
		Initalize();
		DarkNebulaMetrics.FacebookLoggedout();
		FLD.FacebookManager.Instance.EndSession();
	}

	public static void AssertLoggedIn(Action pCallback = null)
	{
		Initalize();
		if (IsSessionValid())
		{
			if (pCallback != null)
			{
				pCallback();
			}
		}
		else
		{
			Login(pCallback);
		}
	}

	public static bool IsSessionValid()
	{
		return FLD.FacebookManager.Instance.HasPermission("publish_stream");
	}

	public static void PostCompletedEpisodeOneToStream(int pLevel, Action pCallback = null)
	{
		AssertLoggedIn(() =>
		{
			if (!IsSessionValid())
			{
				pCallback();
			}
			else
			{
				PostStream("has completed the first episode of Dark Nebula!", string.Empty, "completed_episode_01", pLevel, pCallback);
			}
		});
	}

	public static void PostCompletedEpisodeTwoToStream(int pLevel, Action pCallback = null)
	{
		AssertLoggedIn(() =>
		{
			if (!IsSessionValid())
			{
				pCallback();
			}
			else
			{
				PostStream("has completed the second episode of Dark Nebula!", string.Empty, "completed_episode_02", pLevel, pCallback);
			}
		});
	}

	public static void PostDemonToStream(int pLevel, Action pCallback = null)
	{
		AssertLoggedIn(() =>
		{
			if (!IsSessionValid())
			{
				pCallback();
			}
			else
			{
				PostStream("has become the Demon of Dark Nebula!", "Awarded when you complete all the levels with Expert Gold results.", "demon", pLevel, pCallback);
			}
		});
	}

	public static void PostTrueMasterToStream(int pLevel, Action pCallback = null)
	{
		AssertLoggedIn(() =>
		{
			if (!IsSessionValid())
			{
				pCallback();
			}
			else
			{
				PostStream("has become the True Master of Dark Nebula!", "Awarded when you complete all the levels with Gold star results.", "truemaster", pLevel, pCallback);
			}
		});
	}

	public static void PostLevelComplete(int pEpisode, int pLevel, float pTime, bool pExpert, StarValue pStar, Action pCallback = null)
	{
		int level = pLevel;
		Action callback = pCallback;
		AssertLoggedIn(() =>
		{
			if (!IsSessionValid())
			{
				pCallback();
			}
			else
			{
				string levelName = LoadLevelScreen.GetLevelName(level);
				string arg = LevelCompleteTimeSection.ToTimeString(pTime);
				string text = ((pStar == StarValue.Gold) ? "Gold" : ((pStar != StarValue.Silver) ? "Bronze" : "Silver"));
				text = ((!pExpert) ? ("a " + text) : ("an Expert " + text));
				string pCaption = string.Format("has completed Level '{0}' with {1} star!", levelName, text);
				string pDescription = string.Format("Completion Time: {0} ({1} difficulty)", arg, (!pExpert) ? "Normal" : "Expert");
				string pImageName = string.Format("level_{0}_{1}", pEpisode.ToString("D2"), level.ToString("D2"));
				PostStream(pCaption, pDescription, pImageName, pLevel, callback);
			}
		});
	}

	private static void PostStream(string pCaption, string pDescription, string pImageName, int pLevel, Action pCallback)
	{
		string pTitle = "Dark Nebula - Episode " + ((Episode != 1) ? "Two" : "One");
		string pCaption2 = "{*actor*} " + pCaption;
		string pPictureURL = string.Format("http://darknebula.leaderboard.1337gamedesign.com/external/dark_nebula_feed_{0}.jpg", pImageName);
		string pLinkURL = "http://www.freelunchdesign.com/redirect.php?id=darknebula-ep" + Episode;
		FLD.FacebookManager.Instance.ShowPostDialog(pTitle, pCaption2, pDescription, pLinkURL, pPictureURL);
		DarkNebulaMetrics.ShareFacebook(pLevel);
	}
}
