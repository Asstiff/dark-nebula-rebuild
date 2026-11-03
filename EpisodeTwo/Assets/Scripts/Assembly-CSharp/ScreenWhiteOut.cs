using UnityEngine;

public class ScreenWhiteOut : MonoBehaviour
{
	private FlagConditionChanged whiteOutCondition;

	private FlagConditionChanged longWhiteOutCondition;

	private CameraFade _fade;

	private float fadeoutDuration;

	protected Color color = Color.white;

	public CameraFade fade
	{
		get
		{
			if (_fade == null && _fade == null)
			{
				_fade = base.gameObject.AddComponent<CameraFade>();
			}
			return _fade;
		}
	}

	private void Awake()
	{
		whiteOutCondition = new FlagConditionChanged(102);
		whiteOutCondition.Callback += OnWhiteout;
		longWhiteOutCondition = new FlagConditionChanged(103);
		longWhiteOutCondition.Callback += OnLongWhiteout;
	}

	private void OnDestroy()
	{
		if (whiteOutCondition != null)
		{
			whiteOutCondition.Callback -= OnWhiteout;
			whiteOutCondition.Dispose();
			whiteOutCondition = null;
		}
		if (longWhiteOutCondition != null)
		{
			longWhiteOutCondition.Callback -= OnLongWhiteout;
			longWhiteOutCondition.Dispose();
			longWhiteOutCondition = null;
		}
	}

	private void OnWhiteout()
	{
		Color colorFrom = color;
		colorFrom.a = 0f;
		fade.StartFade(colorFrom, color, 1f / 15f);
		fadeoutDuration = 1f;
		Invoke("FadeOut", 1f / 15f);
	}

	private void FadeOut()
	{
		Color colorTo = color;
		colorTo.a = 0f;
		fade.StartFade(color, colorTo, fadeoutDuration);
	}

	private void OnLongWhiteout()
	{
		Color colorFrom = color;
		colorFrom.a = 0f;
		fade.StartFade(colorFrom, color, 2f / 3f);
		fadeoutDuration = 2f / 3f;
		Invoke("FadeOut", 2f);
	}
}
