using System;
using FLD;
using UnityEngine;

public class BoostShotBullet : Entity
{
	public int color;

	public int damage;

	public AudioClip DieSound;

	private float angle;

	private Vector3 direction;

	private tk2dBaseSprite _sprite;

	private bool isActive;

	public tk2dBaseSprite Sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dBaseSprite>();
			}
			return _sprite;
		}
	}

	public void Fire(Vector3 pos, Vector3 pDirectionSpeed, int pColor, int pDamage)
	{
		base.transform.position = pos;
		damage = pDamage;
		color = pColor;
		direction = pDirectionSpeed;
		angle = Mathf.Atan2(direction.z, direction.x);
		base.transform.rotation = Quaternion.Euler(0f, -90f + (0f - angle) * 57.29578f, 0f);
		UpdateGfx();
		isActive = true;
	}

	private void FixedUpdate()
	{
		if (isActive)
		{
			base.GetComponent<Rigidbody>().MovePosition(base.transform.position + direction * Time.deltaTime * 30f);
		}
	}

	private void UpdateGfx()
	{
		Sprite.spriteId = Sprite.GetSpriteIdByName("boost_shot_bullet/" + color);
	}

	private void OnTriggerEnter(Collider c)
	{
		if (!isActive)
		{
			return;
		}
		Entity component = c.gameObject.GetComponent<Entity>();
		if (component is Enemy)
		{
			Enemy enemy = (Enemy)component;
			if ((color == 0 && enemy.color == 1) || (color == 1 && enemy.color == 3) || (color == 2 && enemy.color == 2))
			{
				RotationAngle impactDirectionRadian = new RotationAngle(angle + (float)System.Math.PI / 2f);
				enemy.ReceiveDamage(Mathf.Max(1, damage), impactDirectionRadian);
			}
		}
		else if (!c.gameObject.CompareTag("Player") && !c.gameObject.GetComponent<Collider>().isTrigger)
		{
			string type = "-red";
			switch (color)
			{
			case 1:
				type = "-green";
				break;
			case 2:
				type = "-blue";
				break;
			}
			ObjectPool.instance.GetObjectForType<Particle>().Play(type, base.transform.position, 1.3f);
			Audio.Play("DieBoostShot", DieSound);
			ObjectPool.instance.PoolObject(this);
			isActive = false;
		}
	}
}
