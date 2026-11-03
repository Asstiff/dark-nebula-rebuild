using System.Collections.Generic;
using FLD;
using Palringo;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
	[SerializeField]
	private FlipperButtonMultipleValues _viewTypeFlipper;

	[SerializeField]
	private LeaderboardView _leaderboardView;

	[SerializeField]
	private bool _useLargeScreen;

	[SerializeField]
	private DynamicFont _infoText;

	[SerializeField]
	private LeaderboardViewType[] _wantedViewTypes;

	private LeaderboardViewType _viewType = LeaderboardViewType.Daily;

	private bool _isExpert;

	private int _level = 1;

	private void Start()
	{
		HideInfoText();
		if (Application.loadedLevelName == "PostComplete")
		{
			_level = PostCompleteScreen.Result.LevelNum;
		}
		_isExpert = UserSettings.Instance.IsExpertMode;
		_leaderboardView.EventDifficultyChanged += DifficultyChanged;
		if (_useLargeScreen)
		{
			_leaderboardView.SetUseLargeView();
		}
		SetupLeaderboard();
	}

	public void SetupLeaderboard()
	{
		if (DarkNebulaPalringo.IsAuthorized)
		{
			_viewType = _wantedViewTypes[0];
			_viewTypeFlipper.gameObject.SetActive(true);
			string[] array = new string[_wantedViewTypes.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = _wantedViewTypes[i].ToString().ToUpper().Localize();
			}
			_viewTypeFlipper.Init(0, array, "VIEW TYPE");
			_viewTypeFlipper.Title.gameObject.SetActive(false);
			_viewTypeFlipper.EventValueChanged += ViewTypeChanged;
		}
		else
		{
			_viewTypeFlipper.gameObject.SetActive(false);
		}
	}

	public void UpdateLeaderboard()
	{
		HideInfoText();
		if (DarkNebulaPalringo.IsAuthorized)
		{
			UpdateGlobalLeaderboard();
		}
		else
		{
			UpdateLocalLeaderboard();
		}
	}

	private void ShowInfoText(string pText)
	{
		_infoText.gameObject.SetActive(true);
		_infoText.Text = pText;
	}

	private void HideInfoText()
	{
		_infoText.gameObject.SetActive(false);
	}

	private void UpdateGlobalLeaderboard()
	{
		_leaderboardView.ShowLoadScreen(_isExpert);
		if (_level == 11 || _level == 23)
		{
			ShowInfoText("CORE LEVEL INFO".Localize());
			_leaderboardView.SetList(new List<LeaderboardEntryVO>(), _isExpert);
			_leaderboardView.ShowLoadScreen(_isExpert, false);
			return;
		}
		if (_level == 0 && !UserProgress.Instance.IsTotalTimeUnlocked(_isExpert))
		{
			ShowInfoText("NO TOTAL SCORE INFO".Localize());
		}
		string leaderboardID = GetLeaderboardID(_level, _isExpert);
		Leaderboard globalLeaderboard = DarkNebulaPalringo.Connect.GetLeaderboard(leaderboardID);
		globalLeaderboard.GetScores(_viewType, 0, 50).Wait((List<Palringo.LeaderboardEntry> globalEntries) =>
		{
			Debug.Log("Received " + globalEntries.Count + " entries");
			if ((globalEntries.Count > 0 && (globalEntries[0].View != _viewType || LeaderboardEntry.GetLevelFromFromLeaderboardEntry(globalEntries[0]) != _level)) || _leaderboardView == null)
			{
				Debug.Log("User has moved away from requested view when reply was received, disregard result");
			}
			else if (globalEntries.Count <= 0)
			{
				_leaderboardView.GotoEntry(default(LeaderboardEntryVO));
				_leaderboardView.ShowLoadScreen(_isExpert, false);
			}
			else
			{
				List<LeaderboardEntryVO> leaderboardEntries = new List<LeaderboardEntryVO>();
				foreach (Palringo.LeaderboardEntry globalEntry in globalEntries)
				{
					LeaderboardEntryVO entryFromGlobalEntry = LeaderboardEntry.GetEntryFromGlobalEntry(globalEntry);
					leaderboardEntries.Add(entryFromGlobalEntry);
				}
				globalLeaderboard.GetLocalPlayer(_viewType).Wait((Palringo.LeaderboardEntry pLocalPlayerEntry) =>
				{
					if ((globalEntries.Count > 0 && (globalEntries[0].View != _viewType || LeaderboardEntry.GetLevelFromFromLeaderboardEntry(globalEntries[0]) != _level)) || _leaderboardView == null)
					{
						Debug.Log("User has moved away from requested view when reply was received, disregard result");
					}
					else
					{
						LeaderboardEntryVO leaderboardEntryVO = default(LeaderboardEntryVO);
						if (pLocalPlayerEntry != null)
						{
							leaderboardEntryVO = LeaderboardEntry.GetEntryFromGlobalEntry(pLocalPlayerEntry);
							if (globalEntries.Find((Palringo.LeaderboardEntry x) => x.User.UserId == pLocalPlayerEntry.User.UserId) == null)
							{
								leaderboardEntries.Add(leaderboardEntryVO);
							}
						}
						_leaderboardView.SetList(leaderboardEntries, _isExpert);
						_leaderboardView.GotoEntry(leaderboardEntryVO);
						_leaderboardView.ShowLoadScreen(_isExpert, false);
					}
				});
			}
		});
	}

	private void UpdateLocalLeaderboard()
	{
		if (_level == 11 || _level == 23)
		{
			Debug.Log("viewing core level - nothing to see here");
			ShowInfoText("CORE LEVEL INFO".Localize());
			_leaderboardView.SetList(new List<LeaderboardEntryVO>(), _isExpert);
			return;
		}
		if (_level == 0)
		{
			if (UserProgress.Instance.IsTotalTimeUnlocked(_isExpert))
			{
				List<LeaderboardEntryVO> list = new List<LeaderboardEntryVO>();
				list.Add(LeaderboardEntry.GetTotalTimeEntryForLocalUser(_isExpert));
				_leaderboardView.SetList(list, _isExpert);
			}
			else
			{
				_leaderboardView.SetList(new List<LeaderboardEntryVO>(), _isExpert);
				Timer.Delay(0.01f, () =>
				{
					ShowInfoText("NO TOTAL SCORE INFO".Localize());
				});
			}
			return;
		}
		List<LeaderboardEntryVO> list2 = new LocalLeaderboard().GetList(_isExpert, _level);
		if (list2.Count == 0) ShowInfoText("COMPLETE THIS LEVEL TO SAVE A LOCAL TIME.");
		_leaderboardView.SetList(list2, _isExpert);
		_leaderboardView.ShowLoadScreen(_isExpert, false);
	}

	private void ViewTypeChanged(int pViewTypeValue)
	{
		_viewType = _wantedViewTypes[pViewTypeValue];
		UpdateLeaderboard();
	}

	public void ShowLeaderboard(bool pShow = true)
	{
		_leaderboardView.gameObject.SetActive(pShow);
	}

	private void DifficultyChanged(bool pExpert)
	{
		_isExpert = pExpert;
		UpdateLeaderboard();
	}

	private void ShowLoadScreen()
	{
		_leaderboardView.ShowLoadScreen(_isExpert);
		_leaderboardView.SetList(new List<LeaderboardEntryVO>(), _isExpert);
	}

	private void HideLoadScreen()
	{
		_leaderboardView.ShowLoadScreen(_isExpert, false);
	}

	private void OnDisable()
	{
		_leaderboardView.EventDifficultyChanged -= DifficultyChanged;
		_leaderboardView = null;
	}

	public void SetLevel(int pLevel)
	{
		_level = pLevel;
		UpdateLeaderboard();
	}

	public static string GetLeaderboardID(int pLevel, bool pIsExpert)
	{
		string empty = string.Empty;
		if (pLevel == 0)
		{
			return string.Format("{0}/{1}", "total", (!pIsExpert) ? "easy" : "hard");
		}
		return string.Format("level{0}/{1}", pLevel.ToString(), (!pIsExpert) ? "easy" : "hard");
	}
}
