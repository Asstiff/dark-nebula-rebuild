using System;
using UnityEngine;

public class RotationAngle
{
	private float _radians;

	public float Degrees
	{
		get
		{
			return _radians * 57.29578f % 360f;
		}
		set
		{
			_radians = value * ((float)Math.PI / 180f);
		}
	}

	public float Radians
	{
		get
		{
			return _radians;
		}
		set
		{
			_radians = value;
		}
	}

	public float Angle
	{
		get
		{
			return _radians;
		}
		set
		{
			_radians = value;
		}
	}

	public RotationAngle(float pRadians = 0f)
	{
		_radians = pRadians;
	}

	public static RotationAngle Random()
	{
		return new RotationAngle(UnityEngine.Random.value * (float)Math.PI * 2f);
	}

	public Vector3 ToVector3()
	{
		return new Vector3(Mathf.Cos(_radians), 0f, Mathf.Sin(_radians));
	}
}
