using FLD;
using UnityEngine;

public class SoundEntity : Entity
{
	public AudioClip Sfx;

	private FlagCondition activateEvent;

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		Sfx = LevelAssets.GetAudio(GetParameterString("soundName"));
	}

	private void Start()
	{
		activateEvent = new FlagCondition(GetParameterInt("activateEventFlag"), GetParameterInt("activateEventCount"));
		activateEvent.Callback += OnActivated;
	}

	private void OnActivated()
	{
		Audio.Play("sfx" + Random.value, Sfx, transform);
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
