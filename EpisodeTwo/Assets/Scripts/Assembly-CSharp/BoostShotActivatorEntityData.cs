using System;
using System.Collections.Generic;

public class BoostShotActivatorEntityData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		ObjectPool.AddAsset<BoostShotBullet>("Assets/Prefabs/Entities/BoostShotBullet.prefab", 1);
		return base.GetAssetsToPreload(pParams);
	}
}
