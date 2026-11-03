using System;
using UnityEngine;

public class SuperLifeOrb : Entity
{
	private bool _isOn = true;

	public int[] maxlivesSetting = new int[2];

	[SerializeField]
	private tk2dSprite _hardCurrencyBoxEmpty;

	[SerializeField]
	private HomingHardCurrency _homingHC;

	private void Start()
	{
		HomingHardCurrency homingHC = _homingHC;
		homingHC.Collected = (Action)Delegate.Combine(homingHC.Collected, new Action(OnCollected));
		World.RegisterSuperLifeOrb(this);
	}

	private void OnCollected()
	{
		UserProgress.Instance.HCSpawnChance--;
		HomingHardCurrency homingHC = _homingHC;
		homingHC.Collected = (Action)Delegate.Remove(homingHC.Collected, new Action(OnCollected));
		if (_isOn)
		{
			TurnOff();
			World.vessel.TriggerGainHC();
			SpawnParticle();
		}
	}

	private void SpawnParticle()
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play("-green", World.vessel.transform.position, 2f);
	}

	public void TurnOff()
	{
		_isOn = false;
		_homingHC.gameObject.SetActive(false);
	}
}
