using UnityEngine;

public class CreditVinjett : MonoBehaviour
{
	private float Delay = 7f;

	private float alpha;

	private float targetAlpha = 0.5f;

	private float tweenTimeleft;

	private float startAlpha;

	private float tweenDuration;

	private float tweenDelta;

	private float startTime;

	private tk2dSprite sprite;

	private void Start()
	{
		sprite = GetComponent<tk2dSprite>();
		startTime = Time.time;
	}

	private void Update()
	{
		if (startTime + Delay < Time.time)
		{
			tweenTimeleft -= Time.deltaTime;
			if (tweenTimeleft <= 0f)
			{
				alpha = targetAlpha;
				startAlpha = targetAlpha;
				targetAlpha = Random.value * 0.6f;
				tweenDuration = Random.Range(0.3f, 1f);
				tweenTimeleft = tweenDuration;
				tweenDelta = targetAlpha - alpha;
			}
			else
			{
				alpha = startAlpha + tweenDelta * tweenTimeleft;
			}
		}
		sprite.color = new Color(1f, 1f, 1f, alpha);
	}
}
