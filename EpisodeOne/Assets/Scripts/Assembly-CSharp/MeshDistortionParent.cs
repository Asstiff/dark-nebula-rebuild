using UnityEngine;

public class MeshDistortionParent : MonoBehaviour
{
	private void Start()
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			if (meshRenderer.gameObject.GetComponent<MeshDistortion>() == null)
			{
				meshRenderer.gameObject.AddComponent<MeshDistortion>();
			}
		}
	}
}
