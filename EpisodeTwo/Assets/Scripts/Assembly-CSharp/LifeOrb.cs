using FLD;
using UnityEngine;

public class LifeOrb : Entity
{
	private static int _audioIndex;

	public AudioClip Sfx;

	private bool _isTaken;

	private tk2dAnimatedSprite _sprite;

	private tk2dAnimatedSprite sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dAnimatedSprite>();
			}
			return _sprite;
		}
	}

	public override bool PostInit()
	{
		if (base.PostInit())
		{
			return true;
		}
		if (!base.enabled)
		{
			OnDisable();
		}
		return false;
	}

	private void Start()
	{
		World.RegisterLifeOrb();
	}

	private void OnEnable()
	{
		sprite.enabled = !_isTaken;
		sprite.Play();
	}

	private void OnDisable()
	{
		if (sprite != null)
		{
			_sprite.enabled = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_isTaken && other.CompareTag("Player"))
		{
			if (World.vessel.TriggerLifeOrb(base.transform.position))
			{
				Audio.Play("lifeorb" + _audioIndex, Sfx, World.vessel.transform.position);
				_audioIndex = (_audioIndex + 1) % 15;
			}
			SpawnShockwave();
			SpawnParticle();
			DisableRenderer();
			_isTaken = true;
			freezable = false;
		}
	}

	private void DisableRenderer()
	{
		sprite.GetComponent<Renderer>().enabled = false;
		sprite.enabled = false;
	}

	public void SpawnParticle()
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play(string.Empty, base.transform.position);
	}

	public void SpawnShockwave()
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play(string.Empty, base.transform.position);
	}
}
