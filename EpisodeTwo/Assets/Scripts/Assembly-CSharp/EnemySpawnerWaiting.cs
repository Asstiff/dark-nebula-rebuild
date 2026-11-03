public class EnemySpawnerWaiting : EnemySpawnerState
{
	public EnemySpawnerWaiting(EnemySpawner spawner)
		: base(spawner)
	{
		timer = spawner.respawntime - 1.5f;
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
		timer -= dt;
		if (timer <= 0f)
		{
			return new EnemySpawnerProducing(spawner);
		}
		return this;
	}
}
