using UnityEngine;

public class MainMenuSpinner : MonoBehaviour
{
	public float rotationScale = 1f;

	private float rotation;

	private void Update()
	{
		rotation -= 7f * Time.deltaTime;
		base.transform.rotation = Quaternion.AngleAxis(rotation * rotationScale, Vector3.forward);
	}
}
