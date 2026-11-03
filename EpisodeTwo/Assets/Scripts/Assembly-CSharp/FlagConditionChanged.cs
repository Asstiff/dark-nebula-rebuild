using System;

public class FlagConditionChanged
{
	private int flag;

	private int count;

	public event Action Callback;

	public FlagConditionChanged(int pFlag)
	{
		if (pFlag != 0)
		{
			flag = pFlag;
			count = 1;
			World.GetTrigger(flag).Changed += HandleWorldSignalFlagChanged;
			Reset();
		}
	}

	public void Reset()
	{
		count = 1;
	}

	public void Dispose()
	{
		World.GetTrigger(flag).Changed -= HandleWorldSignalFlagChanged;
		Callback = null;
	}

	private void HandleWorldSignalFlagChanged(int newValue)
	{
		if (newValue == count)
		{
			count++;
			if (Callback != null)
			{
				Callback();
			}
		}
	}
}
