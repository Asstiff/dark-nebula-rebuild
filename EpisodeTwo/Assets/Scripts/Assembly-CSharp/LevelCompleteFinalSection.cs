using UnityEngine;

public class LevelCompleteFinalSection : LevelCompleteSection
{
	public GameObject ClickToChange;

	private bool done;

	public override void Start()
	{
		base.Start();
		Title.GetComponent<SimpleText>().SetStringID("LEADERBOARD NAME");
		ClickToChange.GetComponent<SimpleText>().SetStringID("(CLICK TO CHANGE)");
		animationDuration -= starAnimationBeginTime;
		starAnimationBeginTime = 0f;
	}

	public override void Update()
	{
		base.Update();
		if (startTime != 0f && !done)
		{
			Value.GetComponent<SimpleText>().SetText(GetResultText());
			done = true;
		}
	}

	private string GetResultText()
	{
		string text = ((!UserSettings.I.expert) ? string.Empty : "EXPERT ");
		switch (StarResult)
		{
		case StarValue.Silver:
			return Language.Get(text + "SILVER STAR!");
		case StarValue.Gold:
			return Language.Get(text + "GOLD STAR!");
		default:
			return Language.Get(text + "BRONZE STAR!");
		}
	}
}
