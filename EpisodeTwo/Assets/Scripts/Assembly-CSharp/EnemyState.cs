public class EnemyState : StateMachineState
{
	protected Enemy enemy;

	public EnemyState(Enemy e)
	{
		enemy = e;
	}

	public override void Exit()
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		return this;
	}
}
