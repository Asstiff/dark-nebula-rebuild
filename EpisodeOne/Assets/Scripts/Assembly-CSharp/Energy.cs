using UnityEngine;

public class Energy : Entity
{
	private void Update()
	{
		float angle = 0.25f * Time.deltaTime * 30f * 57.29578f;
		base.transform.Rotate(Vector3.back, angle);
	}
}
