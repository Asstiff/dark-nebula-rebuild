using UnityEngine;

public class DarkNebulaInput
{
	public static bool UseSphero
	{
		get
		{
			return SpheroAccelerometer.GetIsConnected();
		}
	}

	public static Vector2 GetInput()
	{
		#if UNITY_EDITOR_WIN || (UNITY_STANDALONE_WIN && DEVELOPMENT_BUILD)
        Vector2 keyboard = new Vector2((Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f),
            (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f));
        return Vector2.ClampMagnitude(keyboard, 1f) * .35f;
#else
        Vector2 zero = Vector2.zero;
		if (UseSphero)
		{
			zero.x += SpheroAccelerometer.Instance.position.x;
			zero.y += SpheroAccelerometer.Instance.position.y * 1.05f;
		}
		else
		{
			zero.x += 0f - LevelMotionCalibration.Gravity.x;
			zero.y += LevelMotionCalibration.Gravity.y * 1.05f;
		}
		Vector3 vector = Vector3.zero;
		vector.x = zero.x;
		vector.y = zero.y;
		if (!UseSphero)
		{
			vector.z = LevelMotionCalibration.Gravity.z;
		}
		if (!UseSphero)
		{
			vector = UserSettings.I.centerCalibration * vector;
		}
		zero.x = vector.x;
		zero.y = vector.y;
		return zero;
#endif
	}
}
