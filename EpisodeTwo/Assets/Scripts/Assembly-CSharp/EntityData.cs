using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityData
{
	public virtual Dictionary<string, Type> GetAssetsToPreload(Dictionary<string, string> pParams)
	{
		return new Dictionary<string, Type>();
	}

	protected Dictionary<string, Type> AddAudio(string key, Dictionary<string, Type> pData, Dictionary<string, string> pParams)
	{
		if (pParams.ContainsKey(key))
		{
			string key2 = "Assets/SourceContent/Ep1/Audio/SFX/" + pParams[key] + ".wav";
			if (!pData.ContainsKey(key2))
			{
				pData.Add(key2, typeof(AudioClip));
			}
		}
		return pData;
	}
}
