public class MurtyWarning : MurtyState
{
	public MurtyWarning(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.showWarning = true;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		frame += dt * 30f;
		if (frame >= 60f)
		{
			return new MurtyFrenzy(murty);
		}
		return this;
	}
}
