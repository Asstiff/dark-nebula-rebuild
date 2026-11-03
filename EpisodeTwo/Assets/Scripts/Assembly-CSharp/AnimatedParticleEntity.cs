using FLD;
using UnityEngine;

public class AnimatedParticleEntity : MonoBehaviour
{
	private Vector3 velocity;

	private float power;

	private Particle _particle;

	public void Push(Particle particle, float pAngle, float pSpeed)
	{
		_particle = particle;
		velocity = Math.AngleToVector(pAngle) * pSpeed;
		power = 1f;
	}

	private void Update()
	{
		base.transform.Translate(velocity * power * Time.deltaTime);
		power -= Time.deltaTime;
		if (power <= 0f)
		{
			if (_particle != null)
			{
				_particle.ReturnToObjectPool();
			}
			_particle = null;
		}
	}
}
