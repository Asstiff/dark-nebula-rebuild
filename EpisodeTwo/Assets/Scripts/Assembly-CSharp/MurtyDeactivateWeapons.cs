public class MurtyDeactivateWeapons : MurtyState
{
	private bool hasFoldedInArms;

	public MurtyDeactivateWeapons(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.collisionGivesDamage = false;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			return new MurtyDying(murty);
		}
		if (frame <= 0f)
		{
			FoldInBlades();
		}
		else if (frame >= 20f && !hasFoldedInArms)
		{
			hasFoldedInArms = true;
			FoldInArms();
		}
		else if (frame > 25f)
		{
			return new MurtyExhausted(murty);
		}
		frame += dt * 30f;
		return this;
	}

	private void FoldInArms()
	{
		murty.armFactor = 0f;
	}

	private void FoldInBlades()
	{
		murty.bladeFactor = 0f;
	}
}
