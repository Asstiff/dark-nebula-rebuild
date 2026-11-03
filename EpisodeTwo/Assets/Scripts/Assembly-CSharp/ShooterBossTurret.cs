using System;
using FLD;
using UnityEngine;

public class ShooterBossTurret : Enemy
{
	private ShooterBoss.State lastState;

	public AudioClip TurretFireSFX;

	private float timeUntilNextBullet;

	private float timeBetweenConsecutiveBullets;

	public event Action Destroyed;

	protected override void Start()
	{
		base.Start();
		maxHealth = 40;
		base.Health = maxHealth;
		invincibleTimerMax = 0f;
		enableDamageShake = false;
		enableDamageDebris = false;
		enableDamageShockWave = false;
		enableDieShake = false;
		removeFromWorldOnDeath = false;
		timeBetweenConsecutiveBullets = ((!UserSettings.I.expert) ? 0.3f : 0.15f);
		timeUntilNextBullet = timeBetweenConsecutiveBullets;
	}

	public void UpdateState(ShooterBoss.State state)
	{
		lastState = state;
	}

	public override void Update()
	{
		base.Update();
		if (lastState == ShooterBoss.State.TURRETS_STATE)
		{
			timeUntilNextBullet -= Time.deltaTime;
			if (timeUntilNextBullet <= 0f)
			{
				float num = ShooterBoss.TurretsRotationSpeed / ((!UserSettings.I.expert) ? 0.02f : 0.03f) * 2f;
				Vector2 vector = FLD.Math.Delta2d(base.transform.position, ShooterBoss.Instance.transform.position).normalized * num;
				ShooterEnemyBulletEntity objectForType = ObjectPool.instance.GetObjectForType<ShooterEnemyBulletEntity>();
				objectForType.transform.parent = ShooterBoss.Instance.transform.parent;
				objectForType.Init(base.transform.position, vector * 30f, "bulletboss", 1.2f, 10f, true);
				objectForType.PostInit();
				Audio.Play("turretfire", TurretFireSFX);
				timeUntilNextBullet = timeBetweenConsecutiveBullets * (ShooterBoss.TurretsRotationSpeed / 0.02f);
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		StarfighterBulletEntity component = collider.gameObject.GetComponent<StarfighterBulletEntity>();
		if (!(component == null))
		{
			if (lastState == ShooterBoss.State.TURRETS_STATE)
			{
				ReceiveDamage(1);
			}
			component.RemoveFromWorld();
		}
	}

	protected override void OnDie(RotationAngle pImpactDirection)
	{
		base.OnDie(pImpactDirection);
		if (Destroyed != null)
		{
			Destroyed();
		}
		UnityEngine.Object.Destroy(base.transform.parent.parent.gameObject);
	}
}
