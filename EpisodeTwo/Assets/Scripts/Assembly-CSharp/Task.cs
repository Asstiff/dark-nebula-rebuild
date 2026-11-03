using UnityEngine;

public class Task
{
	public virtual bool IsDone()
	{
		return false;
	}

	public virtual void Update()
	{
	}

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}

	public virtual void SetParameter(string key, string value)
	{
		Debug.LogWarning(string.Concat("Unimplemented SetParameter ", this, ".", key, "=", value));
	}
}
