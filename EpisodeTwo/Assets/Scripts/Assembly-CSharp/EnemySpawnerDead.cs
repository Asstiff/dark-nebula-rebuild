public class EnemySpawnerDead : EnemySpawnerState
{
	public EnemySpawnerDead(EnemySpawner spawner)
		: base(spawner)
	{
		spawner.SmashItUp();
	}

	public override void Exit()
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		return this;
	}
}
