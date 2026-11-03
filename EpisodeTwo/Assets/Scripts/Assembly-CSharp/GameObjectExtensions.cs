using UnityEngine;

public static class GameObjectExtensions
{
	public static T GetOrAddComponent<T>(this GameObject pGO) where T : MonoBehaviour
	{
		T val = pGO.GetComponent<T>();
		if (val == null)
		{
			val = pGO.AddComponent<T>();
		}
		return val;
	}

	public static T GetOrAddComponent<T>(this MonoBehaviour pMonoBehaviour) where T : MonoBehaviour
	{
		return pMonoBehaviour.gameObject.GetOrAddComponent<T>();
	}
}
