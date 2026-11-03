using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class LifeOrb : Entity
{
	[SerializeField]
	private List<AudioClip> _magnetSfxs;

	private static int _audioIndex;

	public AudioClip Sfx;

	private Transform _transform;

	private bool _magnetTriggered;

	private tk2dAnimatedSprite _sprite;

	private bool _isTaken;

	private float _magnetMaxSpeed;

	private float _magnetDistance;

	private float _currentSpeed = 50f;

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

	private void Start()
	{
		_magnetMaxSpeed = BoosterManager.MagnetMaxSpeed;
		_magnetDistance = BoosterManager.MagnetDistance;
		_transform = base.transform;
		World.RegisterLifeOrb();
		if (!base.enabled)
		{
			OnDisable();
		}
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

	private void Update()
	{
		if (_magnetTriggered && !_isTaken)
		{
			MoveTowardsVessel();
		}
		else if (!_isTaken && World.vessel.MagnetBooster && !World.vessel.IsDead && Vector3.Distance(base.transform.position, World.vessel.transform.position) < _magnetDistance)
		{
			if (_magnetSfxs.Count > 0)
			{
				int index = Random.Range(0, _magnetSfxs.Count);
				Audio.Play("magnet" + _audioIndex, _magnetSfxs[index], base.transform);
			}
			_magnetTriggered = true;
			Vessel vessel = World.vessel;
			vessel.VesselDied = (Action)Delegate.Combine(vessel.VesselDied, new Action(CollectOrb));
		}
	}

	private void MoveTowardsVessel()
	{
		base.GetComponent<Rigidbody>().position = Vector3.MoveTowards(base.GetComponent<Rigidbody>().position, World.vessel.transform.position, _currentSpeed * Time.deltaTime);
		float num = World.vessel.CurrentMaxSpeed / 0.35f;
		float num2 = _magnetMaxSpeed * num;
		if (_currentSpeed <= num2)
		{
			_currentSpeed += 1010f * Time.deltaTime;
			if (_currentSpeed > num2)
			{
				_currentSpeed = num2;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_isTaken && other.CompareTag("Player"))
		{
			CollectOrb();
		}
	}

	private void CollectOrb()
	{
		Vessel vessel = World.vessel;
		vessel.VesselDied = (Action)Delegate.Remove(vessel.VesselDied, new Action(CollectOrb));
		if (World.vessel.TriggerLifeOrb(_transform.position))
		{
			Audio.Play("lifeorb" + _audioIndex, Sfx, World.vessel.transform.position);
			_audioIndex = (_audioIndex + 1) % 15;
		}
		SpawnShockwave();
		SpawnParticle();
		DisableRenderer();
		_isTaken = true;
	}

	private void DisableRenderer()
	{
		sprite.GetComponent<Renderer>().enabled = false;
		_sprite.enabled = false;
	}

	public void SpawnParticle()
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play(string.Empty, _transform.position);
	}

	public void SpawnShockwave()
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play(string.Empty, _transform.position);
	}
}
