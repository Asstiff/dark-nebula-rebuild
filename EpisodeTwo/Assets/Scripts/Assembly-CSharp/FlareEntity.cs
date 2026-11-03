using UnityEngine;

public class FlareEntity : Entity
{
	public tk2dSprite Streak;

	private float minFlareScale = 0.5f;

	private float maxFlareScale = 7f;

	private float flareScale;

	protected override void Awake()
	{
		base.Awake();
		flareScale = minFlareScale;
		base.enabled = false;
		World.GamePreStartOnce(GamePreStart);
	}

	private void GamePreStart()
	{
		base.enabled = true;
	}

	private void Update()
	{
		float num = Mathf.Abs(World.CameraManager.cameraPosition.y + 100f - base.transform.position.z);
		float num2 = num / 130f;
		if (num > 130f)
		{
			flareScale = minFlareScale;
		}
		else
		{
			flareScale = minFlareScale + (1f - num2) * (maxFlareScale - minFlareScale);
		}
		Streak.transform.localScale = new Vector3(flareScale, 1f, 1.2f);
	}
}
