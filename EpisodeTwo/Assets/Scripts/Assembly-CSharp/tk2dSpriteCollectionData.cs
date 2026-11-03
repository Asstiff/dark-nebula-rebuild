using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollectionData")]
public class tk2dSpriteCollectionData : MonoBehaviour
{
	public const int CURRENT_VERSION = 3;

	[HideInInspector]
	public int version;

	public bool materialIdsValid;

	[HideInInspector]
	public tk2dSpriteDefinition[] spriteDefinitions;

	private Dictionary<string, int> spriteNameLookupDict;

	[HideInInspector]
	public bool premultipliedAlpha;

	[HideInInspector]
	public Material material;

	public Material[] materials;

	public Texture[] textures;

	[HideInInspector]
	public bool allowMultipleAtlases;

	[HideInInspector]
	public string spriteCollectionGUID;

	[HideInInspector]
	public string spriteCollectionName;

	[HideInInspector]
	public float invOrthoSize = 1f;

	[HideInInspector]
	public float halfTargetHeight = 1f;

	[HideInInspector]
	public int buildKey;

	[HideInInspector]
	public string dataGuid = string.Empty;

	public int Count
	{
		get
		{
			return spriteDefinitions.Length;
		}
	}

	public tk2dSpriteDefinition FirstValidDefinition
	{
		get
		{
			tk2dSpriteDefinition[] array = spriteDefinitions;
			foreach (tk2dSpriteDefinition tk2dSpriteDefinition2 in array)
			{
				if (tk2dSpriteDefinition2.Valid)
				{
					return tk2dSpriteDefinition2;
				}
			}
			return null;
		}
	}

	public int FirstValidDefinitionIndex
	{
		get
		{
			for (int i = 0; i < spriteDefinitions.Length; i++)
			{
				if (spriteDefinitions[i].Valid)
				{
					return i;
				}
			}
			return -1;
		}
	}

	public int GetSpriteIdByName(string name)
	{
		InitDictionary();
		int value = 0;
		spriteNameLookupDict.TryGetValue(name, out value);
		return value;
	}

	public void InitDictionary()
	{
		if (spriteNameLookupDict == null)
		{
			spriteNameLookupDict = new Dictionary<string, int>(spriteDefinitions.Length);
			for (int i = 0; i < spriteDefinitions.Length; i++)
			{
				spriteNameLookupDict[spriteDefinitions[i].name] = i;
			}
		}
	}

	public void InitMaterialIds()
	{
		if (materialIdsValid)
		{
			return;
		}
		int num = -1;
		Dictionary<Material, int> dictionary = new Dictionary<Material, int>();
		for (int i = 0; i < materials.Length; i++)
		{
			if (num == -1 && materials[i] != null)
			{
				num = i;
			}
			dictionary[materials[i]] = i;
		}
		if (num == -1)
		{
			Debug.LogError("Init material ids failed.");
			return;
		}
		tk2dSpriteDefinition[] array = spriteDefinitions;
		foreach (tk2dSpriteDefinition tk2dSpriteDefinition2 in array)
		{
			if (!dictionary.TryGetValue(tk2dSpriteDefinition2.material, out tk2dSpriteDefinition2.materialId))
			{
				tk2dSpriteDefinition2.materialId = num;
			}
		}
		materialIdsValid = true;
	}
}
