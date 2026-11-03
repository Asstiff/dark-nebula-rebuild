public class BattlePuckSpawn : BattlePuckState
{
	public BattlePuckSpawn(BattlePuck p)
		: base(p)
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (battlePuck.IsMovementEnabled)
		{
			return new BattlePuckPatrol(battlePuck);
		}
		return this;
	}
}
