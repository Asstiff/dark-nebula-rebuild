using UnityEngine;

public class VesselDespawnEntity : Entity
{
	private FlagCondition activateEvent;

	private void Start()
	{
		activateEvent = new FlagCondition(GetParameterInt("activateEventFlag"), GetParameterInt("activateEventCount"));
		activateEvent.Callback += OnActivateEvent;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (activateEvent != null)
		{
			activateEvent.Callback -= OnActivateEvent;
			activateEvent.Dispose();
			activateEvent = null;
		}
	}

	private void OnActivateEvent()
	{
		StandardVessel standardVessel = (StandardVessel)World.vessel;
		Vector3 position = standardVessel.transform.position;
		base.transform.position = position;
		standardVessel.SpawnerType = GetParameterInt("spawnertype");
		standardVessel.InitiateDespawner(GetParameterInt("doneFlag"));
	}
}
