using UnityEngine;

public class StarfighterBackgroundEntity : Entity
{
	protected override void Awake()
	{
		base.Awake();
		base.alwaysAlive = true;
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		position.x = World.cameraFollow.windowPosX;
		position.z = World.cameraFollow.windowPosZ;
		base.transform.position = position;
	}
}
