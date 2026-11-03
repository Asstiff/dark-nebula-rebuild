using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class HomingTurret : Enemy
{
	public enum HomingTurretState
	{
		HOMINGTURRET_SLEEPING = 0,
		HOMINGTURRET_ACTIVATED = 1,
		HOMINGTURRET_START_AIMING = 2,
		HOMINGTURRET_DEAD = 3
	}

	public tk2dSprite Base;

	public tk2dSprite Turret;

	public tk2dSprite Fire;

	public AudioClip turretFireSound;

	public AudioClip turretChargeSound;

	public AudioClip turretOpenSound;

	public HomingTurretBullet BulletPrefab;

	private FlagConditionChanged alertEvent;

	public HomingTurretState turretState;

	private int alertEventFlag;

	private int maxAliveBullets;

	private bool activated;

	private bool charging;

	private bool stopturretFrameInc;

	private float animationSpeed;

	private float angle;

	private float bulletspeed;

	private float originalBulletSpeed;

	private float _baseFrame;

	private float _turretFrame;

	private float activationRadius;

	private float _turretScale;

	private float currFrame;

	private float reloadTime;

	private float loadingScale;

	private List<HomingTurretBullet> firedBullets;

	public float turretFrame
	{
		get
		{
			return _turretFrame;
		}
		set
		{
			_turretFrame = value;
			Turret.spriteId = Turret.GetSpriteIdByName("homingturret_tower/" + (int)_turretFrame);
		}
	}

	public float baseFrame
	{
		get
		{
			return _baseFrame;
		}
		set
		{
			_baseFrame = value;
			Base.spriteId = Base.GetSpriteIdByName("homingturret/" + (int)_baseFrame);
		}
	}

	public float turretScale
	{
		get
		{
			return _turretScale;
		}
		set
		{
			_turretScale = value;
			Turret.transform.localScale = new Vector3(_turretScale, _turretScale, 1f);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		firedBullets = new List<HomingTurretBullet>();
		HomingTurretBullet homingTurretBullet = CreateOrGetBullet();
		homingTurretBullet.Die();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (alertEvent != null)
		{
			alertEvent.Callback -= OnAlertEvent;
			alertEvent.Dispose();
			alertEvent = null;
		}
	}

	protected override void Start()
	{
		base.Start();
		maxHealth = 3;
		base.Health = maxHealth;
		if (GetParameterBool("listensToAlert"))
		{
			alertEventFlag = GetParameterInt("alertFriendsTriggerFlag");
		}
		else
		{
			alertEventFlag = 0;
		}
		alertEvent = new FlagConditionChanged(alertEventFlag);
		alertEvent.Callback += OnAlertEvent;
		animationSpeed = 0.5f;
		angle = -(float)System.Math.PI / 2f;
		reloadTime = GetParameterFloat("reloadTime");
		maxAliveBullets = GetParameterInt("maxAliveBullets");
		activationRadius = GetParameterFloat("activationRadius");
		bulletspeed = GetParameterFloat("bulletspeed");
		originalBulletSpeed = bulletspeed;
		turretScale = 0f;
		turretFrame = 0f;
		removeFromWorldOnDeath = false;
		invincibleTimerMax = 0f;
		color = 1;
		baseFrame = 0f;
		activated = false;
		charging = false;
		Fire.transform.localScale = new Vector3(0f, 1f, 0f);
		Turret.transform.localRotation = Quaternion.Euler(0f, 0f, angle * 57.29578f + 90f);
	}

	public override void ReceiveDamage(int damage, RotationAngle impactDirectionRadian)
	{
		base.ReceiveDamage(damage, impactDirectionRadian);
		turretFrame = 6 - base.Health;
	}

	private void TransitToState(HomingTurretState newstate)
	{
		turretState = newstate;
		if (newstate == HomingTurretState.HOMINGTURRET_ACTIVATED)
		{
			World.TriggerFlag(GetParameterInt("alertFriendsTriggerFlag"));
		}
		if (newstate == HomingTurretState.HOMINGTURRET_DEAD)
		{
			foreach (HomingTurretBullet firedBullet in firedBullets)
			{
				if (!firedBullet.Dead)
				{
					firedBullet.Die();
				}
			}
			baseFrame = 11f;
			Turret.GetComponent<Renderer>().enabled = false;
			Fire.GetComponent<Renderer>().enabled = false;
			World.TriggerFlag(dieFlag);
		}
		if (newstate == HomingTurretState.HOMINGTURRET_START_AIMING)
		{
			currFrame = -29f;
		}
	}

	private void OnAlertEvent()
	{
		if (turretState == HomingTurretState.HOMINGTURRET_SLEEPING)
		{
			TransitToState(HomingTurretState.HOMINGTURRET_ACTIVATED);
		}
	}

	private void UpdateRotation()
	{
		Vector2 normalized = FLD.Math.Delta2d(World.vessel.transform.position, base.transform.position).normalized;
		Vector2 vector = new Vector2(0f - normalized.y, normalized.x);
		Vector2 vector2 = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		float num = vector.x * vector2.x + vector2.y * vector.y;
		float f = Mathf.Min(1f, Mathf.Max(normalized.x * vector2.x + normalized.y * vector2.y, 0f));
		float num2 = Mathf.Acos(f) * ((!(num > 0f)) ? 1f : (-1f));
		angle += 0.04f * num2;
		Turret.transform.localRotation = Quaternion.Euler(0f, 0f, angle * 57.29578f + 90f);
	}

	private void SpawnShockwave(Vector3 wPos)
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", wPos, 0.5f, 12);
	}

	private int NumberOfAliveBullets()
	{
		int num = 0;
		foreach (HomingTurretBullet firedBullet in firedBullets)
		{
			if (!firedBullet.Dead)
			{
				num++;
			}
		}
		return num;
	}

	private void UpdateFire()
	{
		bulletspeed = originalBulletSpeed * World.Difficulty;
		float num = bulletspeed;
		Vector2 vector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * num;
		Vector3 position = base.transform.position;
		Vector2 vector2 = new Vector2(position.x + vector.x / num * 8f * 7f, position.z + vector.y / num * 8f * 7f);
		HomingTurretBullet homingTurretBullet = CreateOrGetBullet();
		float num2 = Mathf.Min(World.Difficulty, 1f);
		homingTurretBullet.GetComponent<Rigidbody>().linearVelocity = vector;
		homingTurretBullet.transform.parent = base.transform.parent;
		homingTurretBullet.transform.position = new Vector3(vector2.x, base.transform.position.y + 0.5f, vector2.y);
		homingTurretBullet.SetBulletSpeed(bulletspeed * num2);
		homingTurretBullet.PostInit();
		Audio.Play("homingturretfire", turretFireSound, base.transform.position);
	}

	private HomingTurretBullet CreateOrGetBullet()
	{
		foreach (HomingTurretBullet firedBullet in firedBullets)
		{
			if (firedBullet.Dead)
			{
				firedBullet.gameObject.SetActive(true);
				firedBullet.Dead = false;
				return firedBullet;
			}
		}
		HomingTurretBullet homingTurretBullet = UnityEngine.Object.Instantiate(BulletPrefab) as HomingTurretBullet;
		firedBullets.Add(homingTurretBullet);
		return homingTurretBullet;
	}

	public override void Update()
	{
		base.Update();
		if (World.vessel == null)
		{
			return;
		}
		switch (turretState)
		{
		case HomingTurretState.HOMINGTURRET_SLEEPING:
			if ((base.transform.position - World.vessel.transform.position).magnitude < activationRadius)
			{
				TransitToState(HomingTurretState.HOMINGTURRET_ACTIVATED);
			}
			break;
		case HomingTurretState.HOMINGTURRET_ACTIVATED:
			if (!activated)
			{
				Audio.Play("turretOpenSound", turretOpenSound);
				activated = true;
			}
			if (baseFrame < 10f)
			{
				baseFrame += animationSpeed * Time.deltaTime * 30f;
			}
			if (baseFrame >= 7f)
			{
				turretScale += 0.125f * animationSpeed * Time.deltaTime * 30f;
				if (turretScale > 1f)
				{
					turretScale = 1f;
					TransitToState(HomingTurretState.HOMINGTURRET_START_AIMING);
				}
			}
			break;
		case HomingTurretState.HOMINGTURRET_START_AIMING:
			if (isDead)
			{
				TransitToState(HomingTurretState.HOMINGTURRET_DEAD);
			}
			if (!stopturretFrameInc && (double)turretFrame < 3.0)
			{
				turretFrame += animationSpeed * Time.deltaTime * 30f;
				if (turretFrame >= 3f)
				{
					turretFrame = 3f;
					stopturretFrameInc = true;
				}
			}
			UpdateRotation();
			if (NumberOfAliveBullets() >= maxAliveBullets)
			{
				loadingScale = 0f;
				currFrame = 0f;
				Fire.transform.localScale = new Vector3(loadingScale, loadingScale, 1f);
				break;
			}
			if (currFrame > reloadTime * 30f)
			{
				currFrame = 0f;
				UpdateFire();
			}
			else
			{
				if (currFrame > reloadTime * 10f)
				{
					if (!charging)
					{
						Audio.Play("turretChargeSound", turretChargeSound);
						charging = true;
					}
					loadingScale = (currFrame - reloadTime * 10f) / (reloadTime * 30f);
					if ((double)loadingScale > 0.5)
					{
						loadingScale = 1f - loadingScale;
						loadingScale -= 1.5f * (0.5f - loadingScale) * Time.deltaTime * 30f;
						if (loadingScale < 0f)
						{
							loadingScale = 0f;
						}
					}
					loadingScale *= 2f;
				}
				else
				{
					loadingScale = 0f;
					charging = false;
				}
				Fire.transform.localScale = new Vector3(loadingScale, loadingScale, 1f);
			}
			currFrame += Time.deltaTime * 30f;
			break;
		case HomingTurretState.HOMINGTURRET_DEAD:
			break;
		}
	}
}
