using System;
using FLD;
using UnityEngine;

public class Fish : Enemy
{
	private const int FISH_NUM_JOINTS = 4;

	private const int FISH_NUM_ATTACK_SOUND = 6;

	private const int FISH_NUM_DIE_SOUND = 2;

	private const int FISH_NUM_INJURED_SOUND = 6;

	private const float minTimeBetweenHits = 0.5f;

	public float SpeedTweak = 15f;

	public AudioClip damageSound;

	public AudioClip explodeSound;

	public AudioClip[] attackSound;

	public AudioClip[] dieSound;

	public AudioClip[] injuredSound;

	public tk2dSprite Head;

	public FishPart[] part;

	public float Stiff = 0.001f;

	private float[] jointLastTimeHit = new float[4];

	private int[] jointHealth = new int[4];

	private float targetSpeed;

	private bool firstTime;

	private bool destroyed;

	private Vector3 direction;

	private FlagCondition activateEvent;

	private float fastModeSpeed;

	private float speed;

	private bool wakeupWhenOnScreen;

	protected override void Awake()
	{
		base.Awake();
		base.alwaysAlive = true;
	}

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < 4; i++)
		{
			part[i].transform.parent = base.transform.parent;
			part[i].index = i;
			part[i].UpdateGfx(jointHealth[i]);
		}
		maxHealth = 3;
		speed = GetParameterFloat("speed");
		targetSpeed = GetParameterFloat("slowModeSpeed");
		fastModeSpeed = GetParameterFloat("fastModeSpeed");
		wakeupWhenOnScreen = GetParameterBool("wakeupWhenOnScreen");
		base.alwaysAlive = GetParameterBool("alwaysAlive");
		firstTime = true;
		destroyed = false;
		direction.x = -1f;
		direction.y = 0f;
		if (GetParameterInt("activateEvent") != 0)
		{
			activateEvent = new FlagCondition(GetParameterInt("activateEventFlag"), 1);
			activateEvent.Callback += OnActivateEvent;
		}
		FishPart[] array = part;
		foreach (FishPart fishPart in array)
		{
			fishPart.Hit += OnHitPart;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (activateEvent != null)
		{
			activateEvent.Callback -= OnActivateEvent;
			activateEvent.Dispose();
			activateEvent = null;
		}
	}

	private void OnHitPart(int i, Transform pTransform)
	{
		if (jointLastTimeHit[i] + 0.5f < Time.time)
		{
			jointLastTimeHit[i] = Time.time;
			if (jointHealth[i] < 3)
			{
				SpawnExplosion(pTransform.position);
				Audio.Play("part" + UnityEngine.Random.value, damageSound);
				Audio.PlayRandom("partinj" + UnityEngine.Random.value, injuredSound);
				World.TriggerFlag(100);
				jointHealth[i] = Mathf.Min(3, jointHealth[i] + 1);
				part[i].UpdateGfx(jointHealth[i]);
			}
		}
	}

	public override bool PostInit()
	{
		if (base.PostInit())
		{
			return true;
		}
		for (int i = 0; i < 4; i++)
		{
			jointLastTimeHit[i] = 0f;
			jointHealth[i] = 0;
		}
		return false;
	}

	private void OnActivateEvent()
	{
		if (targetSpeed != fastModeSpeed)
		{
			Debug.Log("Start Fish");
			targetSpeed = fastModeSpeed;
			Audio.PlayRandom("fishAttack" + UnityEngine.Random.value, attackSound);
		}
	}

	public override void Update()
	{
		if (!World.IsRunning)
		{
			return;
		}
		base.Update();
		if (wakeupWhenOnScreen && targetSpeed != fastModeSpeed && World.EntityWithinScreen(this))
		{
			OnActivateEvent();
		}
		if (activateEvent == null && firstTime)
		{
			OnActivateEvent();
			firstTime = false;
		}
		bool flag = true;
		for (int i = 0; i < 4; i++)
		{
			if (jointHealth[i] < maxHealth)
			{
				flag = false;
			}
		}
		if (!destroyed)
		{
			if (!World.EntityWithinScreen(this) && World.vessel != null)
			{
				World.vessel.WaypointEnemy(this);
			}
			if (flag)
			{
				Destruct();
				return;
			}
			Satisfy();
			UpdateNormalMovement();
		}
	}

	private Vector3 Vector3Perpendicular(Vector3 v)
	{
		return new Vector3(0f - v.z, v.y, v.x);
	}

	private void SetWorldPos(Vector3 pos)
	{
		base.transform.position = pos;
	}

	private void SetVelocity(Vector3 pos)
	{
		base.GetComponent<Rigidbody>().linearVelocity = pos;
	}

	private void UpdateNormalMovement()
	{
		StandardVessel standardVessel = World.vessel as StandardVessel;
		if (!(standardVessel == null) && standardVessel.canMove)
		{
			Vector3 position = standardVessel.transform.position;
			Vector3 position2 = base.transform.position;
			Vector3 normalized = (position2 - position).normalized;
			Vector3 vector = Vector3Perpendicular(direction);
			float num = Mathf.Clamp(normalized.x * vector.x + normalized.z * vector.z, -1f, 1f);
			direction += vector * (0.07f * (0f - num) * Time.deltaTime * 30f);
			direction.Normalize();
			float num2 = Mathf.Min(World.Difficulty, 1f);
			float num3 = targetSpeed * num2;
			Vector3 zero = Vector3.zero;
			zero.x = direction.x;
			zero.z = direction.z;
			speed += (num3 - speed) * Time.deltaTime * 5f * num2;
			Vector3 vector2 = zero * speed;
			SetVelocity(vector2 * SpeedTweak);
			float num4 = Mathf.Atan2(direction.z, direction.x);
			Head.transform.localRotation = Quaternion.Euler(90f, 0f, num4 * 57.29578f + 90f);
		}
	}

	private void SpawnSparks(RotationAngle angle)
	{
		SpawnSpark(angle, 0.7f);
		RotationAngle spriteRotation = new RotationAngle(angle.Angle - (float)System.Math.PI);
		SpawnSpark(spriteRotation, 0.7f);
	}

	private void SpawnSpark(RotationAngle spriteRotation, float f)
	{
		Vector3 position = base.transform.position;
		Vector3 vector = spriteRotation.ToVector3();
		position += vector * 60f;
		float scale = 2f;
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		objectForType.Play("_sparks", position, scale, false, spriteRotation);
	}

	private void Satisfy()
	{
		Entity entity = this;
		float num = WithinPi2(FLD.Math.CreateRotationFromDirection(-direction));
		for (int i = 0; i < 4; i++)
		{
			Entity entity2 = part[i];
			float angle = FLD.Math.CreateRotationFromDirection(entity2.transform.position - entity.transform.position);
			angle = AdjustAngle(WithinPi2(angle), num);
			angle = WithinPi2(angle * (1f - Stiff) + Stiff * num);
			Vector3 vector = new RotationAngle(angle + (float)System.Math.PI / 2f).ToVector3();
			entity2.transform.localRotation = Quaternion.Euler(0f, (0f - angle) * 57.29578f, 0f);
			entity2.transform.position = entity.transform.position + vector * 75f;
			num = angle;
			entity = entity2;
		}
	}

	private float AdjustAngle(float angle, float prevAngle)
	{
		float num = angle - prevAngle;
		if (num < -(float)System.Math.PI)
		{
			return angle + (float)System.Math.PI * 2f;
		}
		if (num > (float)System.Math.PI)
		{
			return angle - (float)System.Math.PI * 2f;
		}
		return angle;
	}

	private float WithinPi2(float angle)
	{
		while (angle < 0f)
		{
			angle += (float)System.Math.PI * 2f;
		}
		while (angle > (float)System.Math.PI * 2f)
		{
			angle -= (float)System.Math.PI * 2f;
		}
		return angle;
	}

	private void SpawnDebrisParticles(Vector3 spawnPosition)
	{
		for (int i = 0; i < 10; i++)
		{
			SpawnDebrisParticleAndSize(spawnPosition, 1f);
		}
	}

	private void SpawnDebrisParticleAndSize(Vector3 spawnPosition, float size)
	{
		string type = "_debris" + UnityEngine.Random.Range(1, 4);
		RotationAngle rotationAngle = RotationAngle.Random();
		Vector3 vector = rotationAngle.ToVector3();
		spawnPosition += vector * (UnityEngine.Random.value * 10f + 20f);
		float scale = size + UnityEngine.Random.value * 0.5f;
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		objectForType.Play(type, spawnPosition, scale, UnityEngine.Random.value * 200f + 100f, true, rotationAngle);
	}

	private void SpawnShockwave(Vector3 wPos)
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", wPos, 0.5f, 12);
	}

	private void SpawnExplosion(Vector3 wPos)
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", wPos);
	}

	private void Destruct()
	{
		destroyed = true;
		base.GetComponent<Collider>().enabled = false;
		base.GetComponent<Rigidbody>().freezeRotation = true;
		base.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
		for (int i = 0; i < 4; i++)
		{
			Vector3 position = part[i].transform.position;
			SpawnShockwave(position);
			SpawnDebrisParticles(position);
			Audio.Play("fishexp" + UnityEngine.Random.value, explodeSound);
			Audio.PlayRandom("fishdie" + UnityEngine.Random.value, dieSound);
			World.TriggerFlag(GetParameterInt("dieEventFlag"));
			World.TriggerFlag(100);
			SpawnExplosion(position);
			part[i].Die();
			Head.spriteId = Head.GetSpriteIdByName("fish-head/1");
		}
	}
}
