using System;
using System.Collections.Generic;
using UnityEngine;

public class WindmillData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		Dictionary<string, Type> assetsToPreload = base.GetAssetsToPreload(pParams);
		assetsToPreload.Add("Assets/Prefabs/Entities/WindmillRod.prefab", typeof(GameObject));
		assetsToPreload.Add("Assets/Prefabs/Entities/WindmillBullet.prefab", typeof(GameObject));
		return assetsToPreload;
	}
}
