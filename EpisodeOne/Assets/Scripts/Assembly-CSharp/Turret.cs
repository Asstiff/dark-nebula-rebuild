using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class Turret : Entity
{
	public AudioClip Sfx;

	public List<float> bulletSpeedSetting = new List<float>();

	public List<float> reloadtimeSetting = new List<float>();

	public List<float> startdelaySetting = new List<float>();

	public float bulletSize = 20f;

	public int direction;

	private float bulletSpeed = 5f;

	private float originalBulletSpeed = 5f;

	private float reloadtime = 30f;

	private float originalReloadtime = 30f;

	private float startdelay;

	private float originalStartdelay;

	private Vector3 bulletPosition;

	private Transform _transform;

	private float _z;

	private static bool _inited = false;

	private static float[] directionAngle = new float[8];

	private static float[] directionX = new float[8];

	private static float[] directionZ = new float[8];

	private static int[] frames = new int[8] { 0, 1, 3, 5, 4, 7, 2, 6 };

	private Vector3 v = Vector3.zero;

	private int _lastTimeFrame;

	private static void Init()
	{
		if (!_inited)
		{
			for (int i = 0; i < 8; i++)
			{
				float num = -(float)Math.PI / 2f + (float)Math.PI / 4f * (float)i;
				directionAngle[i] = num;
				directionX[i] = Mathf.Cos(num);
				directionZ[i] = 0f - Mathf.Sin(num);
			}
		}
	}

	private void OnEnable()
	{
		Init();
		UpdateGfx();
		_transform = base.transform;
		_z = _transform.position.z;
		if (bulletSpeedSetting.Count == 0)
		{
			Debug.LogError("Turret has no speed setting");
		}
		bulletSpeed = (originalBulletSpeed = UserSettings.GetDifficultValue(bulletSpeedSetting.ToArray()));
		reloadtime = (originalReloadtime = UserSettings.GetDifficultValue(reloadtimeSetting.ToArray()));
		if (startdelaySetting.Count != 0)
		{
			startdelay = (originalStartdelay = UserSettings.GetDifficultValue(startdelaySetting.ToArray()));
		}
		else
		{
			startdelay = 0f;
		}
		World.SignalDifficultyChanged += SetDifficultyTo;
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

	private void Update()
	{
		int num = Mathf.FloorToInt((Time.time + startdelay) / reloadtime);
		if (num != _lastTimeFrame)
		{
			_lastTimeFrame = num;
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
				objectForType.Init(v * bulletSpeed, bulletSize, bulletPosition, Time.time);
				Blast objectForType2 = ObjectPool.instance.GetObjectForType<Blast>();
				bulletPosition = _transform.position;
				bulletPosition.x += v.x * 30f;
				bulletPosition.z += v.z * 30f;
				objectForType2.Play(bulletPosition);
				int num2 = (int)(bulletPosition.x / 64f);
				int num3 = (int)((float)Time.frameCount / 3f);
				Audio.Play("bullet" + num2 + " " + num3, Sfx, _transform.position);
			}
		}
	}

	private void UpdateGfx()
	{
		try
		{
			tk2dSprite componentInChildren = GetComponentInChildren<tk2dSprite>();
			if (componentInChildren != null)
			{
				componentInChildren.spriteId = componentInChildren.GetSpriteIdByName("turret/" + frames[direction]);
			}
		}
		catch (Exception)
		{
		}
	}
}
