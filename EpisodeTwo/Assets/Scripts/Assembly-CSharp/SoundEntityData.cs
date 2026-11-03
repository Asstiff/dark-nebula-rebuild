using System;
using System.Collections.Generic;

public class SoundEntityData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		return AddAudio("soundName", base.GetAssetsToPreload(pParams), pParams);
	}
}
