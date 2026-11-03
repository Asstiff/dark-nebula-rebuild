using UnityEngine;

public class CameraShaker : GameCamera
{
	private FlagConditionChanged shakeCondition;

	private FlagConditionChanged shakeBigCondition;

	private float shakeAmount;

	public CameraShaker(Vector2 p)
		: base(p)
	{
		shakeCondition = new FlagConditionChanged(100);
		shakeCondition.Callback += OnShake;
		shakeBigCondition = new FlagConditionChanged(101);
		shakeBigCondition.Callback += OnShakeBig;
	}

	private void OnDestroy()
	{
		if (shakeCondition != null)
		{
			shakeCondition.Callback -= OnShake;
			shakeCondition.Dispose();
			shakeCondition = null;
		}
		if (shakeBigCondition != null)
		{
			shakeBigCondition.Callback -= OnShakeBig;
			shakeBigCondition.Dispose();
			shakeBigCondition = null;
		}
	}

	public override void UpdateCamera()
	{
		if (shakeAmount == 0f)
		{
			return;
		}
		if (shakeAmount > 0f)
		{
			shakeAmount -= 6f * Time.deltaTime;
			if (shakeAmount <= 0.1f)
			{
				shakeAmount = 0f;
			}
		}
		localPos.x = Random.Range(-1f, 1f) * shakeAmount * 6f;
		localPos.y = Random.Range(-1f, 1f) * shakeAmount * 6f;
	}

	private void OnShake()
	{
		shakeAmount = 2f;
	}

	private void OnShakeBig()
	{
		shakeAmount = 3f;
	}
}
