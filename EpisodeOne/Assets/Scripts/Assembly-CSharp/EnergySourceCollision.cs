using FLD;
using UnityEngine;

public class EnergySourceCollision : MonoBehaviour
{
	public const int NONE = 0;

	public const int RED = 1;

	public const int BLUE = 2;

	public const int GREEN = 3;

	public int color;

	public AudioClip Sfx;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && World.vessel.TriggerEnergy(color))
		{
			Audio.Play("energy", Sfx, World.vessel.transform);
			string type = "Red";
			switch (color)
			{
			case 2:
				type = "Blue";
				break;
			case 3:
				type = "Green";
				break;
			}
			Shockwave objectForType = ObjectPool.instance.GetObjectForType<Shockwave>();
			objectForType.Play(type, World.vessel.Gfx.transform.position, 0.3f, 8);
			objectForType.transform.parent = World.vessel.transform;
		}
	}
}
