using System;
using FLD;
using UnityEngine;

public class VesselBullet : Entity
{
	public tk2dSprite Gfx;

	public AudioClip SfxActivated;

	public AudioClip SfxDeactivated;

	public LightEntity lightEntity;

	[HideInInspector]
	public int color;

	private RotationAngle rotation = new RotationAngle();

	private float rotationSpeed;

	private float bulletRadius;

	private float targetBulletRadius;

	private float radiusSpeed;

	private float addedBulletRadius;

	private float normalRadius;

	private float fadeOutRadius;

	private float time;

	private bool isActive;

	private bool isCollisionEnabled;

	public void Init(int pColor, bool hasLightEntity)
	{
		time = 0f;
		normalRadius = 110f;
		fadeOutRadius = 210f;
		targetBulletRadius = normalRadius;
		bulletRadius = 0f;
		color = pColor;
		radiusSpeed = 5f;
		rotationSpeed = 0.03f;
		isCollisionEnabled = true;
		isActive = true;
		base.transform.localPosition = Vector3.zero;
		Gfx.spriteId = Gfx.GetSpriteIdByName("vesselbullet_" + Constants.GetColorString(color));
		Audio.Play("vesselbullet", SfxActivated);
		Shockwave objectForType = ObjectPool.instance.GetObjectForType<Shockwave>();
		objectForType.Play(string.Empty, World.vessel.Gfx.transform.position, 0.3f, 8);
		objectForType.transform.parent = World.vessel.transform;
		lightEntity.SetVisible(hasLightEntity);
	}

	private void FixedUpdate()
	{
		if (isActive)
		{
			bulletRadius += (targetBulletRadius - bulletRadius) * Time.deltaTime * radiusSpeed;
			addedBulletRadius = 1f * bulletRadius;
			if (targetBulletRadius <= normalRadius)
			{
				addedBulletRadius *= bulletRadius / targetBulletRadius;
			}
			if (Mathf.Abs(bulletRadius - fadeOutRadius) < 5f)
			{
				ReturnToObjectPool();
				return;
			}
			time += Time.deltaTime;
			rotation.Radians += rotationSpeed * (float)System.Math.PI * 2f * Time.deltaTime * 30f;
			base.transform.rotation = Quaternion.AngleAxis(rotation.Degrees, Vector3.up);
		}
	}

	private void Update()
	{
		if (bulletRadius > normalRadius && targetBulletRadius == fadeOutRadius)
		{
			float a = 1f - (bulletRadius - normalRadius) / (fadeOutRadius - normalRadius);
			Gfx.color = new Color(1f, 1f, 1f, a);
		}
		else
		{
			Gfx.color = new Color(1f, 1f, 1f, 1f);
		}
		float num = addedBulletRadius / 112f;
		base.transform.localScale = new Vector3(num, 1f, num);
	}

	public void ReturnToObjectPool()
	{
		isActive = false;
		ObjectPool.instance.PoolObject(this);
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (isCollisionEnabled)
		{
			Enemy component = pCollider.gameObject.GetComponent<Enemy>();
			if (component != null && component.color == color)
			{
				component.ReceiveDamage(1, rotation);
			}
		}
	}

	public void Deactivate()
	{
		Audio.Play("SfxDeactivated", SfxDeactivated);
		targetBulletRadius = fadeOutRadius;
		radiusSpeed = 10f;
		isCollisionEnabled = false;
	}
}
