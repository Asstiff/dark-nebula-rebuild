using System.Collections.Generic;
using UnityEngine;

public class WallEntityCreator
{
	public static void spawnTileMapCollisionEntities(ILevelEP2 level, TileMap tileMap, int layer, GameObject collisionParent = null)
	{
		if (collisionParent == null)
		{
			collisionParent = GameObject.Find("Collisions");
		}
		if (collisionParent == null)
		{
			collisionParent = new GameObject("Collisions");
		}
		for (ushort num = 0; num < tileMap.Height; num++)
		{
			for (ushort num2 = 0; num2 < tileMap.Width; num2++)
			{
				ushort tile = tileMap.GetTile(num2, num);
				List<TileCollisionInfo> list = tileMap.collisionInfoForTile(tile);
				if (list != null)
				{
					addWallEntities(level, tileMap, list, num2, num, collisionParent.transform);
				}
			}
		}
	}

	private static void addWallEntities(ILevelEP2 level, TileMap tileMap, List<TileCollisionInfo> a, ushort x, ushort y, Transform parent)
	{
		foreach (TileCollisionInfo item in a)
		{
			Vector2 vector = new Vector2(item.pos.x + (float)(int)x * tileMap.tileWidth, item.pos.y + (float)(int)y * tileMap.tileHeight);
			Vector2 vector2 = new Vector2(vector.x - tileMap.origo.x, vector.y);
			GameObject gameObject = null;
			switch (item.type)
			{
			case TypeCollisionInfoType.blueEnergySource:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/EnergySourceEntity.prefab");
				gameObject.GetComponent<EnergySourceCollision>().color = 2;
				break;
			case TypeCollisionInfoType.greenEnergySource:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/EnergySourceEntity.prefab");
				gameObject.GetComponent<EnergySourceCollision>().color = 3;
				break;
			case TypeCollisionInfoType.redEnergySource:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/EnergySourceEntity.prefab");
				gameObject.GetComponent<EnergySourceCollision>().color = 1;
				break;
			case TypeCollisionInfoType.death:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/DeathEntity.prefab");
				break;
			case TypeCollisionInfoType.abyss:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/AbyssEntity.prefab");
				break;
			case TypeCollisionInfoType.water:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/WaterEntity.prefab");
				ObjectPool.AddAsset<WaterSplashEntity>("Assets/Prefabs/Entities/WaterSplashEntity.prefab", 1);
				break;
			default:
				gameObject = new GameObject();
				break;
			}
			gameObject.name = "Collision " + item.type;
			gameObject.transform.position = new Vector3(vector2.x, 128f, 0f - vector2.y);
			gameObject.transform.parent = parent;
			Collider collider;
			if (item.type == TypeCollisionInfoType.circleWall)
			{
				CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.radius = item.width / 2f;
				capsuleCollider.height = 256f + capsuleCollider.radius * 2f;
				collider = capsuleCollider;
			}
			else
			{
				BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.size = new Vector3(item.width, 256f, item.height);
				collider = boxCollider;
			}
			switch (item.type)
			{
			case TypeCollisionInfoType.abyss:
			case TypeCollisionInfoType.water:
				gameObject.tag = "Abyss";
				collider.isTrigger = true;
				break;
			case TypeCollisionInfoType.redEnergySource:
			case TypeCollisionInfoType.blueEnergySource:
			case TypeCollisionInfoType.greenEnergySource:
				collider.isTrigger = true;
				break;
			}
		}
	}
}
