public class SkillMeter
{
	private const float SKILL_INC = 0.5f;

	private const float SKILL_DEC = -3f;

	private const int MIN_SKILL = -2;

	private const int MAX_SKILL = 5;

	private const int START_SKILL = 0;

	private const float NEGATIVE_DIFFICULTY = 0.35f;

	private const float POSITIVE_DIFFICULTY = 0.5f;

	private int minSkill;

	private int maxSkill;

	private float skill;

	public void Init()
	{
		if (UserSettings.Instance.IsExpertMode)
		{
			minSkill = 5;
			skill = 5f;
		}
		else
		{
			minSkill = -2;
			skill = 0f;
		}
		maxSkill = 5;
		SetSkillOnWorld();
	}

	public void NotifyOfDeath()
	{
		skill += -3f;
		if (skill < (float)minSkill)
		{
			skill = minSkill;
		}
		SetSkillOnWorld();
	}

	public void NotifyOfCheckpoint()
	{
		skill += 0.5f;
		if (skill > (float)maxSkill)
		{
			skill = maxSkill;
		}
		SetSkillOnWorld();
	}

	private void SetSkillOnWorld()
	{
		float num;
		if (skill <= 0f)
		{
			num = skill / (float)minSkill;
			num = 1f - num * 0.35f;
		}
		else
		{
			num = skill / (float)maxSkill;
			num = 1f + num * 0.5f;
		}
		World.SetDifficultyTo(num);
	}
}
