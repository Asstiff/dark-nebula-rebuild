using UnityEngine;

public class DeathCollision : MonoBehaviour
{
	private void OnCollisionStay(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.CompareTag("Player"))
		{
			World.vessel.TriggerDeathCollision();
		}
	}
}
