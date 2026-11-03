using UnityEngine;

public class CurveAnimationTimeFunction
{
	private const float maxValue = 1f;

	private bool useLinear;

	private float easeStart;

	private float easeEnd;

	private BezierCurve bezier;

	public CurveAnimationTimeFunction(float easeStart, float easeEnd, float duration)
	{
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 zero3 = Vector2.zero;
		Vector2 zero4 = Vector2.zero;
		easeStart = Mathf.Ceil(easeStart);
		easeEnd = Mathf.Ceil(easeEnd);
		zero.x = 0f;
		zero.y = 0f;
		if (easeStart == 0f)
		{
			zero2.x = 1f / 3f;
			zero2.y = 1f / 3f;
		}
		else if (easeStart == 1f)
		{
			zero2.x = 0.42f;
			zero2.y = 0f;
		}
		else if (easeStart == 2f)
		{
			zero2.x = 0.5f;
			zero2.y = 0f;
		}
		if (easeEnd == 0f)
		{
			zero3.x = 2f / 3f;
			zero3.y = 2f / 3f;
		}
		else if (easeEnd == 1f)
		{
			zero3.x = 0.58f;
			zero3.y = 1f;
		}
		else if (easeEnd == 2f)
		{
			zero3.x = 0.5f;
			zero3.y = 1f;
		}
		zero4.x = 1f;
		zero4.y = 1f;
		useLinear = easeStart < 0.001f && easeEnd < 0.001f;
		bezier = new BezierCurve(zero, zero2, zero3, zero4);
	}

	public float timeFunc(float t)
	{
		if (useLinear)
		{
			return t;
		}
		return bezier.Calculate(t).y;
	}
}
