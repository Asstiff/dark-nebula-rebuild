using System;
using System.Collections.Generic;

public class ShooterEnemyWaveEntityData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		ObjectPool.AddAsset<TurnerEnemyEntity>("Assets/Prefabs/Entities/TurnerEnemyEntity.prefab", 10);
		ObjectPool.AddAsset<SpinnerEnemyEntity>("Assets/Prefabs/Entities/SpinnerEnemyEntity.prefab", 10);
		ObjectPool.AddAsset<StarfighterWeaponsPickupEntity>("Assets/Prefabs/Entities/StarfighterWeaponsPickupEntity.prefab", 1);
		return base.GetAssetsToPreload(pParams);
	}
}
