using System.Collections.Generic;
using UnityEngine;

public class WallEntityCreatorBlocks
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
		int num = -1;
		ushort num2 = 0;
		bool flag = false;
		for (ushort num3 = 0; num3 < tileMap.Width; num3++)
		{
			num = -1;
			num2 = 0;
			for (ushort num4 = 0; num4 < tileMap.Height; num4++)
			{
				ushort tile = tileMap.GetTile(num3, num4);
				List<TileCollisionInfo> list = tileMap.collisionInfoForTile(tile);
				flag = ((list != null && list.Count == 1 && list[0].type != TypeCollisionInfoType.circleWall && list[0].height == tileMap.tileHeight && list[0].width == tileMap.tileWidth) ? true : false);
				if (num != tile || num4 % 20 == 0 || !flag)
				{
					if (num != -1)
					{
						List<TileCollisionInfo> list2 = tileMap.collisionInfoForTile((ushort)num);
						if (list2 != null)
						{
							addWallEntities(level, tileMap, list2, num3, (ushort)(num4 - 1), collisionParent.transform, num2, layer);
						}
					}
					num = tile;
					num2 = num4;
				}
			}
			List<TileCollisionInfo> list3 = tileMap.collisionInfoForTile((ushort)num);
			if (list3 != null)
			{
				addWallEntities(level, tileMap, list3, num3, (ushort)(tileMap.Height - 1), collisionParent.transform, num2, layer);
			}
		}
	}

	private static void addWallEntities(ILevelEP2 level, TileMap tileMap, List<TileCollisionInfo> a, ushort x, ushort yend, Transform parent, ushort ystart, int layer)
	{
		float z = (float)(yend - ystart + 1) * tileMap.tileHeight;
		float num = (float)(yend - ystart) * tileMap.tileHeight / 2f;
		foreach (TileCollisionInfo item in a)
		{
			Vector2 vector = new Vector2(item.pos.x + (float)(int)x * tileMap.tileWidth, item.pos.y + (float)(int)ystart * tileMap.tileHeight);
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
				gameObject.GetComponent<AbyssCollision>().LayerHeight = (float)layer * 5f;
				break;
			case TypeCollisionInfoType.water:
				gameObject = level.InstatiatePrefab("Assets/Prefabs/Collisions/WaterEntity.prefab");
				gameObject.GetComponent<AbyssCollision>().LayerHeight = (float)layer * 5f;
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
				if (item.height == tileMap.tileHeight && item.width == tileMap.tileWidth)
				{
					boxCollider.size = new Vector3(item.width, 256f, z);
					collider = boxCollider;
					vector2.y += num;
					gameObject.transform.position = new Vector3(vector2.x, 128f, 0f - vector2.y);
				}
				else
				{
					boxCollider.size = new Vector3(item.width, 256f, item.height);
					collider = boxCollider;
				}
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
			collider.gameObject.isStatic = true;
		}
	}
}
