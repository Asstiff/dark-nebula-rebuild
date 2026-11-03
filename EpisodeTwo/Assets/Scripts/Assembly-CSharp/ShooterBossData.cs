using System;
using System.Collections.Generic;

public class ShooterBossData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		ObjectPool.AddAsset<ShooterEnemyBulletEntity>("Assets/Prefabs/Entities/ShooterEnemyBulletEntity.prefab", 16);
		return base.GetAssetsToPreload(pParams);
	}
}
