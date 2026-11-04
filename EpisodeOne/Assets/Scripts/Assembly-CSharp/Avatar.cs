public class Avatar : Product
{
	public const int NormalSpriteID = 24;

	public const int ExpertSpriteID = 30;

	public const int MatrixSpriteID = 29;

	public const int EnergyRedSpriteID = 26;

	public const int EnergyBlueSpriteID = 27;

	public const int EnergyGreenSpriteID = 28;

	public bool DefaultAvatar;

	public bool Owned
	{
		get
		{
			if (DefaultAvatar)
			{
				return true;
			}
			return AvatarUnlocks.IsUnlocked(ID);
		}
	}
}
