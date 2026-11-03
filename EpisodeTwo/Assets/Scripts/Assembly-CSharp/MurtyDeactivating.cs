public class MurtyDeactivating : MurtyState
{
	public MurtyDeactivating(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.spinVelocityFactor = 0f;
		murty.targetSpeed = 0f;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			return new MurtyDying(murty);
		}
		if (murty.spinVelocity <= 0.1f)
		{
			return new MurtyDeactivateWeapons(murty);
		}
		return this;
	}
}
