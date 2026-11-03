using UnityEngine;

public class SpinnerEnemyEntity : ShooterEnemyEntity
{
	private tk2dSprite sprite;

	private static int[] spriteIds;

	protected override void Awake()
	{
		base.Awake();
		sprite = GetComponentInChildren<tk2dSprite>();
		if (spriteIds == null)
		{
			spriteIds = sprite.GetFrameIds("Entity_enemy_shooter_two/", 32);
		}
	}

	public override void RemoveFromWorld()
	{
		ObjectPool.instance.PoolObject(this);
		hasRunPostInit = false;
	}

	public override void Init(Vector3 p, float angle, int pIndex)
	{
		base.Init(p, angle, pIndex);
		maxHealth = 1;
		sprite.spriteId = sprite.GetSpriteIdByName("Entity_enemy_shooter_two/0");
	}

	public override void Update()
	{
		if (!IsPooled)
		{
			base.Update();
			float num = Time.time * 30f % 32f;
			sprite.spriteId = spriteIds[(int)num];
			base.transform.localScale = new Vector3(currDecent, 1f, currDecent);
			base.transform.localRotation = Quaternion.Euler(90f, 0f, 57.29578f * (0f - angle));
		}
	}
}
