public class MurtyDying : MurtyState
{
	private int lastExplosion;

	public MurtyDying(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.spinVelocityFactor = 0f;
		murty.targetSpeed = 0f;
		murty.speed = 0f;
		murty.collisionGivesDamage = false;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (frame == 0f)
		{
			murty.PrepareDeadLook();
		}
		if ((int)frame / 9 != lastExplosion)
		{
			lastExplosion = (int)frame / 9;
			RandomExplosion();
		}
		else if (frame >= 60f)
		{
			murty.SwitchToDeadLook();
			return new MurtyDead(murty);
		}
		frame += dt * 30f;
		return this;
	}

	private void RandomExplosion()
	{
		murty.SpawnRandomExplosion();
	}
}
