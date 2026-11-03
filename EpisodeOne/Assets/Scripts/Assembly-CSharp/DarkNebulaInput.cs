using UnityEngine;

public class DarkNebulaInput
{
	public static bool UseSphero
	{
		get
		{
			return false;
		}
	}

	public static Vector2 GetInput()
	{
		Vector2 zero = Vector2.zero;
		if (!UseSphero)
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
			vector = UserSettings.Instance.centerCalibration * vector;
		}
		zero.x = vector.x;
		zero.y = vector.y;
		return zero;
	}
}
