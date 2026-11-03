using FLD;
using UnityEngine;

public class MurtyActivateWeapons : MurtyState
{
	private bool hasFoldedOutBlades;

	public MurtyActivateWeapons(MurtyHellicasterEntity pMurty)
		: base(pMurty)
	{
		murty.eyeFactor = 1f;
		murty.collisionGivesDamage = true;
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		if (murty.IsDead)
		{
			return new MurtyDying(murty);
		}
		if (frame <= 0f)
		{
			FoldOutArms();
		}
		else if (frame >= 3f && !hasFoldedOutBlades)
		{
			hasFoldedOutBlades = true;
			FoldOutBlades();
		}
		else if (frame >= 8f)
		{
			return new MurtyAggressive(murty);
		}
		frame += Time.deltaTime * 30f;
		return this;
	}

	private void FoldOutArms()
	{
	}

	private void FoldOutBlades()
	{
		Audio.Play("bladeFoldOutSound", murty.SfxBladeFoldOutSound);
		FoldOutBladesFactor(1f);
	}

	private void FoldOutArmsFactor(float factor)
	{
	}

	private void FoldOutBladesFactor(float factor)
	{
		murty.bladeFactor = factor;
	}
}
