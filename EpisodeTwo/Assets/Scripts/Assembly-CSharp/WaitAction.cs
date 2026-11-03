using UnityEngine;

public class WaitAction : Task
{
	private float startTime;

	public float time;

	public override void Enter()
	{
		startTime = Time.realtimeSinceStartup;
	}

	public override bool IsDone()
	{
		return startTime + time <= Time.realtimeSinceStartup;
	}

	public override string ToString()
	{
		return string.Format("[WaitAction].time = " + time);
	}

	public override void SetParameter(string key, string value)
	{
		switch (key)
		{
		case "time":
			time = float.Parse(value);
			break;
		default:
			base.SetParameter(key, value);
			break;
		}
	}
}
