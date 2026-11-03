public class EnemySpawnerSpawning : EnemySpawnerState
{
	private const float spawnTime = 0.6f;

	public EnemySpawnerSpawning(EnemySpawner entity, Enemy spawnedEnemy)
		: base(entity)
	{
		enemy = spawnedEnemy;
		timer = 0f;
	}

	public override void Exit()
	{
		enemy.Scale = 1f;
		(enemy as BattlePuck).IsMovementEnabled = true;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		timer += dt;
		float num = timer / 0.6f;
		num *= num;
		float scale = 0.6f + num * 0.4f;
		enemy.Scale = scale;
		if (spawner.IsDead)
		{
			return new EnemySpawnerDead(spawner);
		}
		if (num >= 1f)
		{
			return new EnemySpawnerHatchClosing(spawner);
		}
		return this;
	}
}
