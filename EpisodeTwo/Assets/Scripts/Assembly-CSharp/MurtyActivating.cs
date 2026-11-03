using FLD;

public class MurtyActivating : MurtyState
{
	public MurtyActivating(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		Audio.Play("MustryActivating", murty.SfxActivating, murty.transform.position);
		murty.eyeFactor = 1f;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		frame += dt * 30f;
		if (frame >= 65f)
		{
			return new MurtyActivateWeapons(murty);
		}
		return this;
	}
}
