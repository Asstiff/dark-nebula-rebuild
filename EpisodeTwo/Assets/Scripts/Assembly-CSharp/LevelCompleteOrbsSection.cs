using UnityEngine;

public class LevelCompleteOrbsSection : LevelCompleteSection
{
	private bool done;

	public override void Start()
	{
		base.Start();
		Title.GetComponent<SimpleText>().SetStringID("ORBS COLLECTED");
	}

	public override void Update()
	{
		base.Update();
		if (startTime != 0f && !done)
		{
			float num = Time.time - startTime;
			float num2 = Mathf.Min(1f, num / 1.6666666f);
			int num3 = Mathf.CeilToInt((float)LevelCompleteScreen.Result.OrbsCollected * num2);
			Value.GetComponent<SimpleText>().SetText(num3.ToString("D2") + "/" + LevelCompleteScreen.Result.OrbsOnLevel);
			if (num2 == 1f)
			{
				done = true;
			}
		}
	}
}
