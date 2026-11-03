public class MurtyPrepareFrenzy : MurtyState
{
	public MurtyPrepareFrenzy(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.spinVelocityFactor = 0f;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			return new MurtyDying(murty);
		}
		if (murty.spinVelocity <= 0.1f)
		{
			return new MurtyWarning(murty);
		}
		return this;
	}
}
