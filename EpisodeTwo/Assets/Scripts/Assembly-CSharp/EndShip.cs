using FLD;
using UnityEngine;

public class EndShip : Entity
{
	public AudioClip Sfx;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			DarkNebulaMetrics.EndLevel(World.levelNum, UserSettings.I.expert);
			Audio.Play("endlevel", Sfx);
			World.vessel.transform.position = base.transform.position + new Vector3(128f, 0f, -32f);
			World.LoadLevel("EndOfGame", 3.3333333f);
			Audio.PauseMusic(3.3333333f);
		}
	}
}
