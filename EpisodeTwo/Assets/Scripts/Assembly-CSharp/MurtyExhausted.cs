public class MurtyExhausted : MurtyState
{
	public MurtyExhausted(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.showWarning = false;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			return new MurtyDying(murty);
		}
		if (frame >= 5f)
		{
			murty.reversedSpin = !murty.reversedSpin;
			return new MurtyActivateWeapons(murty);
		}
		frame += dt * 30f;
		return this;
	}
}
