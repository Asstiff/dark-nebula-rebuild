using System;
using UnityEngine;

public class Particle : Entity
{
	private tk2dAnimatedSprite _sprite;

	private Transform _transform;

	private AnimatedParticleEntity anim;

	public LightEntity lightEntity;

	protected override void Awake()
	{
		base.Awake();
		_sprite = GetComponentInChildren<tk2dAnimatedSprite>();
		_transform = base.transform;
		_sprite.Stop();
		SetChildedLight(false);
	}

	public void Play(string type, Vector3 pos, float scale, float pSpeed, bool shouldLoop)
	{
		Play(type, pos, scale, pSpeed, shouldLoop, new RotationAngle(UnityEngine.Random.value * (float)Math.PI * 2f));
	}

	public void SetChildedLight(bool on)
	{
		lightEntity.SetVisible(on);
	}

	public void Play(string type, Vector3 pos, float scale, float pSpeed, bool shouldLoop, RotationAngle angle)
	{
		Play(type, pos, scale, shouldLoop, angle);
		anim = base.gameObject.GetComponent<AnimatedParticleEntity>();
		if (anim == null)
		{
			anim = base.gameObject.AddComponent<AnimatedParticleEntity>();
		}
		else
		{
			anim.enabled = true;
		}
		anim.Push(this, UnityEngine.Random.value * (float)Math.PI * 2f, pSpeed);
		if (shouldLoop)
		{
			_sprite.SetFrame(UnityEngine.Random.Range(0, _sprite.CurrentClip.frames.Length));
		}
	}

	public Particle Play(string type, Vector3 pos, float scale = 1.5f)
	{
		Play(type, pos, scale, false, new RotationAngle(UnityEngine.Random.value * (float)Math.PI * 2f));
		return this;
	}

	public void Play(string type, Vector3 pos, float scale, bool shouldLoop, RotationAngle angle)
	{
		float num = scale * 2f;
		_transform.position = pos + new Vector3(0f, 40f, 0f);
		_sprite.Play("Particle" + type);
		if (_sprite.CurrentClip == null)
		{
			Debug.LogError("Missing animation Particle" + type);
			ReturnToObjectPool();
			return;
		}
		_sprite.CurrentClip.fps = 30f;
		CancelInvoke("ReturnToObjectPool");
		if (!shouldLoop)
		{
			Invoke("ReturnToObjectPool", AnimationLength(_sprite.CurrentClip));
		}
		_transform.localScale = new Vector3(num, num, 1f);
		_sprite.transform.localEulerAngles = new Vector3(0f, 0f, angle.Degrees);
	}

	public float AnimationLength(tk2dSpriteAnimationClip currentClip)
	{
		return (float)currentClip.frames.Length / currentClip.fps;
	}

	public void ReturnToObjectPool()
	{
		if (!IsPooled)
		{
			SetChildedLight(false);
			_sprite.Stop();
			if (anim != null)
			{
				anim.enabled = false;
			}
			ObjectPool.instance.PoolObject(this);
		}
	}
}
