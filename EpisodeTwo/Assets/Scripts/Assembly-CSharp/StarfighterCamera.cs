using UnityEngine;

public class StarfighterCamera : GameCamera
{
	public float Speed = 4f;

	public StarfighterCamera(Vector2 p)
		: base(p)
	{
		World.CameraManager.ActivateCameraPriority(this, 2);
	}

	public override void UpdateCamera()
	{
		localPos.y += Speed * Time.deltaTime * 30f;
	}

	public override void Remove()
	{
		World.CameraManager.DeactivateCamera(this);
	}
}
