using FLD;
using UnityEngine;

public class WaterSplashEntity : Entity
{
	private const float splashFadeoutScale = 0.4f;

	private const float splashEndScale = 1f;

	private const float rippleFadeoutScale = 0.35f;

	private const float rippleEndScale = 0.5f;

	public tk2dBaseSprite Splash;

	public tk2dBaseSprite Ripple;

	public AudioClip SplashSound;

	private float splashAlpha;

	private float rippleAlpha;

	private float time;

	private float splashScale;

	private float rippleScale;

	public void Play(Vector3 position)
	{
		base.gameObject.SetActive(true);
		base.transform.position = position;
		splashScale = 0f;
		rippleScale = 0f;
		splashAlpha = 1f;
		rippleAlpha = 1f;
		time = 0f;
		Audio.Play("SplashSound", SplashSound);
	}

	private void Update()
	{
		splashScale += 0.04f * Time.deltaTime * 30f;
		if (splashScale > 0.4f)
		{
			splashAlpha = 1f - (splashScale - 0.4f) / 0.6f;
		}
		rippleScale += 0.01f * Time.deltaTime * 30f;
		if (rippleScale > 0.35f)
		{
			rippleAlpha = 1f - (rippleScale - 0.35f) / 0.15f;
		}
		if (time > 60f)
		{
			ReturnToObjectPool();
			return;
		}
		time += Time.deltaTime * 30f;
		Ripple.color = new Color(1f, 1f, 1f, rippleAlpha);
		Ripple.transform.localScale = new Vector3(rippleScale, rippleScale, 1f);
		Splash.color = new Color(1f, 1f, 1f, splashAlpha);
		Splash.transform.localScale = new Vector3(splashScale, splashScale, 1f);
	}

	private void ReturnToObjectPool()
	{
		ObjectPool.instance.PoolObject(this);
		base.gameObject.SetActive(false);
	}
}
