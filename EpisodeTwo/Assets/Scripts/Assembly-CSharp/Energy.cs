using UnityEngine;

public class Energy : MonoBehaviour
{
	private void Update()
	{
		base.transform.RotateAround(Vector3.up, 0.25f * Time.deltaTime * 30f);
	}
}
