using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEntityData : EntityData
{
	public override Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		Dictionary<string, Type> assetsToPreload = base.GetAssetsToPreload(pParams);
		AddSound(assetsToPreload, pParams, "takeOffSoundName");
		AddSound(assetsToPreload, pParams, "arriveSoundName");
		assetsToPreload.Add("Assets/SourceContent/Ep1/Audio/SFX/LaserWall7.wav", typeof(AudioClip));
		return assetsToPreload;
	}

	private void AddSound(Dictionary<string, Type> p, Dictionary<string, string> pParams, string pParamKey)
	{
		if (pParams.ContainsKey(pParamKey))
		{
			string text = pParams[pParamKey];
			if (!string.IsNullOrEmpty(text))
			{
				p.Add("Assets/SourceContent/Ep1/Audio/SFX/" + text + ".wav", typeof(AudioClip));
			}
		}
	}
}
