using FLD;
using UnityEngine;

public class ShooterEnemyBulletEntity : Enemy
{
	private Vector2 dir;

	private tk2dSprite sprite;

	public Material NormalMaterial;

	public Material AdditiveMaterial;

	protected override void Awake()
	{
		base.Awake();
		sprite = GetComponentInChildren<tk2dSprite>();
	}

	public void Init(Vector3 enemyPos, Vector2 bulletDir, string name, float scale, float col, bool pIsAdditive)
	{
		dir = bulletDir;
		base.transform.position = enemyPos;
		base.transform.localScale = new Vector3(scale, 1f, scale);
		(base.GetComponent<Collider>() as CapsuleCollider).radius = col / 2f;
		sprite.spriteId = sprite.GetSpriteIdByName(name);
		sprite.GetComponent<Renderer>().material = ((!pIsAdditive) ? NormalMaterial : AdditiveMaterial);
		base.transform.localRotation = Quaternion.Euler(90f, (0f - Math.CreateRotationFromDirection(dir)) * 57.29578f + 180f, 0f);
	}

	public override void Update()
	{
		if (!IsPooled)
		{
			base.Update();
			base.transform.position += new Vector3(dir.x * Time.deltaTime, 0f, dir.y * Time.deltaTime);
			if (!World.EntityWithinScreen(this))
			{
				RemoveFromWorld();
			}
		}
	}

	public override void RemoveFromWorld()
	{
		ObjectPool.instance.PoolObject(this);
	}
}
