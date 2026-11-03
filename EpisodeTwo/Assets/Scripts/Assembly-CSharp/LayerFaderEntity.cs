using UnityEngine;

public class LayerFaderEntity : Entity
{
	protected float time;

	protected FlagCondition activateEvent;

	protected bool isDone;

	protected ParallaxLayer tileMap;

	protected CurveFunction curve;

	protected CurveFunction distortionCurve;

	private int distorsionstrength;

	protected override void Awake()
	{
		base.Awake();
		base.keepalive = true;
	}

	private void Start()
	{
		activateEvent = new FlagCondition(GetParameterInt("activateEventFlag"), GetParameterInt("activateEventCount"));
		activateEvent.Callback += OnActivateEvent;
		distorsionstrength = GetParameterInt("distorsionstrength");
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

	private void Update()
	{
		if (!(tileMap == null) && !isDone)
		{
			curve.Update();
			distortionCurve.Update();
			float value = curve.Value;
			float distortion = distortionCurve.Value * (float)distorsionstrength;
			tileMap.Distortion = distortion;
			tileMap.SetAlpha(value);
			if (curve.IsEffectDone() && distortionCurve.IsEffectDone())
			{
				World.TriggerFlag(GetParameterInt("effectDoneTriggerFlag"));
				isDone = true;
			}
		}
	}

	public void OnActivateEvent()
	{
		curve = new CurveFunction(GetParameterFloat("rampIn"), GetParameterFloat("rampOut"), GetParameterFloat("length"));
		distortionCurve = new CurveFunction(GetParameterInt("distorsionrampin"), GetParameterInt("distorsionrampout"), GetParameterInt("distorsionlength"));
		tileMap = World.GetParallaxLayer(GetParameterInt("affectlayer"));
		isDone = false;
		if (tileMap != null)
		{
			tileMap.Enabled = true;
			Update();
		}
		else
		{
			Debug.LogWarning("Can't find layer " + GetParameterInt("affectlayer"));
		}
	}
}
