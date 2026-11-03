using FLD;
using UnityEngine;

public class EndOfLevel : Entity
{
	public AudioClip Sfx;

	private bool hasTriggered;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && !World.vessel.IsFalling && !hasTriggered)
		{
			hasTriggered = true;
			Audio.Play("endoflevel", Sfx);
			World.hud.ShowMessage("Level complete".Localize().ToUpper());
			Invoke("TriggerEnd", 3f);
		}
	}

	private void TriggerEnd()
	{
		World.TriggerEndOfLevel();
	}
}
