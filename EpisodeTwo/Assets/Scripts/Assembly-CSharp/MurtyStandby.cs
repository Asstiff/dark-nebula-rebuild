public class MurtyStandby : MurtyState
{
	public MurtyStandby(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.targetSpeed = 0f;
		murty.speed = 0f;
	}

	public override void Exit()
	{
		murty.eyeFactor = 1f;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsAwareOfVessel)
		{
			return new MurtyActivating(murty);
		}
		return this;
	}
}
