using UnityEngine;

public class EnemySpawnerHatchClosing : EnemySpawnerState
{
	private const int closeFrames = 7;

	public EnemySpawnerHatchClosing(EnemySpawner entity)
		: base(entity)
	{
		frame = 0f;
	}

	public override void Exit()
	{
		spawner.TurnBlinkOff();
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		spawner.SetHatchOpen((int)frame);
		if (spawner.IsDead)
		{
			return new EnemySpawnerDead(spawner);
		}
		frame += Time.deltaTime * 30f;
		if (frame >= 7f)
		{
			return new EnemySpawnerLifeMonitor(spawner);
		}
		return this;
	}
}
