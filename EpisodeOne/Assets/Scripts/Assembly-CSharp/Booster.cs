public class Booster : Product
{
	public enum BoosterType
	{
		SPEED = 0,
		SHIELD = 1,
		MAGNET = 2
	}

	public BoosterType Type;

	public string EnabledIconID;

	public string DisabledIconID;

	public string LockedIconID;

	public float MagnetMaxSpeed = 400f;

	public float MagnetDistance = 160f;

	public float SpeedBonus = 1.28f;

	public override string ID
	{
		get
		{
			string text = string.Empty;
			if (UserSettings.Instance.IsExpertMode)
			{
				text = "Expert";
			}
			return string.Concat(text, Type, LoadLevelScreen.LevelToLoad.ToString());
		}
	}

	public static string GetIdForBooster(BoosterType pType, int pLevel, bool pExpert)
	{
		string text = string.Empty;
		if (pExpert)
		{
			text = "Expert";
		}
		return string.Concat(text, pType, pLevel.ToString());
	}

	public static string GetIdForCurrentDifficulty(BoosterType pType, int pLevel)
	{
		string text = string.Empty;
		if (UserSettings.Instance.IsExpertMode)
		{
			text = "Expert";
		}
		return string.Concat(text, pType, pLevel.ToString());
	}
}
