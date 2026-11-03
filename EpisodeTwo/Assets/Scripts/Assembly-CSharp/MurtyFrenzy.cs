using FLD;

public class MurtyFrenzy : MurtyState
{
	private bool playedEntry;

	private bool setTargetSpeed;

	public MurtyFrenzy(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.spinVelocityFactor = 1f;
		murty.wantsToGoFrenzy = false;
		murty.armFactor = 1f;
		murty.reversedSpin = !murty.reversedSpin;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			return new MurtyDying(murty);
		}
		if (frame >= 5f && !playedEntry)
		{
			playedEntry = true;
			Audio.Play("murty entrysound", murty.SfxEntry, murty.transform.position);
		}
		if (frame >= 30f && !setTargetSpeed)
		{
			setTargetSpeed = true;
			murty.targetSpeed = 1f;
		}
		if (frame >= 300f)
		{
			return new MurtyDeactivating(murty);
		}
		frame += dt * 30f;
		return this;
	}
}
