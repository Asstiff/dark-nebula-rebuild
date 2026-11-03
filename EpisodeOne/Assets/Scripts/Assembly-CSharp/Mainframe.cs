using FLD;
using UnityEngine;

public class Mainframe : Entity
{
	public AudioClip Sfx;

	private bool _cought;

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			World.vessel.AllowedToMove = false;
			Vector3 vector = base.transform.position + new Vector3(0f, 0f, -164f);
			Vector3 position = World.vessel.transform.position;
			position += new Vector3((vector.x - position.x) * 0.2f, 0f, (vector.z - position.z) * 0.08f) * Time.deltaTime * 30f;
			World.vessel.transform.position = position;
			if (position.z > vector.z)
			{
				World.vessel.transform.position = new Vector3(position.x, position.y, vector.z);
			}
			if ((World.vessel.transform.position - vector).magnitude < 5f && !_cought)
			{
				_cought = true;
				Audio.Play("vesselstop", Sfx);
				Invoke("ShowMessage1", 3.3333333f);
				Invoke("ShowMessage2", 4.5f);
				Invoke("CompleteLevel", 10f);
			}
		}
	}

	private void ShowMessage1()
	{
		World.hud.ShowMessage("ENTERING".Localize());
	}

	private void ShowMessage2()
	{
		World.hud.ShowMessage("THE CORE".Localize());
	}

	private void CompleteLevel()
	{
		World.TriggerEndOfLevel();
	}
}
