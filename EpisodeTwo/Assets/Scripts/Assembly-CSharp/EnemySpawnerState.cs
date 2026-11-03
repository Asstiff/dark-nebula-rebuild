public class EnemySpawnerState : EnemyState
{
	public EnemySpawner spawner;

	public float timer;

	public float frame;

	public EnemySpawnerState(EnemySpawner pSpawner)
		: base(pSpawner)
	{
		spawner = pSpawner;
	}

	public override void Exit()
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		return this;
	}
}
