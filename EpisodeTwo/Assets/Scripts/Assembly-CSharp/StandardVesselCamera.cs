using UnityEngine;

public class StandardVesselCamera : GameCamera
{
	private float lookAheadY;

	private float lookAheadX;

	private Vector2 targetPosition;

	private Vector2 aspectMargin;

	private bool UseLookAhead;

	private Vector2 prevLookAhead = Vector2.zero;

	private Vector2 prevTargetPosition = Vector2.zero;

	private Vector3 prevVesselPos;

	public StandardVesselCamera(Vector2 p)
		: base(p)
	{
		SetWorldPos(p);
		World.CameraManager.ActivateCameraPriority(this, 2);
		float cameraAspect = CameraSize.CameraAspect;
		UseLookAhead = cameraAspect > 0.8f;
		// Vessel cameras are created during Update, outside a rendering callback.
		Camera gameCamera = World.cameraFollow != null ? World.cameraFollow.GetComponent<Camera>() : null;
		float renderAspect = gameCamera != null ? gameCamera.aspect : cameraAspect;
		aspectMargin.x = renderAspect * 32f * cameraAspect;
		aspectMargin.y = 1f / Mathf.Max(renderAspect, 0.01f) * 32f * cameraAspect;
	}

	public override void Reset()
	{
	}

	public override void Remove()
	{
		World.CameraManager.DeactivateCamera(this);
	}

	public override void UpdateCamera()
	{
		StandardVessel standardVessel = (StandardVessel)World.vessel;
		if (standardVessel == null)
		{
			return;
		}
		Vector2 vector = new Vector2(standardVessel.transform.position.x, standardVessel.transform.position.z);
		if (prevTargetPosition == Vector2.zero)
		{
			prevTargetPosition = vector;
		}
		if (UseLookAhead)
		{
			Vector3 vector2 = World.vessel.GetComponent<Rigidbody>().linearVelocity;
			if (prevVesselPos != Vector3.zero)
			{
				vector2 = (World.vessel.transform.position - prevVesselPos) / Time.deltaTime;
			}
			prevVesselPos = World.vessel.transform.position;
			Vector2 a = new Vector2(Mathf.Clamp(vector2.x, -550f, 550f), Mathf.Clamp(vector2.z, -550f, 550f));
			vector += Vector2.Scale(a, aspectMargin * 0.015f);
			vector = Vector2.Lerp(prevTargetPosition, vector, Time.deltaTime * 4f);
		}
		prevTargetPosition = vector;
		SetWorldPos(vector);
	}
}
