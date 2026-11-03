using System;

public class TimelineNode
{
	public Action<float> Method;

	public float StartTime;

	public float EndTime;

	public TimelineNode(float start, float e, Action<float> s)
	{
		StartTime = start;
		EndTime = e;
		Method = s;
	}
}
