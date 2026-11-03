using FLD;
using UnityEngine;

public class Spike : Entity
{
	public AudioClip Sfx;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player") && !World.vessel.IsInAir)
		{
			Audio.Play("spike", Sfx, base.transform);
			World.vessel.TriggerSpike();
		}
	}
}
