using UnityEngine;

public class VesselMatrixDespawnEntity : VesselMatrixSpawnEntity
{
	public AudioClip vesselDespawnSound;

	public override void Init(Vector3 p, int doneFlag)
	{
		spawnSound = vesselDespawnSound;
		Init(p, false, doneFlag, 0, 0);
		StandardVessel standardVessel = (StandardVessel)World.vessel;
		standardVessel.DisableMovement();
	}

	public override void Update()
	{
		if (IsAnimationDone())
		{
			base.Update();
			((StandardVessel)World.vessel).Despawn();
		}
		else if (IsEffectDone())
		{
			base.Update();
			((StandardVessel)World.vessel).DisableMovement();
		}
		else
		{
			base.Update();
		}
	}
}
