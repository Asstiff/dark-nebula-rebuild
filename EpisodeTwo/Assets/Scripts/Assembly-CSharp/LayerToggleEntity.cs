public class LayerToggleEntity : Entity
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
		World.SetLayerEnabled(GetParameterInt("affectlayer"), GetParameterBool("layerstate"));
	}
}
