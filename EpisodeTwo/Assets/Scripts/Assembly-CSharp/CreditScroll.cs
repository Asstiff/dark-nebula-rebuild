using UnityEngine;

public class CreditScroll : MonoBehaviour
{
	public Transform Follower;

	public float Speed = -60f;

	public float WrapPosition = -512f;

	public float Height = 512f;

	private void Update()
	{
		base.transform.position += new Vector3(0f, Time.deltaTime * Speed, 0f);
		if (base.transform.position.y <= WrapPosition)
		{
			base.transform.position += new Vector3(0f, Height, 0f);
		}
		Follower.position = base.transform.position + new Vector3(0f, Height, 0f);
	}
}
