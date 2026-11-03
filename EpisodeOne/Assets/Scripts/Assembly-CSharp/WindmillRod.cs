using UnityEngine;

public class WindmillRod : Entity
{
	public GameObject Gfx;

	public void SetNumberOfBullets(int numberOfBullets, float bulletSpacing)
	{
		float num = 40f + (float)(numberOfBullets - 1) * bulletSpacing;
		Gfx.transform.localPosition = new Vector3(2f, 0f, 0f);
		Gfx.transform.localScale = new Vector3(num / 64f, 1f, 1f);
	}
}
