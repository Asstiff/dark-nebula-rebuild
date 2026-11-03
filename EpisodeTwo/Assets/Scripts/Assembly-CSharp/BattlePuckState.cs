public class BattlePuckState : EnemyState
{
	protected BattlePuck battlePuck;

	public BattlePuckState(BattlePuck p)
		: base(p)
	{
		battlePuck = p;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		battlePuck.UpdateNormalMovement();
		return this;
	}
}
