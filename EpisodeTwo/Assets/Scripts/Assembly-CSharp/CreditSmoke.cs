using UnityEngine;

public class CreditSmoke : MonoBehaviour
{
	public float Speed = 0.1f;

	private void Update()
	{
		base.transform.RotateAround(Vector3.forward, Time.deltaTime * Speed);
	}
}
