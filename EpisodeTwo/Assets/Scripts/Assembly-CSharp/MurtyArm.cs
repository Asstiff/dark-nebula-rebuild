using UnityEngine;

public class MurtyArm : MurtyPart
{
	public float HandDistanceFactor;

	public float BladeFactor;

	public MurtyHand Hand;

	public tk2dSprite innerBody;

	private float currentHandDistanceFactor;

	private void Update()
	{
		float num = HandDistanceFactor - currentHandDistanceFactor;
		currentHandDistanceFactor += num * 25f * Time.deltaTime;
		Vector2 vector = default(Vector2);
		vector.x = currentHandDistanceFactor * 60f;
		vector.y = 0f;
		Hand.SetLocalPos(-vector);
	}

	public override void SetIsDead(bool dead)
	{
		base.SetIsDead(dead);
		Hand.isDead = dead;
	}
}
