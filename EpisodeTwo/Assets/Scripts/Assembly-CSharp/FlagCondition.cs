using System;

public class FlagCondition
{
	private int flag;

	private int count;

	public event Action Callback;

	public FlagCondition(int pFlag, int pCount)
	{
		flag = pFlag;
		count = pCount;
		Reset();
	}

	public void Dispose()
	{
		World.GetTrigger(flag).Changed -= HandleWorldSignalFlagChanged;
		World.SignalUpdate -= HandleWorldSignalUpdate;
	}

	public void Reset()
	{
		if (flag != 0)
		{
			if (flag == -1)
			{
				World.SignalUpdate += HandleWorldSignalUpdate;
			}
			else
			{
				World.GetTrigger(flag).Changed += HandleWorldSignalFlagChanged;
			}
		}
	}

	private void HandleWorldSignalUpdate()
	{
		World.SignalUpdate -= HandleWorldSignalUpdate;
		if (Callback != null)
		{
			Callback();
		}
	}

	private void HandleWorldSignalFlagChanged(int newValue)
	{
		if (newValue == count)
		{
			World.GetTrigger(flag).Changed -= HandleWorldSignalFlagChanged;
			if (Callback != null)
			{
				Callback();
			}
		}
	}
}
