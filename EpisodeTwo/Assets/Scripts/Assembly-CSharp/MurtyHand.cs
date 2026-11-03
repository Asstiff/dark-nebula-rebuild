using UnityEngine;

public class MurtyHand : MurtyPart
{
	public float ExtendedFactor;

	public bool ReversedDirection;

	public float BladeFactor;

	public tk2dSprite outerBody;

	protected bool invertY;

	public MurtyBlade blade;

	public MurtySpike spike;

	protected float currentBladeFactor;

	private void Update()
	{
		currentBladeFactor += (BladeFactor - currentBladeFactor) * 10f * Time.deltaTime;
		blade.transform.localScale = new Vector3(1f, 1f, currentBladeFactor);
		spike.transform.localScale = new Vector3(1f, 1f, currentBladeFactor);
	}

	private void SetReversedDirection(bool r)
	{
		blade.ReversedDirection = r;
	}

	public override void SetIsDead(bool dead)
	{
		base.SetIsDead(dead);
		blade.isDead = dead;
		spike.isDead = dead;
	}
}
