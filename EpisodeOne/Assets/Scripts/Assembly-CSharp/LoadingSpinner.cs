using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
	private float _angle;

	private void Update()
	{
		_angle += Time.deltaTime / 1.75f;
		float num = Mathf.Floor(_angle * 12f) / 12f;
		base.transform.localRotation = Quaternion.AngleAxis(num * 360f, Vector3.forward);
	}
}
