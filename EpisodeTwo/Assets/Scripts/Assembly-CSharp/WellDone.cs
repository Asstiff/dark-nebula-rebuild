using FLD;
using UnityEngine;

public class WellDone : Entity
{
	private static float lastWellDone = -1f;

	protected override void Awake()
	{
		base.Awake();
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
				empty = Language.Get("Excellent!");
				break;
			case 1:
				empty = Language.Get("Perfect!");
				break;
			case 2:
				empty = Language.Get("Superb!");
				break;
			default:
				empty = Language.Get("Outstanding!");
				break;
			}
			World.hud.ShowMessage(empty);
			Audio.Play("extralife" + Random.value, World.vessel.SfxExtraLife, World.vessel.transform);
		}
	}
}
