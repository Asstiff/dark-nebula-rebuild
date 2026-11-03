public class EnemySpawnerIdle : EnemySpawnerState
{
	public EnemySpawnerIdle(EnemySpawner spawner)
		: base(spawner)
	{
	}

	public override void Exit()
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		return new EnemySpawnerLifeMonitor(spawner);
	}
}
