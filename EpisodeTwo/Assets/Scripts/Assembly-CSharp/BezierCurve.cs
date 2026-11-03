using UnityEngine;

public class BezierCurve
{
	private Vector2 p0;

	private Vector2 p1;

	private Vector2 p2;

	private Vector2 p3;

	public BezierCurve(Vector2 P0, Vector2 P1, Vector2 P2, Vector2 P3)
	{
		p0 = P0;
		p1 = P1;
		p2 = P2;
		p3 = P3;
	}

	public Vector2 Calculate(float t)
	{
		float num = 1f - t;
		float num2 = num * num;
		float num3 = num2 * num;
		float num4 = t * t;
		float num5 = num4 * t;
		Vector2 vector = p0 * num3;
		Vector2 vector2 = p1 * (3f * num2 * t);
		Vector2 vector3 = p2 * (3f * num * num4);
		Vector2 vector4 = p3 * num5;
		return vector + vector2 + vector3 + vector4;
	}
}
