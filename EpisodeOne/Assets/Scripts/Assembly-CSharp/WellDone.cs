using FLD;
using UnityEngine;

public class WellDone : Entity
{
	private static float lastWellDone = -1f;

	private void Awake()
	{
		lastWellDone = -1f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !World.vessel.IsFalling && (lastWellDone == -1f || !(lastWellDone >= base.transform.position.z)))
		{
			lastWellDone = base.transform.position.z;
			string empty = string.Empty;
			switch (Random.Range(0, 3))
			{
			case 0:
				empty = "Excellent!".Localize().ToUpper();
				break;
			case 1:
				empty = "Perfect!".Localize().ToUpper();
				break;
			case 2:
				empty = "Superb!".Localize().ToUpper();
				break;
			default:
				empty = "Outstanding!".Localize().ToUpper();
				break;
			}
			World.hud.ShowMessage(empty);
			Audio.Play("extralife", World.vessel.SfxExtraLife, World.vessel.transform);
		}
	}
}
