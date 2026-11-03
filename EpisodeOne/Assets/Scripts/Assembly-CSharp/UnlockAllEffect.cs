using System;
using System.Collections;
using FLD;
using UnityEngine;

public class UnlockAllEffect : MonoBehaviour
{
	public GameObject GlowSprite;

	public GameObject Spinner;

	public GameObject UnlockParticle;

	public AudioClip UnlockSound;

	public bool AutoStart = true;

	public Action EffectComplete;

	private tk2dSprite _glowSprite;

	private Material _spinner;

	private ParticleSystem _unlockParticle;

	public float UnlockDelay
	{
		get
		{
			return 1.6600001f;
		}
	}

	private void Start()
	{
		_glowSprite = GlowSprite.GetComponent<tk2dSprite>();
		_spinner = Spinner.GetComponentInChildren<MeshRenderer>().sharedMaterial;
		_unlockParticle = UnlockParticle.GetComponent<ParticleSystem>();
		_glowSprite.transform.localScale = Vector3.one * 1f;
		SetGlowAlpha(0f);
		SetSpinnerAlpha(0f);
		if (AutoStart)
		{
			Invoke("StartAnimation", 1f);
		}
	}

	public void StartAnimation()
	{
		StartCoroutine("Animate");
	}

	private IEnumerator Animate()
	{
		Audio.Play("Unlock", UnlockSound);
		yield return new WaitForSeconds(0.25f);
		iTween.ScaleTo(_glowSprite.gameObject, iTween.Hash("scale", Vector3.one * 1.3f, "time", 0.65f, "easetype", iTween.EaseType.easeOutSine));
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", 0.8f, "easetype", iTween.EaseType.easeOutSine, "onupdate", "SetGlowAlpha"));
		Invoke("FadeOutGlow", 0.66f);
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "delay", 0.33f, "time", 0.8f, "easetype", iTween.EaseType.easeOutSine, "onupdate", "SetSpinnerAlpha"));
		Invoke("FadeOutSpinner", 1.13f);
		yield return new WaitForSeconds(0.33f);
		_unlockParticle.Play();
		yield return new WaitForSeconds(2f);
		if (EffectComplete != null)
		{
			EffectComplete();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void FadeOutGlow()
	{
		iTween.ScaleTo(_glowSprite.gameObject, iTween.Hash("scale", Vector3.one * 1f, "time", 0.8f, "easetype", iTween.EaseType.easeInSine));
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 0.8f, "easetype", iTween.EaseType.easeInSine, "onupdate", "SetGlowAlpha"));
	}

	private void FadeOutSpinner()
	{
		iTween.ValueTo(base.gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 0.8f, "easetype", iTween.EaseType.easeInSine, "onupdate", "SetSpinnerAlpha"));
	}

	private void Update()
	{
		if (!(Spinner == null))
		{
			Spinner.transform.Rotate(Vector3.forward * 10f * Time.deltaTime);
		}
	}

	private void SetGlowAlpha(float pAlpha)
	{
		_glowSprite.color = new Color(_glowSprite.color.r, _glowSprite.color.g, _glowSprite.color.b, pAlpha);
	}

	private void SetSpinnerAlpha(float pAlpha)
	{
		_spinner.color = new Color(_spinner.color.r, _spinner.color.g, _spinner.color.b, pAlpha);
	}
}
