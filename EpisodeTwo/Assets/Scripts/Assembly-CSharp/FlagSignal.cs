using System;

public class FlagSignal
{
	public event Action<int> Changed;

	public void Trigger(int pValue)
	{
		if (Changed != null)
		{
			Changed(pValue);
		}
	}
}
