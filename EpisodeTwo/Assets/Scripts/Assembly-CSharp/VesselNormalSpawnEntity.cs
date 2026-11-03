using System;
using UnityEngine;

public class VesselNormalSpawnEntity : VesselSpawnEntity
{
	public override void Init(Vector3 p, int length, int scrollTime)
	{
		throw new InvalidOperationException("Cannot call this function");
	}

	public override void Init(Vector3 p, bool respawn, int flag, int length, int scrollTime)
	{
		base.Init(p, length, scrollTime);
	}

	private void Spawn()
	{
		StandardVessel standardVessel = World.vessel as StandardVessel;
		standardVessel.Spawn(skinType);
		standardVessel.IsLookingForShield = true;
		standardVessel.DisableMovement();
	}

	public override void Update()
	{
		if (!IsPooled)
		{
			if (!spawned)
			{
				Spawn();
				spawned = true;
			}
			base.Update();
		}
	}

	public override void OnIntroductionDone()
	{
		(World.vessel as StandardVessel).EnableMovement();
	}
}
