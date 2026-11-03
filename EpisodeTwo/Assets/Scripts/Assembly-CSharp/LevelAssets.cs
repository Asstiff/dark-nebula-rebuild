using System;
using UnityEngine;

public class LevelAssets : MonoBehaviour
{
	[Serializable]
	public class AssetEntry
	{
		[SerializeField]
		public UnityEngine.Object Prefab;

		[SerializeField]
		public string Name;
	}

	public AssetEntry[] Assets;

	private static LevelAssets _instance;

	public static LevelAssets Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.Find("!LevelAssets").GetComponent<LevelAssets>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public static void Reset()
	{
		Instance.Assets = new AssetEntry[0];
	}

	public static AudioClip GetAudio(string pName)
	{
		AudioClip audioClip = GetObject(pName) as AudioClip;
		if (audioClip == null && !string.IsNullOrEmpty(pName))
		{
			Debug.Log("Missing audio " + pName);
		}
		return audioClip;
	}

	public static Transform GetAsset(string pName)
	{
		return GetObject(pName) as Transform;
	}

	public static UnityEngine.Object GetObject(string pName)
	{
		for (int i = 0; i < Instance.Assets.Length; i++)
		{
			if (Instance.Assets[i].Name == pName)
			{
				return Instance.Assets[i].Prefab;
			}
		}
		return null;
	}

	public static bool HasAsset(string pName)
	{
		for (int i = 0; i < Instance.Assets.Length; i++)
		{
			if (Instance.Assets[i].Name == pName)
			{
				return true;
			}
		}
		return false;
	}

	public static void AddAssetPrefab(string path)
	{
		AddAssetPrefab(path, LevelEP2Runtime.GetNameFromPath(path));
	}

	public static void AddAssetPrefab(string path, Type pType)
	{
		AddAssetPrefab(path, LevelEP2Runtime.GetNameFromPath(path), pType);
	}

	public static void AddAssetPrefab(string path, string pName)
	{
		AddAssetPrefab(path, pName, typeof(GameObject));
	}

	public static void AddAssetPrefab(string path, string pName, Type pType)
	{
	}

	public static void AddAsset(string pName, UnityEngine.Object pAsset)
	{
		int num = Instance.Assets.Length + 1;
		AssetEntry[] array = new AssetEntry[num];
		Instance.Assets.CopyTo(array, 0);
		Instance.Assets = array;
		AssetEntry assetEntry = new AssetEntry();
		assetEntry.Prefab = pAsset;
		assetEntry.Name = pName;
		Instance.Assets[num - 1] = assetEntry;
	}
}
