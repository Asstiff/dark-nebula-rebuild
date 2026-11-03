public class LevelCompletePlayerSection : LevelCompleteSection
{
	private bool done;

	public override void Start()
	{
		base.Start();
		Title.GetComponent<SimpleText>().SetStringID("PLAYER");
		Value.SetActive(false);
		animationDuration -= starAnimationBeginTime;
		starAnimationBeginTime = 0f;
	}

	public override void Update()
	{
		base.Update();
		if (startTime != 0f && !done)
		{
			Value.SetActive(true);
			Value.GetComponent<UnityEngine.Renderer>().enabled = true;
			Value.GetComponent<SimpleText>().SetText(GetResultText());
			done = true;
		}
	}

	private string GetResultText()
	{
		if (LevelCompleteScreen.Result.PlayerLivesLost > 0)
		{
			if (LevelCompleteScreen.Result.PlayerLivesLost == 1)
			{
				return Language.Get("%d LIFE LOST", 1);
			}
			return Language.Get("%d LIVES LOST", LevelCompleteScreen.Result.PlayerLivesLost);
		}
		if (LevelCompleteScreen.Result.PlayerLostShields > 0)
		{
			if (LevelCompleteScreen.Result.PlayerLostShields == 1)
			{
				return Language.Get("%d SHIELD LOST", 1);
			}
			return Language.Get("%d SHIELDS LOST", LevelCompleteScreen.Result.PlayerLostShields);
		}
		return Language.Get("NO SHIELD LOST");
	}
}
