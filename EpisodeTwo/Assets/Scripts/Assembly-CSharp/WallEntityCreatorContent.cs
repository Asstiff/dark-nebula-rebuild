using System.Collections.Generic;

public class WallEntityCreatorContent
{
	public static void spawnTileMapCollisionEntities(TileMap tileMap, int layer)
	{
		for (ushort num = 0; num < tileMap.Height; num++)
		{
			for (ushort num2 = 0; num2 < tileMap.Width; num2++)
			{
				ushort tile = tileMap.GetTile(num2, num);
				List<TileCollisionInfo> list = tileMap.collisionInfoForTile(tile);
				if (list != null)
				{
					addWallEntities(tileMap, list);
				}
			}
		}
	}

	private static void addWallEntities(TileMap tileMap, List<TileCollisionInfo> a)
	{
		foreach (TileCollisionInfo item in a)
		{
			switch (item.type)
			{
			case TypeCollisionInfoType.redEnergySource:
			case TypeCollisionInfoType.blueEnergySource:
			case TypeCollisionInfoType.greenEnergySource:
				LevelAssets.AddAssetPrefab("Assets/Prefabs/Collisions/EnergySourceEntity.prefab", "EnergySourceEntity");
				break;
			case TypeCollisionInfoType.death:
				LevelAssets.AddAssetPrefab("Assets/Prefabs/Collisions/DeathEntity.prefab", "DeathEntity");
				break;
			case TypeCollisionInfoType.abyss:
				LevelAssets.AddAssetPrefab("Assets/Prefabs/Collisions/AbyssEntity.prefab", "AbyssEntity");
				break;
			case TypeCollisionInfoType.water:
				LevelAssets.AddAssetPrefab("Assets/Prefabs/Collisions/WaterEntity.prefab", "WaterEntity");
				ObjectPool.AddAsset<WaterSplashEntity>("Assets/Prefabs/Entities/WaterSplashEntity.prefab", 1);
				break;
			}
		}
	}
}
