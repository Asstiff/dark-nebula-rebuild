using System;
using System.Collections.Generic;

public class Timeline
{
	private List<TimelineNode> timeline;

	public Timeline()
	{
		timeline = new List<TimelineNode>();
	}

	public void AddEndMethod(float startTime, float endTime, Action<float> s)
	{
		TimelineNode item = new TimelineNode(startTime, endTime, s);
		timeline.Add(item);
	}

	private void Update(float time)
	{
		foreach (TimelineNode item in timeline)
		{
			if (time >= item.StartTime && time <= item.EndTime)
			{
				float obj = (time - item.StartTime) / (item.EndTime - item.StartTime);
				item.Method(obj);
			}
		}
	}
}
