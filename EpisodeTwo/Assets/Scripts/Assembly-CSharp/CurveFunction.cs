using UnityEngine;

public class CurveFunction
{
	private float rampIn;

	private float rampOut;

	private float length;

	private float time;

	private float f;

	private bool effectDone;

	public float Value
	{
		get
		{
			return f;
		}
	}

	public CurveFunction(float _rampIn, float _rampOut, float _length)
	{
		rampIn = _rampIn;
		rampOut = _rampOut;
		length = _length;
		time = 0f;
		effectDone = false;
		f = 0f;
	}

	public void Update()
	{
		time += Time.deltaTime;
		if (time < rampIn)
		{
			f = time / rampIn;
			return;
		}
		if (time < rampIn + length)
		{
			f = 1f;
			return;
		}
		if (time < rampIn + length + rampOut)
		{
			f = 1f - (time - (rampIn + length)) / rampOut;
			return;
		}
		f = 0f;
		effectDone = true;
	}

	public bool IsEffectDone()
	{
		return effectDone;
	}
}
