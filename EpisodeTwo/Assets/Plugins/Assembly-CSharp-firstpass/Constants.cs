using System;

public class Constants
{
	public const int ColliderHeight = 256;

	public const int LayerDefault = 0;

	public const int LayerSideWalls = 8;

	public const int LayerVessel = 9;

	public const int LayerBullets = 10;

	public const int LayerImpenetratableWall = 16;

	public const int LayerBoostShot = 13;

	public const int LayerAbyss = 15;

	public const float MusicFadeTime = 1.5f;

	public const float MusicVolume = 0.6f;

	public const float ScreenFadeInTime = 0.5f;

	public const float ScreenFadeOutTime = 0.5f;

	public const int VirtualScreenHeight = 480;

	public const int VirtualScreenHeightHalf = 240;

	public const int VirtualScreenWidth = 320;

	public const int VirtualScreenWidthHalf = 160;

	public const int VisibleLevelCount = 20;

	public const float PI = (float)Math.PI;

	public const float TargetFPS = 30f;

	public const float LayerDistance = 5f;

	public const string TagPlayer = "Player";

	public const string TagAbyss = "Abyss";

	public const string TagBullet = "Bullet";

	public const string TagPuck = "Puck";

	public const string TagElectroPuck = "ElectroPuck";

	public const string TagTurret = "Turret";

	public const string TagCanBeBorded = "CanBeBorded";

	public const string CollisionTypeAbyss = "abysscollision";

	public const string CollisionTypeBox = "boxcollision";

	public const string CollisionTypeCircle = "circlecollision";

	public const string CollisionTypeRedEnergy = "redenergysourcecollision";

	public const string CollisionTypeGreenEnergy = "greenenergysourcecollision";

	public const string CollisionTypeBlueEnergy = "blueenergysourcecollision";

	public const string CollisionTypeDeath = "deathcollision";

	public const int FLAG_CAMERA_SHAKE = 100;

	public const int FLAG_CAMERA_SHAKE_BIG = 101;

	public const int FLAG_SCREEN_WHITEOUT = 102;

	public const int FLAG_SCREEN_WHITEOUT_LONG = 103;

	public const int FLAG_TEMPLE_EFFECT_START = 104;

	public const int FLAG_TEMPLE_EFFECT_DONE = 105;

	public const int FLAG_TEMPLE_EFFECT_NOTIFY = 106;

	public const int FLAG_STARFIGHTER_DAMAGE_ALL_ENEMIES = 107;

	public const int FLAG_STARFIGHTER_CAMERA_SLOWDOWN = 108;

	public const int FLAG_SCREEN_FADETOBLACK = 109;

	public const int FLAG_RESET_PLATFORMS = 110;

	public const int FLAG_START_CTHULHU = 111;

	public const int FLAG_END_CTHULHU = 99;

	public const int FLAG_DONT_RENDER_HUD = 98;

	public const int FLAG_END_SLOTTED_IN = 97;

	public const int FLAG_END_MUSIC = 96;

	public const int FLAG_END_GAME = 95;

	public static string GetColorString(int color)
	{
		switch (color)
		{
		case 1:
			return "red";
		case 2:
			return "blue";
		case 3:
			return "green";
		default:
			return "nocolor";
		}
	}

	public static int GetColorInt(string color)
	{
		switch (color)
		{
		case "red":
			return 1;
		case "blue":
			return 2;
		case "green":
			return 3;
		default:
			return 0;
		}
	}
}
