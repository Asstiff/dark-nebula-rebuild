using FLD;
using UnityEngine;

public class SpeedZone : Entity
{
	public AudioClip Sfx;

	private float speed;

	private void Start()
	{
		speed = GetParameterFloat("speed");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !World.vessel.IsFalling && !float.IsInfinity(speed) && !float.IsNaN(speed))
		{
			if (speed != 0f && (World.vessel as StandardVessel).ForcedSpeed == 0f)
			{
				Audio.Play("speedzone", Sfx);
				World.hud.ShowMessage(Language.Get("SPEED ZONE!"));
			}
			(World.vessel as StandardVessel).SetForcedVelocity(new Vector2(0f, speed * 20f * 2f), false);
		}
	}
}
