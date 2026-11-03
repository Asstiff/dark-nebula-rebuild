using FLD;
using UnityEngine;

public class ExplosionEntity : Entity
{
	public AudioClip SfxExplotion;

	public void Explode(Vector3 position, float scale)
	{
		Audio.Play("Explosion" + Random.value, SfxExplotion, position);
		ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", position, scale);
		ObjectPool.instance.PoolObject(this);
	}
}
