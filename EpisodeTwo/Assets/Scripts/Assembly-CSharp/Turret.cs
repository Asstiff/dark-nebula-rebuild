using System;
using FLD;
using UnityEngine;

public class Turret : Entity
{
	public AudioClip Sfx;

	public int type;

	public int direction;

	private float bulletSpeedSetting = 5f;

	private float reloadtimeSetting = 30f;

	private float startdelaySetting;

	private float bulletSize = 20f;

	private float bulletSpeed = 5f;

	private float originalBulletSpeed = 5f;

	private float reloadtime = 30f;

	private float originalReloadtime = 30f;

	private float startdelay;

	private float originalStartdelay;

	private Vector3 bulletPosition;

	private Transform _transform;

	private float _z;

	public static float[] directionAngle;

	public static float[] directionX;

	public static float[] directionZ;

	private static int[] frames;

	private int intReloadtime;

	private Vector3 v = Vector3.zero;

	private int _lastTimeFrame;

	private int timeFrame;

	static Turret()
	{
		directionAngle = new float[8];
		directionX = new float[8];
		directionZ = new float[8];
		frames = new int[8] { 0, 1, 3, 5, 4, 7, 2, 6 };
		for (int i = 0; i < 8; i++)
		{
			float num = -(float)System.Math.PI / 2f + (float)System.Math.PI / 4f * (float)i;
			directionAngle[i] = num;
			directionX[i] = Mathf.Cos(num);
			directionZ[i] = 0f - Mathf.Sin(num);
		}
		Debug.Log("directions " + directionAngle[0] + " " + directionAngle[1]);
	}

	private void Start()
	{
		startdelaySetting = GetParameterFloat("startdelay");
		reloadtimeSetting = GetParameterFloat("reloadtime");
		bulletSpeedSetting = GetParameterFloat("bulletspeed");
		direction = GetParameterInt("direction");
		type = GetParameterInt("type");
		_transform = base.transform;
		_z = _transform.position.z;
		bulletSpeed = (originalBulletSpeed = bulletSpeedSetting);
		reloadtime = (originalReloadtime = reloadtimeSetting);
		intReloadtime = (int)(reloadtime * 30f);
		startdelay = (originalStartdelay = startdelaySetting);
		World.SignalDifficultyChanged += SetDifficultyTo;
		UpdateGfx();
		_lastTimeFrame = Mathf.FloorToInt((World.WorldTimeInSeconds + startdelay) / reloadtime);
	}

	private void OnDisable()
	{
		World.SignalDifficultyChanged -= SetDifficultyTo;
	}

	private void SetDifficultyTo(float delta)
	{
		bulletSpeed = originalBulletSpeed * delta;
		reloadtime = originalReloadtime / delta;
		startdelay = originalStartdelay / delta;
	}

	private void FixedUpdate()
	{
		if (!World.IsRunning || intReloadtime == 0)
		{
			return;
		}
		timeFrame = Mathf.FloorToInt((World.WorldTimeInSeconds + startdelay) * 30f);
		if (timeFrame % intReloadtime != 0 || _lastTimeFrame == timeFrame)
		{
			return;
		}
		_lastTimeFrame = timeFrame;
		if (!World.IsFrozen(_z))
		{
			v.x = directionX[direction];
			v.z = directionZ[direction];
			bulletPosition = _transform.position;
			bulletPosition.x += v.x * 40f;
			bulletPosition.z += v.z * 40f;
			Bullet objectForType = ObjectPool.instance.GetObjectForType<Bullet>();
			objectForType.transform.position = bulletPosition;
			objectForType.transform.rotation = Quaternion.AngleAxis(directionAngle[direction] * 57.29578f, Vector3.up);
			objectForType.Init(v * bulletSpeed, bulletSize, bulletPosition, World.WorldTimeInSeconds);
			if (type == 1)
			{
				objectForType.SetFrame(0);
			}
			else if (type == 2)
			{
				objectForType.setChildedLight(true);
				objectForType.SetFrame(1);
			}
			Blast objectForType2 = ObjectPool.instance.GetObjectForType<Blast>();
			bulletPosition = _transform.position;
			bulletPosition.x += v.x * 30f;
			bulletPosition.z += v.z * 30f;
			objectForType2.Play(bulletPosition);
			int num = (int)(Time.realtimeSinceStartup / 3f);
			Audio.Play("bullet" + num, Sfx, _transform.position);
		}
	}

	private void UpdateGfx()
	{
		try
		{
			tk2dSprite componentInChildren = GetComponentInChildren<tk2dSprite>();
			if (componentInChildren != null)
			{
				string text = string.Empty;
				switch (type)
				{
				case 0:
					text = "turret/" + frames[direction];
					break;
				case 1:
					text = "turret_ep2/" + frames[direction];
					break;
				case 2:
					text = "turret_ep2_darkness/" + frames[direction];
					break;
				}
				componentInChildren.spriteId = componentInChildren.GetSpriteIdByName(text);
				if (componentInChildren.spriteId == 0)
				{
					Debug.LogError("Could not find sprite " + text);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		type = GetParameterInt("type");
		direction = GetParameterInt("direction");
		UpdateGfx();
	}

	public override void InitParametersInRuntime()
	{
		InitParametersInEditor();
	}
}
