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
		Vector2 zero = Vector2.zero;
		if (UseSphero)
		{
			zero.x += SpheroAccelerometer.Instance.position.x;
			zero.y += SpheroAccelerometer.Instance.position.y * 1.05f;
		}
		else
		{
			zero.x += 0f - Input.acceleration.x;
			zero.y += Input.acceleration.y * 1.05f;
		}
		Vector3 vector = Vector3.zero;
		vector.x = zero.x;
		vector.y = zero.y;
		if (!UseSphero)
		{
			vector.z = Input.acceleration.z;
		}
		if (!UseSphero)
		{
			vector = UserSettings.I.centerCalibration * vector;
		}
		zero.x = vector.x;
		zero.y = vector.y;
		return zero;
	}
}
