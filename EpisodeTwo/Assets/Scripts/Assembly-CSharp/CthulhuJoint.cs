using System;
using UnityEngine;

public class CthulhuJoint : MonoBehaviour
{
	public Action<int> Hit;

	private int index;

	public new LightEntity light;

	public void SetIndex(int i)
	{
		index = i;
		light.Scale = 4f;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (c.CompareTag("Bullet") && Hit != null)
		{
			Hit(index);
		}
	}
}
