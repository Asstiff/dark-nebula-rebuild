public class BattlePuckPatrol : BattlePuckState
{
	public BattlePuckPatrol(BattlePuck p)
		: base(p)
	{
		battlePuck.SetSpeed(0.5f);
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		base.UpdateWithDt(dt);
		if (battlePuck.IsAwareOfVessel)
		{
			return new BattlePuckActivate(battlePuck);
		}
		return this;
	}
}
