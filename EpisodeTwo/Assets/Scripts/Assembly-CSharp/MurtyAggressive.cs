using FLD;

public class MurtyAggressive : MurtyState
{
	public MurtyAggressive(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.spinVelocityFactor = 0.1f;
		murty.Invoke("PlayClickSound", 0.33f);
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			murty.CancelInvoke("PlayClickSound");
			return new MurtyDying(murty);
		}
		float armFactor = frame / 150f;
		murty.armFactor = armFactor;
		if (frame >= 150f)
		{
			murty.wantsToGoFrenzy = true;
			murty.CancelInvoke("PlayClickSound");
			Audio.Play("moving_platform_factory_arrive", murty.SfxEnd);
			return new MurtyPrepareFrenzy(murty);
		}
		frame += dt * 30f;
		return this;
	}
}
