using System;
using UnityEngine;

public class TurnerEnemyEntity : ShooterEnemyEntity
{
	private tk2dSprite sprite;

	private static int[] spriteIDsRed;

	private static int[] spriteIDs;

	private bool red;

	private float prevDecent;

	public void Init(Vector3 p, float angle, int pIndex, bool pRed)
	{
		sprite = GetComponentInChildren<tk2dSprite>();
		maxHealth = 1;
		base.Init(p, angle, pIndex);
		red = pRed;
		if (red)
		{
			if (spriteIDsRed == null)
			{
				spriteIDsRed = sprite.GetFrameIds("Entity_enemy_shooter_one_red/", 16);
			}
			sprite.spriteId = spriteIDsRed[0];
		}
		else
		{
			if (spriteIDs == null)
			{
				spriteIDs = sprite.GetFrameIds("Entity_enemy_shooter_one/", 16);
			}
			sprite.spriteId = spriteIDs[0];
		}
	}

	public override void Update()
	{
		if (!IsPooled && Time.timeScale != 0f)
		{
			base.Update();
			float num = ((!(angle < 0f)) ? 0f : (Mathf.Ceil((0f - angle) / (float)Math.PI * 2f) * (float)Math.PI * 2f));
			float num2 = 16f * (angle + num % 2f * (float)Math.PI) / ((float)Math.PI * 2f);
			int num3 = Mathf.Clamp((int)num2, 0, 15);
			sprite.spriteId = ((!red) ? spriteIDs[num3] : spriteIDsRed[num3]);
			if (currDecent != prevDecent)
			{
				base.transform.localScale = new Vector3(currDecent, 1f, currDecent);
				prevDecent = currDecent;
			}
			base.transform.localRotation = Quaternion.Euler(90f, 0f, 57.29578f * (0f - angle));
		}
	}

	public override void RemoveFromWorld()
	{
		ObjectPool.instance.PoolObject(this);
		hasRunPostInit = false;
	}
}
