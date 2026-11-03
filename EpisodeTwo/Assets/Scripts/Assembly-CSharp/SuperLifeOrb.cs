using UnityEngine;

public class SuperLifeOrb : Entity
{
	private bool _isOn = true;

	private int maxlives;

	private tk2dAnimatedSprite _animatedSprite;

	private tk2dAnimatedSprite animatedSprite
	{
		get
		{
			if (_animatedSprite == null)
			{
				_animatedSprite = GetComponentInChildren<tk2dAnimatedSprite>();
			}
			return _animatedSprite;
		}
	}

	private void Start()
	{
		maxlives = GetParameterInt("maxlives") + BuildSettings.Current.SuperLifeOffset;
	}

	private void Update()
	{
		if (_isOn && World.vessel != null && !World.vessel.WantsSuperLifeOrbWithMaxLives(maxlives) && World.IsAboutToShowOnScreen(base.transform.position.z, 30f))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isOn && other.CompareTag("Player"))
		{
			turnOff();
			World.vessel.TriggerExtraLife();
			SpawnParticle();
		}
	}

	private void SpawnParticle()
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play("2", World.vessel.transform.position, 2f);
	}

	public void turnOff()
	{
		_isOn = false;
		animatedSprite.Play("SuperLifeOrbOff");
	}
}
