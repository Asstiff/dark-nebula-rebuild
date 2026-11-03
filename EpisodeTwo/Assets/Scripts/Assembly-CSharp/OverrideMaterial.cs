using UnityEngine;

[RequireComponent(typeof(tk2dBaseSprite))]
public class OverrideMaterial : MonoBehaviour
{
	public Material Material;

	private void Start()
	{
		GetComponent<tk2dBaseSprite>().GetComponent<Renderer>().material = Material;
	}
}
