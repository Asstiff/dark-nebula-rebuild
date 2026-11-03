using System;
using UnityEngine;

public class FishPart : Entity
{
	public int index;

	public tk2dSprite Bone;

	public tk2dSprite Cap;

	public event Action<int, Transform> Hit;

	private void OnCollisionEnter(Collision e)
	{
		if (e.gameObject.CompareTag("Player"))
		{
			World.vessel.ReceiveDamage(1);
		}
	}

	private void OnTriggerEnter(Collider e)
	{
		if (e.CompareTag("Bullet") && Hit != null)
		{
			Hit(index, base.transform);
		}
	}

	public void Die()
	{
		base.GetComponent<Rigidbody>().freezeRotation = true;
		base.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
		base.GetComponent<Collider>().enabled = false;
		UpdateGfx(4);
	}

	public void UpdateGfx(int health)
	{
		Cap.spriteId = Cap.GetSpriteIdByName("fish-parts/" + GetSpriteId(index, health));
		Bone.spriteId = Bone.GetSpriteIdByName("fish-bone/" + (index + ((health == 4) ? 4 : 0)));
	}

	private static int GetSpriteId(int part, int health)
	{
		int num = 3 - part;
		return health + num * 5;
	}
}
