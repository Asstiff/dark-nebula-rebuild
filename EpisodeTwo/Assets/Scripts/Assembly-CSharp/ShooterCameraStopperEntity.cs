using UnityEngine;

public class ShooterCameraStopperEntity : Entity
{
	private StarfighterCamera cam;

	protected override void Awake()
	{
		base.Awake();
		World.GamePreStartOnce(GamePreStart);
		base.enabled = false;
	}

	private void GamePreStart()
	{
		base.enabled = true;
	}

	private void Update()
	{
		if (cam == null)
		{
			cam = World.CameraManager.FindHighestPriorityCamera() as StarfighterCamera;
			if (cam == null)
			{
				return;
			}
		}
		if (cam.Speed <= 0f)
		{
			Object.Destroy(base.gameObject);
			cam.Speed = 0f;
		}
		else if (cam.WorldPos().y - base.transform.position.z > -297.6f)
		{
			cam.Speed -= GetParameterFloat("slowDownSpeed") * Time.deltaTime * 30f;
		}
	}
}
