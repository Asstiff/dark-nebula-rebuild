using FLD;
using UnityEngine;

public class EndShip : Entity
{
	public AudioClip Sfx;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			return;
		}
		Audio.Play("endlevel", Sfx);
		World.vessel.AllowedToMove = false;
		World.vessel.transform.position = base.transform.position + new Vector3(128f, 0f, -32f);
		Audio.PauseMusic(3.3333333f);
		if (World.levelNum == 23)
		{
			if (!UserProgress.Instance.HaveCompletedLevel(World.levelNum, UserSettings.Instance.IsExpertMode))
			{
				DarkNebulaMetrics.LevelProgression(Mathf.Max(23, World.levelNum + 1), UserSettings.Instance.IsExpertMode);
			}
			UserProgress.Instance.SetLevelStar(World.levelNum, 1, UserSettings.Instance.IsExpertMode);
			World.LoadLevel("EndOfGame", 3.3333333f);
		}
		else if (World.levelNum == 11)
		{
			if (!UserProgress.Instance.HaveCompletedLevel(World.levelNum, UserSettings.Instance.IsExpertMode))
			{
				DarkNebulaMetrics.LevelProgression(Mathf.Max(23, World.levelNum + 1), UserSettings.Instance.IsExpertMode);
			}
			UserProgress.Instance.SetLevelStar(World.levelNum, 1, UserSettings.Instance.IsExpertMode);
			UserSettings.Instance.SetLastSelectedLevel(World.levelNum, UserSettings.Instance.IsExpertMode);
			World.LoadLevel("EndOfChapterOne", 3.3333333f);
		}
		else
		{
			World.TriggerEndOfLevel();
			World.LoadLevel("LevelComplete", 0.5f);
		}
		if (UserProgress.Instance.IsTotalTimeUnlocked(UserSettings.Instance.IsExpertMode))
		{
			LevelCompleteScreen.SubmitTotalScoreToGlobalLeaderboard();
		}
		else
		{
			Debug.Log("User have not completed all levels on this difficulty. Will not submit total score to palringo");
		}
		DarkNebulaMetrics.LevelCompleted(World.levelNum, -1f, UserSettings.Instance.IsExpertMode, !UserProgress.Instance.HaveCompletedLevel(World.levelNum, UserSettings.Instance.IsExpertMode), AvatarManager.Instance.ActiveAvatar.Name);
	}
}
