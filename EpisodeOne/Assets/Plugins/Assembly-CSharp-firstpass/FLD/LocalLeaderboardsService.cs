using System.Collections.Generic;
using FLD.JSON;

namespace FLD
{
	[Service("Local Leaderboards", Description = "Provides local leaderboards stored on the running device.")]
	public class LocalLeaderboardsService : IService, ILeaderboardService
	{
		private class Leaderboard
		{
			public List<LeaderboardEntry> Entries = new List<LeaderboardEntry>();
		}

		private string _localPlayerName;

		private Dictionary<string, Leaderboard> _leaderboards = new Dictionary<string, Leaderboard>();

		private PersistentStore _storage = PersistentStore.GetInstance("LbLocal");

		[ServiceConfigurable]
		public string DefaultPlayerName { get; set; }

		public bool SupportsPerScoreNames
		{
			get
			{
				return true;
			}
		}

		public string LocalPlayerName
		{
			get
			{
				return _localPlayerName;
			}
			set
			{
				_localPlayerName = value;
			}
		}

		public bool SupportsExternalDialog
		{
			get
			{
				return false;
			}
		}

		public bool SupportsPerLevelLeaderboards
		{
			get
			{
				return true;
			}
		}

		private void Save()
		{
			foreach (KeyValuePair<string, Leaderboard> leaderboard in _leaderboards)
			{
				JValue jValue = JValue.CreateArray();
				foreach (LeaderboardEntry entry in leaderboard.Value.Entries)
				{
					jValue.Add(new JValue
					{
						{ "name", entry.UserName },
						{ "score", entry.Score }
					});
				}
				_storage.Set(leaderboard.Key, jValue);
			}
			_storage.Save();
		}

		private void Load()
		{
			foreach (string key in _storage.Keys)
			{
				JValue json = _storage.GetJson(key);
				if (json == null)
				{
					continue;
				}
				Leaderboard leaderboard = new Leaderboard();
				foreach (JValue item in json)
				{
					List<LeaderboardEntry> entries = leaderboard.Entries;
					LeaderboardEntry leaderboardEntry = new LeaderboardEntry
					{
						UserId = item["name"].StringValue,
						UserName = item["name"].StringValue
					};
					int? intValue = item["score"].IntValue;
					leaderboardEntry.Score = (intValue.HasValue ? intValue.Value : 0);
					entries.Add(leaderboardEntry);
				}
				_leaderboards.Add(key, leaderboard);
			}
		}

		public Async<bool> Initialize()
		{
			_localPlayerName = DefaultPlayerName;
			Load();
			return Async.MakePrecompleted(true);
		}

		public Async<List<LeaderboardEntry>> GetScores(string pBoardId, int pCount)
		{
			return GetScores(pBoardId, 0, pCount);
		}

		public Async<List<LeaderboardEntry>> GetScores(string pBoardId, int pOffset, int pCount)
		{
			return GetScoresForLevel(pBoardId, 0, pOffset, pCount);
		}

		public Async<List<LeaderboardEntry>> GetScoresForLevel(string pBoardId, int pLevel, int pCount)
		{
			return GetScoresForLevel(pBoardId, pLevel, 0, pCount);
		}

		public Async<List<LeaderboardEntry>> GetScoresForLevel(string pBoardId, int pLevel, int pOffset, int pCount)
		{
			Leaderboard leaderboard = GetLeaderboard(pBoardId, pLevel);
			return Async.MakePrecompleted(leaderboard.Entries.Slice(pOffset, pCount).ToList());
		}

		public void OpenExternalDialog()
		{
		}

		public void OpenExternalDialog(string pBoardId)
		{
		}

		private Leaderboard GetLeaderboard(string pId, int pLevel)
		{
			string key = pId + "." + pLevel;
			Leaderboard value;
			if (!_leaderboards.TryGetValue(key, out value))
			{
				value = new Leaderboard();
				_leaderboards.Add(key, value);
			}
			return value;
		}

		public Async<int> SubmitScore(string pBoardId, int pScore)
		{
			return SubmitScoreForLevel(pBoardId, 0, pScore);
		}

		public Async<int> SubmitScoreForLevel(string pBoardId, int pLevel, int pScore)
		{
			Leaderboard leaderboard = GetLeaderboard(pBoardId, pLevel);
			LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
			leaderboardEntry.UserId = _localPlayerName;
			leaderboardEntry.UserName = _localPlayerName;
			leaderboardEntry.Score = pScore;
			LeaderboardEntry item = leaderboardEntry;
			leaderboard.Entries.Add(item);
			leaderboard.Entries.Sort((LeaderboardEntry a, LeaderboardEntry b) => b.Score.CompareTo(a.Score));
			int pResult = leaderboard.Entries.IndexOf(item);
			Save();
			return Async.MakePrecompleted(pResult);
		}

		public Async<int> GetLocalPlayerBestPosition(string pBoardId)
		{
			return GetLocalPlayerBestPositionForLevel(pBoardId, 0);
		}

		public Async<int> GetLocalPlayerBestPositionForLevel(string pBoardId, int pLevel)
		{
			Leaderboard leaderboard = GetLeaderboard(pBoardId, pLevel);
			for (int i = 0; i < leaderboard.Entries.Count; i++)
			{
				if (leaderboard.Entries[i].UserId == _localPlayerName)
				{
					Async.MakePrecompleted(i);
				}
			}
			return Async.MakePrecompleted(-1);
		}
	}
}
