using System;
using FLD;
using UnityEngine;

public class ShooterBossBrainGlow : ShooterBossPart
{
	public AudioClip DamageSFX;

	public AudioClip TurretFireSFX;

	private float timeBetweenConsecutiveBullets;

	private float timeUntilNextBullet;

	private float scaleX = 0.56f;

	private float scaleY = 1.7f;

	private int damageReceived;

	private ShooterBoss.State state;

	private void Awake()
	{
		timeBetweenConsecutiveBullets = (UserSettings.I.expert ? 1 : 2);
		timeUntilNextBullet = timeBetweenConsecutiveBullets;
	}

	private void OnTriggerEnter(Collider collider)
	{
		StarfighterBulletEntity component = collider.GetComponent<StarfighterBulletEntity>();
		if (!(component != null))
		{
			return;
		}
		if (state == ShooterBoss.State.FINAL_FIGHT)
		{
			Audio.Play("glowDamageSFX" + UnityEngine.Random.value, DamageSFX, base.transform.position);
			SpawnDebrisParticles();
			ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.4f, 6);
			damageReceived++;
			if (damageReceived > 40)
			{
				ShooterBoss.Instance.OnBrainDestroyed();
			}
		}
		component.RemoveFromWorld();
	}

	private void SpawnDebrisParticles()
	{
		Vector3 position = base.transform.position;
		RotationAngle rotationAngle = new RotationAngle(UnityEngine.Random.value * (float)System.Math.PI * 2f);
		position += rotationAngle.ToVector3() * (UnityEngine.Random.value * 10f + 20f);
		float num = 0.5f + UnityEngine.Random.value * 0.5f;
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		if (objectForType == null)
		{
			Debug.LogError("Could not create <Particle> from objectpool.");
		}
		else
		{
			objectForType.Play("_debris" + UnityEngine.Random.Range(1, 4), position, num, UnityEngine.Random.value * 100f + 100f, true, rotationAngle);
		}
	}

	public override void UpdateState(ShooterBoss.State pState)
	{
		state = pState;
		base.UpdateState(state);
		if (state >= ShooterBoss.State.MOVE_LOWER_BRAIN_STATE)
		{
			scaleX += 0.07f * Time.deltaTime * 30f;
			if ((double)scaleX > 2.5)
			{
				scaleX = 2.5f;
			}
			scaleY += 0.07f * Time.deltaTime * 30f;
			if ((double)scaleY > 2.5)
			{
				scaleY = 2.5f;
			}
		}
		base.transform.localScale = new Vector3(scaleX, 1f, scaleY);
		if (state == ShooterBoss.State.FINAL_FIGHT)
		{
			timeUntilNextBullet -= Time.deltaTime;
			if (timeUntilNextBullet <= 0f)
			{
				ObjectPool.instance.GetObjectForType<ShooterEnemyBulletEntity>().Init(base.transform.position, new Vector2(0f, -3.6f) * 30f, "mrbig_brainbullet", 1f, 10f, true);
				timeUntilNextBullet = timeBetweenConsecutiveBullets;
			}
		}
		if (state == ShooterBoss.State.MEGA_EXPLOSION)
		{
			GetComponentInChildren<Renderer>().enabled = false;
		}
	}
}
