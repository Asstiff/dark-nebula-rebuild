using FLD;

public class MusicPlayerEntity : Entity
{
	private FlagCondition activateEvent;

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
	}

	private void Start()
	{
		activateEvent = new FlagCondition(GetParameterInt("activateEventFlag"), GetParameterInt("activateEventCount"));
		activateEvent.Callback += OnActivated;
	}

	private void OnActivated()
	{
		if (GetParameterString("musicName") == null)
		{
			Audio.StopMusic(1.5f);
		}
		else
		{
			World.PlayMusic(GetParameterString("musicName"));
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (activateEvent != null)
		{
			activateEvent.Callback -= OnActivated;
			activateEvent.Dispose();
			activateEvent = null;
		}
	}
}
