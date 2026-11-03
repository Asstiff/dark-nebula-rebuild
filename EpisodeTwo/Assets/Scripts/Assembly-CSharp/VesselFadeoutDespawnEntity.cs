public class VesselFadeoutDespawnEntity : Entity
{
	public override bool PostInit()
	{
		StandardVessel standardVessel = (StandardVessel)World.vessel;
		standardVessel.DisableMovement();
		World.TriggerEndOfLevel();
		World.TriggerFlag(98);
		return base.PostInit();
	}
}
