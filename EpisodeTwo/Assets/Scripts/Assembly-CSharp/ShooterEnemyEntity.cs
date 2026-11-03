using System;
using FLD;
using UnityEngine;

public class ShooterEnemyEntity : Enemy
{
	private const float spriteAngleOffset = (float)System.Math.PI / 2f;

	public AudioClip[] explode;

	public bool preAllocated;

	protected float angle;

	protected float currDecent;

	private bool isFalling;

	private bool enableDieShockWave;

	private float decentVel = 1f;

	private FlagCondition receiveOneDamageCondition;

	public event Action<Entity> Died;

	public virtual void Init(Vector3 position, float pAngle, int pIndex)
	{
		base.GetComponent<Rigidbody>().isKinematic = false;
		dieFlag = GetParameterInt("dieFlag");
		base.transform.position = position;
		angle = pAngle;
		maxHealth = 1;
		base.Health = maxHealth;
		currDecent = 1f;
		enableDamageShake = false;
		enableDamageShockWave = false;
		enableDamageDebris = false;
		enableDieShake = false;
		enableDieShockWave = true;
		removeFromWorldOnDeath = true;
		isFalling = false;
		preAllocated = true;
		DisposeReceiveOneDamageCondition();
		receiveOneDamageCondition = new FlagCondition(107, 1);
		receiveOneDamageCondition.Callback += OnReceiveOneDamageCondition;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		DisposeReceiveOneDamageCondition();
	}

	private void DisposeReceiveOneDamageCondition()
	{
		if (receiveOneDamageCondition != null)
		{
			receiveOneDamageCondition.Callback -= OnReceiveOneDamageCondition;
			receiveOneDamageCondition.Dispose();
			receiveOneDamageCondition = null;
		}
	}

	public override void Update()
	{
		if (!IsPooled && Time.timeScale != 0f)
		{
			base.Update();
			if (isFalling)
			{
				currDecent -= decentVel * Time.deltaTime * 30f;
			}
		}
	}

	protected override void OnDie(RotationAngle pImpactDirection)
	{
		preAllocated = true;
		if (World.EntityWithinScreen(this))
		{
			ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", base.transform.position);
			if (enableDieShockWave)
			{
				ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.7f, 8);
			}
			Audio.PlayRandom("shooterenemy" + UnityEngine.Random.value, explode);
			SpawnDebrisParticles(pImpactDirection);
		}
		if (Died != null)
		{
			Died(this);
		}
		if (removeFromWorldOnDeath)
		{
			RemoveFromWorld();
		}
		Dead = true;
		if (dieFlag != 0)
		{
			World.TriggerFlag(dieFlag);
		}
		if (enableDieShake)
		{
			World.TriggerFlag(101);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		StarfighterBulletEntity component = collider.gameObject.GetComponent<StarfighterBulletEntity>();
		if (component != null)
		{
			ReceiveDamage(1, new RotationAngle());
			component.RemoveFromWorld();
		}
	}

	private void OnReceiveOneDamageCondition()
	{
		if (!preAllocated && !Dead)
		{
			ReceiveDamage(1, new RotationAngle());
		}
	}

	public void SetVel(float pVelocity, Vector2 pStrafeVector, float pAngleVelocity)
	{
		float f = angle + (float)System.Math.PI / 2f;
		Vector3 position = base.transform.position;
		position.x += pStrafeVector.x + Mathf.Cos(f) * pVelocity;
		position.z -= pStrafeVector.y + Mathf.Sin(f) * pVelocity;
		base.transform.position = position;
		angle -= pAngleVelocity;
	}
}
