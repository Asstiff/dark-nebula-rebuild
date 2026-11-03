public class EnemySpawnerProducing : EnemySpawnerState
{
	public EnemySpawnerProducing(EnemySpawner spawner)
		: base(spawner)
	{
		timer = 1.5f;
		spawner.SetBlinkFrequency(3f);
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
			Enemy e = spawner.SpawnEnemy();
			return new EnemySpawnerHatchOpening(spawner, e);
		}
		return this;
	}
}
