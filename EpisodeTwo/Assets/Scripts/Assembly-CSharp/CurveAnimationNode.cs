using UnityEngine;

public class CurveAnimationNode
{
	public float length;

	public float time;

	public float endTime;

	public Vector2 position;

	private CurveAnimationTimeFunction timeFunction;

	public CurveAnimationNode(Vector2 pPosition, float pTime, float pDuration, CurveAnimationTimeFunction func)
	{
		time = pTime;
		endTime = pTime + pDuration;
		position = pPosition;
		length = pDuration;
		timeFunction = func;
	}

	public Vector2 getPosition(CurveAnimationNode nextNode, float f)
	{
		Vector2 vector = nextNode.position - position;
		return position + vector * f;
	}

	public float convertTime(float t)
	{
		if (timeFunction == null)
		{
			return t;
		}
		return timeFunction.timeFunc(t);
	}

	public void enter()
	{
	}

	public void exit()
	{
	}
}
