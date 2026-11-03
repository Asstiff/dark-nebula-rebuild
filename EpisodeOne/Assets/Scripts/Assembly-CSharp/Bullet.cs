using FLD;
using UnityEngine;

public class Bullet : Entity
{
	public AudioClip Sfx;

	public GameObject Gfx;

	private Vector3 _velocity;

	private bool _inited;

	private float _scale = 1f;

	private Transform _transform;

	private Transform _gfxTransform;

	private CameraFollow _cameraFollow;

	private Vector3 scaleVector = Vector3.one;

	private Vector3 _startPosition;

	private float _timeOfFire;

	public void Init(Vector3 pVelocity, float bulletSize, Vector3 pStartPosition, float pTimeOfFire)
	{
		_timeOfFire = pTimeOfFire;
		_startPosition = pStartPosition;
		_inited = true;
		_cameraFollow = World.cameraFollow;
		_transform = base.transform;
		_velocity = pVelocity;
		_scale = bulletSize / 20f;
		_gfxTransform = Gfx.transform;
		float num = 0.5f + _velocity.magnitude * 0.1f;
		scaleVector.x = _scale;
		scaleVector.y = _scale * num;
		_gfxTransform.localScale = scaleVector;
	}

	private void Update()
	{
		if (_inited)
		{
			float num = Time.time - _timeOfFire;
			_transform.position = _startPosition + _velocity * num * 30f;
			if (_transform.position.x < -20f)
			{
				ReturnToPool();
			}
			else if (_transform.position.x > 340f)
			{
				ReturnToPool();
			}
			else if (_velocity.z > 0f && _transform.position.z > _cameraFollow.windowPosZ + (float)Constants.VirtualScreenHeight + 600f)
			{
				ReturnToPool();
			}
			else if (_velocity.z < 0f && _transform.position.z < _cameraFollow.windowPosZ - 600f)
			{
				ReturnToPool();
			}
		}
	}

	private void ReturnToPool()
	{
		base.GetComponent<Rigidbody>().velocity = Vector3.zero;
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
		if (_inited && !collision.gameObject.CompareTag("Turret"))
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				World.vessel.TriggerBulletCollision();
			}
			else if (collision.rigidbody != null)
			{
				collision.rigidbody.AddForce(collision.rigidbody.velocity + _velocity * 16f, ForceMode.VelocityChange);
			}
			ObjectPool.instance.GetObjectForType<Particle>().Play("2", base.transform.position, 1.3f);
			ObjectPool.instance.GetObjectForType<Particle>().Play("2", base.transform.position, 1f);
			ObjectPool.instance.GetObjectForType<Blast>().Play(base.transform.position, 1.7f);
			Die();
		}
	}
}
