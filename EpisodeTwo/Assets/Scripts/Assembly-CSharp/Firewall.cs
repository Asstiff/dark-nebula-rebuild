using FLD;
using UnityEngine;

public class Firewall : Entity
{
	public AudioClip SfxVesselExplode;

	public AudioClip SfxLaserDoorExplosion;

	public int num = -1;

	private float _targetDistortion = -1f;

	private float _startDistortion = -1f;

	private Transform _transform;

	private float _nextParticleTime;

	protected override void Awake()
	{
		base.Awake();
		_transform = base.transform;
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.gameObject.CompareTag("Player") && World.vessel.currentEnergy == 1)
		{
			World.vessel.TriggerEnergy(0);
			base.GetComponent<Collider>().enabled = false;
			ObjectPool.instance.GetObjectForType<Particle>().Play("-red", _transform.position);
			Audio.Play("firewall_a", SfxLaserDoorExplosion, _transform.position);
			Audio.Play("firewall_b", SfxVesselExplode, _transform.position);
			if (_startDistortion == -1f)
			{
				_startDistortion = World.distortion;
				_targetDistortion = _startDistortion + 5f;
			}
			World.distortion = _startDistortion + 30f;
		}
	}

	private void Update()
	{
		float num = Time.deltaTime * 30f;
		if (_targetDistortion == -1f)
		{
			return;
		}
		if ((double)World.distortion > (double)_targetDistortion + 0.1 * (double)num)
		{
			World.distortion += (_targetDistortion - World.distortion) * 0.1f * num;
			if (!(Time.time < _nextParticleTime))
			{
				_nextParticleTime = Time.time + 0.033f;
				Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
				if (objectForType != null)
				{
					objectForType.Play("-red", _transform.position + new Vector3(Random.Range(0, 360), 0f, 0f));
				}
			}
		}
		else
		{
			World.distortion = _targetDistortion;
			World.hud.ShowMessage(Language.Get("FIREWALL %d DOWN!", this.num));
			Object.Destroy(base.gameObject);
		}
	}
}
