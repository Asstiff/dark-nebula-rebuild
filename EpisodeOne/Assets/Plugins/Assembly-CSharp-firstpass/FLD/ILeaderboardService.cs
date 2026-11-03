using System.Collections.Generic;

namespace FLD
{
	[Service("Leaderboards", Description = "Used to access leaderboards, i.e. high scores, time records, etc.")]
	public interface ILeaderboardService : IService
	{
		bool SupportsExternalDialog { get; }

		bool SupportsPerScoreNames { get; }

		bool SupportsPerLevelLeaderboards { get; }

		string LocalPlayerName { get; set; }

		void OpenExternalDialog();

		void OpenExternalDialog(string pBoardId);

		Async<List<LeaderboardEntry>> GetScores(string pBoardId, int pCount);

		Async<List<LeaderboardEntry>> GetScores(string pBoardId, int pOffset, int pCount);

		Async<List<LeaderboardEntry>> GetScoresForLevel(string pBoardId, int pLevel, int pCount);

		Async<List<LeaderboardEntry>> GetScoresForLevel(string pBoardId, int pLevel, int pOffset, int pCount);

		Async<int> SubmitScore(string pBoardId, int pScore);

		Async<int> SubmitScoreForLevel(string pBoardId, int pLevel, int pScore);

		Async<int> GetLocalPlayerBestPosition(string pBoardId);

		Async<int> GetLocalPlayerBestPositionForLevel(string pBoardId, int pLevel);
	}
}
