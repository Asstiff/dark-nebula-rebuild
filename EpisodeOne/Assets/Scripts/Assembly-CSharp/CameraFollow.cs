using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	private const float LookAHead = 0.4f;

	private const float TargetDamp = 60f;

	private const float ClampMultiplier = 2f;

	public GameObject Target;

	public float scrollSpeed;

	public float targetScrollSpeed;

	public float worldHeight = 64000f;

	public float worldWidth = 320f;

	public float windowPosZ;

	private float scrollLine;

	private Transform _transform;

	private float ScreenHeight = Constants.VirtualScreenHeight;

	private Vector3 tmp = Vector3.zero;

	private void Start()
	{
		Target = GameObject.Find("VesselEntity");
		_transform = base.transform;
		scrollLine = Constants.VirtualScreenHeightHalf;
		windowPosZ = _transform.position.z - scrollLine;
		tmp.x = _transform.position.x;
		tmp.y = _transform.position.y;
		ScreenHeight = Constants.VirtualScreenHeight;
	}

	private void LateUpdate()
	{
		float num = Time.deltaTime * 30f;
		scrollSpeed += (targetScrollSpeed - scrollSpeed) * 0.1f * num;
		if (scrollSpeed < 0.5f)
		{
			scrollSpeed = 0f;
		}
		float num2 = 0f;
		if (scrollSpeed >= 5f)
		{
			num2 = ScreenHeight * 0.25f * Mathf.Clamp((1f - LegacyScreenFit.ReferenceAspect) * 3f, 0f, 1f);
		}
		else
		{
			float num3 = 0.66f / LegacyScreenFit.ReferenceAspect;
			float y = World.vessel.lastFilteredAccelerometerPosition.y;
			if (LegacyScreenFit.ReferenceAspect > 0.8f)
			{
				float num4 = (1f - Mathf.Abs((base.GetComponent<Camera>().WorldToViewportPoint(Target.transform.position).y - 0.5f) * 2f)) * 2f;
				num2 = ScreenHeight / 2f;
				if (y < 0f)
				{
					num2 += ScreenHeight * 0.4f;
				}
				else if (y > 0f)
				{
					num2 -= ScreenHeight * 0.4f;
				}
				num2 -= y * 60f * num3 * num4;
			}
			else
			{
				num2 = ScreenHeight / 2f;
				num2 -= y * 60f * num3;
			}
		}
		scrollLine += (num2 - scrollLine) * 0.2f * num;
		float num5 = Target.transform.position.z - scrollLine;
		float num6 = 0.045f;
		if (scrollSpeed >= 5f)
		{
			num6 = 0.08f;
			num6 += scrollSpeed / 5f * 0.05f;
		}
		float num7 = (num5 - windowPosZ) * num6 * num;
		if (scrollSpeed < 5f && num7 > 20f * num)
		{
			num7 = 20f * num;
		}
		windowPosZ += num7;
		tmp.z = windowPosZ + ScreenHeight / 2f;
		_transform.position = tmp;
	}

	private void OnDisable()
	{
		Target = null;
	}
}
