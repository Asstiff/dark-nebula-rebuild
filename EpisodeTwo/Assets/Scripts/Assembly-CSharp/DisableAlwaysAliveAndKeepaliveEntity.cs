using UnityEngine;

public class DisableAlwaysAliveAndKeepaliveEntity : Entity
{
	protected override void Awake()
	{
		base.keepalive = true;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			World.DisableAlwaysAliveAndKeepaliveEntities();
		}
	}
}
