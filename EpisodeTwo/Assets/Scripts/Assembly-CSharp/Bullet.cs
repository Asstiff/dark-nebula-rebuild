using FLD;
using UnityEngine;

public class Bullet : Entity
{
	public AudioClip Sfx;

	public GameObject Gfx;

	public LightEntity lightEntity;

	private Vector3 _velocity;

	private bool _inited;

	private float _scale = 1f;

	private Transform _gfxTransform;

	private Vector3 scaleVector = Vector3.one;

	private Vector3 _startPosition;

	private float _timeOfFire;

	private bool childedLight;

	private tk2dSprite sprite;

	public void Init(Vector3 pVelocity, float bulletSize, Vector3 pStartPosition, float pTimeOfFire, int frame = 3)
	{
		_timeOfFire = pTimeOfFire;
		_startPosition = pStartPosition;
		_inited = true;
		_velocity = pVelocity;
		_scale = bulletSize / 20f;
		_gfxTransform = Gfx.transform;
		float num = 0.5f + _velocity.magnitude * 0.1f;
		scaleVector.x = _scale;
		scaleVector.y = _scale * num;
		_gfxTransform.localScale = scaleVector;
		SetFrame(frame);
	}

	public void setChildedLight(bool on)
	{
		childedLight = on;
		lightEntity.SetVisible(on);
	}

	public void SetFrame(int frame)
	{
		if (sprite == null)
		{
			sprite = GetComponentInChildren<tk2dSprite>();
		}
		sprite.spriteId = sprite.GetSpriteIdByName("bullets/" + frame);
	}

	private void Update()
	{
		if (_inited && !IsPooled)
		{
			float num = World.WorldTimeInSeconds - _timeOfFire;
			base.GetComponent<Rigidbody>().MovePosition(_startPosition + _velocity * num * 30f);
		}
	}

	private void ReturnToPool()
	{
		base.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
		ObjectPool.instance.PoolObject(this);
		_inited = false;
	}

	private void Die()
	{
		int num = (int)(base.transform.position.x / 64f);
		int num2 = (int)((float)Time.frameCount / 3f);
		Audio.Play("bulletdie" + num + " " + num2, Sfx, base.transform.position);
		ReturnToPool();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_inited && !IsPooled && !collision.gameObject.CompareTag("Turret"))
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				World.vessel.TriggerBulletCollision();
			}
			else if (collision.rigidbody != null)
			{
				collision.rigidbody.AddForce(collision.rigidbody.linearVelocity + _velocity * 16f, ForceMode.VelocityChange);
			}
			if (childedLight)
			{
				ObjectPool.instance.GetObjectForType<Particle>().Play("-green", base.transform.position, 1.3f).SetChildedLight(true);
				ObjectPool.instance.GetObjectForType<Particle>().Play("-green", base.transform.position, 1f).SetChildedLight(true);
			}
			else
			{
				ObjectPool.instance.GetObjectForType<Particle>().Play("2", base.transform.position, 1.3f);
				ObjectPool.instance.GetObjectForType<Particle>().Play("2", base.transform.position, 1f);
			}
			ObjectPool.instance.GetObjectForType<Blast>().Play(base.transform.position, 1.7f);
			Die();
		}
	}
}
