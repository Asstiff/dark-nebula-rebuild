using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class HomingTurretBullet : Entity
{
	private const float timeUntilFullWidth = 5f;

	public tk2dSprite Fire;

	public tk2dSprite Overlay;

	public tk2dSprite Overlay2;

	public AudioClip dieSound;

	public float bulletSpeed;

	private float counter;

	private float widthScale;

	private float overlayAngle1;

	private float overlayAngle2;

	public float Speed = 1f;

	protected override void Awake()
	{
		base.Awake();
		base.keepalive = true;
		Fire.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
	}

	public override bool PostInit()
	{
		base.keepalive = true;
		counter = 0f;
		return base.PostInit();
	}

	private void CreateShockWave()
	{
		float shockWaveDistance = 100f;
		List<GameObject> list = World.FindEntitiesCloseToMaxDistance(base.transform.position, shockWaveDistance);
		foreach (GameObject item in list)
		{
			if (item.GetComponent<Collider>() != null && item.GetComponent<Rigidbody>() != null && !item.GetComponent<Collider>().isTrigger && item.GetComponent<Rigidbody>().mass != 0f)
			{
				Vector3 force = (base.transform.position - item.gameObject.transform.position).normalized * (10f / item.gameObject.GetComponent<Rigidbody>().mass);
				item.GetComponent<Rigidbody>().AddForce(force);
			}
		}
	}

	public void SetBulletSpeed(float _bulletSpeed)
	{
		bulletSpeed = _bulletSpeed * 20f;
	}

	public void Die()
	{
		base.gameObject.SetActive(false);
		Dead = true;
		ObjectPool.instance.GetObjectForType<Particle>().Play("-red", base.transform.position, 1.3f);
		ObjectPool.instance.GetObjectForType<Particle>().Play("-red", base.transform.position, 1f);
		ObjectPool.instance.GetObjectForType<Blast>().Play(base.transform.position, 1.7f);
		Audio.Play("DieBullet" + UnityEngine.Random.value, dieSound, base.transform.position);
	}

	private void OnCollisionEnter(Collision e)
	{
		if (e.collider.isTrigger)
		{
			return;
		}
		Entity component = e.gameObject.GetComponent<Entity>();
		if (component is Vessel)
		{
			component.GetComponent<Rigidbody>().AddForce(base.GetComponent<Rigidbody>().linearVelocity);
			(component as Vessel).ReceiveDamage(1);
		}
		Die();
		if (!(counter > 3f))
		{
			return;
		}
		if (component is HomingTurret)
		{
			HomingTurret homingTurret = (HomingTurret)component;
			homingTurret.ReceiveDamage(1, new RotationAngle());
			homingTurret.ReceiveDamage(0, new RotationAngle());
			ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", base.transform.position);
			CreateShockWave();
		}
		else if (component is Enemy)
		{
			Enemy enemy = component as Enemy;
			if (enemy.color == 1)
			{
				enemy.ReceiveDamage(3, new RotationAngle());
			}
		}
	}

	private void Update()
	{
		if (!(World.vessel == null))
		{
			if (!World.EntityWithinScreen(this))
			{
				World.vessel.WaypointEnemy(this);
			}
			float z = Mathf.Atan2(base.GetComponent<Rigidbody>().linearVelocity.z, base.GetComponent<Rigidbody>().linearVelocity.x) - (float)System.Math.PI / 2f;
			base.transform.localRotation = Quaternion.Euler(90f, 0f, z);
			Vector3 vector = World.vessel.transform.position - base.transform.position;
			vector.x *= bulletSpeed / 10f;
			vector.y = 0f;
			vector.z *= bulletSpeed / 10f;
			base.GetComponent<Rigidbody>().linearVelocity += vector;
			float magnitude = base.GetComponent<Rigidbody>().linearVelocity.magnitude;
			if (magnitude > bulletSpeed)
			{
				base.GetComponent<Rigidbody>().linearVelocity = base.GetComponent<Rigidbody>().linearVelocity.normalized * bulletSpeed;
			}
			widthScale = counter / 5f;
			if (widthScale > 1f)
			{
				widthScale = 1f;
			}
			counter += Time.deltaTime * 30f;
			overlayAngle1 += 0.2f * Time.deltaTime * 30f;
			overlayAngle2 += 0.1f * Time.deltaTime * 30f;
			Overlay.transform.localScale = new Vector3(0.5f * widthScale, 0.5f * widthScale, 1f);
			Overlay.transform.localRotation = Quaternion.Euler(0f, 0f, overlayAngle1 * 57.29578f);
			Overlay2.transform.localScale = new Vector3(0.3f * widthScale, 0.3f * widthScale, 1f);
			Overlay2.transform.localRotation = Quaternion.Euler(0f, 0f, overlayAngle2 * 57.29578f);
		}
	}

	private void SetSize(float size)
	{
		(base.GetComponent<Collider>() as CapsuleCollider).radius = size / 2f;
	}
}
