public class DarknessEntity : Entity
{
	public float playerLightScale
	{
		get
		{
			return GetParameterFloat("playerLightScale");
		}
	}

	public float playerLightMinScale
	{
		get
		{
			return GetParameterFloat("playerLightMinScale");
		}
	}

	public float playerLightMaxScale
	{
		get
		{
			return GetParameterFloat("playerLightMaxScale");
		}
	}

	private void Start()
	{
		World.GamePreStart += GamePreStart;
	}

	private void GamePreStart()
	{
		World.GamePreStart -= GamePreStart;
		World.Darkness = this;
		World.cameraFollow.EnableDarkness(true);
	}

	public override void DisableAlwaysAliveAndKeepaliveEntities()
	{
	}
}
