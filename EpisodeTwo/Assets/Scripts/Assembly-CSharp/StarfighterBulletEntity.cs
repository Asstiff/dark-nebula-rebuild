using FLD;
using UnityEngine;

public class StarfighterBulletEntity : Entity
{
	private Vector2 dir;

	public void SetDirection(Vector2 pDir)
	{
		dir = pDir;
		dir.y = 0f - dir.y;
		Vector3 v = new Vector3(dir.x, 0f, dir.y);
		base.transform.localRotation = Quaternion.Euler(0f, (0f - Math.CreateRotationFromDirection(v)) * 57.29578f - 90f, 0f);
	}

	private void Update()
	{
		if (!IsPooled)
		{
			Vector3 position = base.transform.position;
			position.x += 20f * dir.x * Time.deltaTime * 30f;
			position.z += 20f * dir.y * Time.deltaTime * 30f;
			base.transform.position = position;
			if (!World.EntityWithinScreen(this))
			{
				ReturnToPool();
			}
		}
	}

	public override void RemoveFromWorld()
	{
		ReturnToPool();
	}

	private void ReturnToPool()
	{
		ObjectPool.instance.PoolObject(this);
	}
}
