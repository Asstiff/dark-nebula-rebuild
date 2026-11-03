public class EnemySpawnerLifeMonitor : EnemySpawnerState
{
	public EnemySpawnerLifeMonitor(EnemySpawner spawner)
		: base(spawner)
	{
		timer = 1.5f;
	}

	public override void Exit()
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (spawner.IsDead)
		{
			return new EnemySpawnerDead(spawner);
		}
		if (spawner.WantsToSpawn())
		{
			return new EnemySpawnerWaiting(spawner);
		}
		return this;
	}
}
