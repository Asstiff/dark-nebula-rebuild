using UnityEngine;

public class VesselTriggerEntity : Entity
{
	private bool triggered;

	private void OnTriggerEnter(Collider collider)
	{
		if (!triggered && collider.CompareTag("Player"))
		{
			World.TriggerFlag(GetParameterInt("passTriggerFlag"));
			triggered = true;
		}
	}
}
