using System;
using System.Collections.Generic;

public class StarfighterVesselRespawnEntityData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		ObjectPool.AddAsset<StarfighterVesselEntity>("Assets/Prefabs/Entities/StarfighterVesselEntity.prefab", 1);
		ObjectPool.AddAsset<StarfighterBulletEntity>("Assets/Prefabs/Entities/StarfighterBulletEntity.prefab", 14);
		return base.GetAssetsToPreload(pParams);
	}
}
