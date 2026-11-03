using UnityEngine;

namespace FLD
{
	public static class GameObjectUtilities
	{
		public static TComponent InstantiateAndGetComponent<TComponent>(this GameObject pPrefab) where TComponent : Component
		{
			GameObject gameObject = (GameObject)Object.Instantiate(pPrefab);
			if (gameObject == null)
			{
				return (TComponent)null;
			}
			TComponent component = gameObject.GetComponent<TComponent>();
			if (component == null)
			{
				Object.Destroy(gameObject);
				return (TComponent)null;
			}
			return component;
		}

		public static T Instantiate<T>(this T pPrefabComponent) where T : Component
		{
			return (T)Object.Instantiate(pPrefabComponent);
		}

		public static T Instantiate<T>(this T pPrefabComponent, Vector3 pPos, Quaternion pOrient) where T : Component
		{
			return (T)Object.Instantiate(pPrefabComponent, pPos, pOrient);
		}

		public static void SetLayerRecursively(this GameObject pObject, int pLayer)
		{
			pObject.layer = pLayer;
			foreach (Transform item in pObject.transform)
			{
				item.gameObject.SetLayerRecursively(pLayer);
			}
		}

		public static void SetVisibleRecursively(this GameObject pRoot, bool pVisible)
		{
			if (!(pRoot == null))
			{
				pRoot.transform.SetVisibleRecursively(pVisible);
			}
		}

		public static void SetVisibleRecursively(this Transform pRoot, bool pVisible)
		{
			if (pRoot == null)
			{
				return;
			}
			Renderer component = pRoot.GetComponent<Renderer>();
			if (component != null)
			{
				component.enabled = pVisible;
			}
			foreach (Transform item in pRoot)
			{
				item.SetVisibleRecursively(pVisible);
			}
		}

		public static void SetRenderQueueRecursively(this GameObject pObject, int pRenderQueue)
		{
			Renderer renderer = pObject.GetComponent<Renderer>();
			if (renderer != null && renderer.material != null)
			{
				renderer.material.renderQueue = pRenderQueue;
			}
			foreach (Transform item in pObject.transform)
			{
				item.gameObject.SetRenderQueueRecursively(pRenderQueue);
			}
		}
	}
}
