using System;
using UnityEngine;

namespace FLD
{
	public class Math
	{
		public static float AngleFromPositions(Vector2 a, Vector2 b)
		{
			return Mathf.Atan2(a.y - b.y, a.x - b.x);
		}

		public static float AngleFromPositions(Vector3 a, Vector3 b)
		{
			return Mathf.Atan2(a.z - b.z, a.x - b.x);
		}

		public static Vector2 AngleToVector(float radians)
		{
			return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
		}

		public static Vector3 AngleToVector3Z(float radians)
		{
			return new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
		}

		public static float Distance2d(Vector3 a, Vector3 b)
		{
			return new Vector2(a.x - b.x, a.z - b.z).magnitude;
		}

		public static Vector2 Delta2d(Vector3 a, Vector3 b)
		{
			return new Vector2(a.x - b.x, a.z - b.z);
		}

		public static float CreateRotationFromDirection(Vector3 v)
		{
			float num = Mathf.Atan2(v.z, v.x);
			return num - (float)System.Math.PI / 2f;
		}

		public static float CreateRotationFromDirection(Vector2 v)
		{
			float num = Mathf.Atan2(v.y, v.x);
			return num - (float)System.Math.PI / 2f;
		}
	}
}
