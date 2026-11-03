using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class HomingHardCurrency : MonoBehaviour
{
	[SerializeField]
	private List<AudioClip> _magnetSfxs;

	public Action Collected;

	private float _currentSpeed = 50f;

	private float _magnetMaxSpeed;

	private float _magnetDistance;

	private bool _magnetTriggered;

	private bool _isTaken;

	private int _audioIndex;

	private float maxRotationSpeed = 5f;

	private float _currentRotationSpeed;

	private void Start()
	{
		_magnetMaxSpeed = BoosterManager.MagnetMaxSpeed;
		_magnetDistance = BoosterManager.MagnetDistance;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			CollectHC();
		}
	}

	private void CollectHC()
	{
		Vessel vessel = World.vessel;
		vessel.VesselDied = (Action)Delegate.Remove(vessel.VesselDied, new Action(CollectHC));
		_isTaken = true;
		if (Collected != null)
		{
			Collected();
		}
	}

	private void FixedUpdate()
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
				_audioIndex = (_audioIndex + Random.Range(1, 1000)) % 15;
				Audio.Play("magnet" + _audioIndex, _magnetSfxs[index], base.transform.parent);
			}
			_magnetTriggered = true;
			Vessel vessel = World.vessel;
			vessel.VesselDied = (Action)Delegate.Combine(vessel.VesselDied, new Action(CollectHC));
		}
	}

	private void MoveTowardsVessel()
	{
		if (_currentRotationSpeed <= maxRotationSpeed)
		{
			_currentRotationSpeed += 10f * Time.deltaTime;
			if (_currentRotationSpeed > maxRotationSpeed)
			{
				_currentRotationSpeed = maxRotationSpeed;
			}
		}
		base.transform.Rotate(new Vector3(base.transform.rotation.x, base.transform.rotation.y + _currentRotationSpeed, base.transform.rotation.z));
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
}
