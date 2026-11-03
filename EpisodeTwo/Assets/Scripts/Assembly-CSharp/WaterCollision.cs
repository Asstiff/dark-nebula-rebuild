public class WaterCollision : AbyssCollision
{
	public override void TriggerFall()
	{
		(World.vessel as StandardVessel).AbyssPositionZ = base.transform.position.y - 94f;
		World.vessel.TriggerFall(true);
	}
}
