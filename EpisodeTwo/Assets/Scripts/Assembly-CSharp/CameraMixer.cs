using UnityEngine;

public class CameraMixer : GameCamera
{
	private ICamera fromCamera;

	private ICamera toCamera;

	private bool catchedUp;

	private BezierCurve bezierCurve;

	private float mixTime;

	public CameraMixer(Vector2 p)
		: base(p)
	{
		Vector2 p2 = default(Vector2);
		p2.x = 0f;
		p2.y = 0f;
		Vector2 p3 = default(Vector2);
		p3.x = 0f;
		p3.y = 0f;
		Vector2 p4 = default(Vector2);
		p4.y = 0f;
		p4.x = 0f;
		p4.y = 1f;
		Vector2 p5 = default(Vector2);
		p5.x = 1f;
		p5.y = 1f;
		catchedUp = true;
		bezierCurve = new BezierCurve(p2, p3, p4, p5);
	}

	public override void UpdateCamera()
	{
		if (fromCamera != null || toCamera != null)
		{
			mixTime += 5f * Time.deltaTime;
			if (mixTime > 1f)
			{
				mixTime = 1f;
			}
			float y = bezierCurve.Calculate(mixTime).y;
			Vector2 vector;
			if (fromCamera == null)
			{
				vector = toCamera.WorldPos();
			}
			else
			{
				Vector2 vector2 = fromCamera.WorldPos();
				Vector2 vector3 = toCamera.WorldPos();
				Vector2 vector4 = (vector3 - vector2) * y;
				vector = vector2 + vector4;
			}
			Vector2 vector5 = WorldPos();
			Vector2 vector6 = vector - vector5;
			if (y == 1f && vector6.magnitude < 0.2f)
			{
				catchedUp = true;
			}
			Vector2 worldPos = ((!catchedUp) ? (vector5 + vector6 * 20f * Time.deltaTime) : vector);
			SetWorldPos(worldPos);
		}
	}

	public void StartMixing(ICamera first, ICamera second)
	{
		mixTime = 0f;
		if (catchedUp)
		{
			if (first != null)
			{
				SetWorldPos(first.WorldPos());
			}
			else
			{
				SetWorldPos(second.WorldPos());
			}
		}
		catchedUp = false;
		fromCamera = first;
		toCamera = second;
	}

	public bool MixingIsDone()
	{
		return mixTime == 1f;
	}
}
