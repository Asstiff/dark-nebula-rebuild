using UnityEngine;

public class EnemySpawnerHatchOpening : EnemySpawnerState
{
	private const int openFrames = 7;

	public EnemySpawnerHatchOpening(EnemySpawner spawner, Enemy e)
		: base(spawner)
	{
		frame = 0f;
		enemy = e;
		enemy.Scale = 0.6f;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		spawner.SetHatchOpen(6 - (int)frame);
		frame += Time.deltaTime * 30f;
		if (frame >= 7f)
		{
			return new EnemySpawnerSpawning(spawner, enemy);
		}
		return this;
	}
}
