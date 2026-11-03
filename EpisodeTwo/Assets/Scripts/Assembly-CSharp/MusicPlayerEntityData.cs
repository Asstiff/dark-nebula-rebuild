using System;
using System.Collections.Generic;

public class MusicPlayerEntityData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		Dictionary<string, Type> assetsToPreload = base.GetAssetsToPreload(pParams);
		World worldInstance = World.GetWorldInstance();
		if (worldInstance != null && pParams.ContainsKey("musicName"))
		{
			worldInstance.AddMusicClip(pParams["musicName"]);
		}
		return assetsToPreload;
	}
}
