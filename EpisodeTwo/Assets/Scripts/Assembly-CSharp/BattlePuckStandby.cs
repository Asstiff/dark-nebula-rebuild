public class BattlePuckStandby : BattlePuckState
{
	public BattlePuckStandby(BattlePuck p)
		: base(p)
	{
		battlePuck.IsActivated = false;
		battlePuck.SetSpeed(0f);
		battlePuck.IsMovementEnabled = true;
	}

	public override void Exit()
	{
		battlePuck.SetSpeed(1f);
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (battlePuck.IsAwareOfVessel)
		{
			return new BattlePuckActivate(battlePuck);
		}
		return this;
	}
}
