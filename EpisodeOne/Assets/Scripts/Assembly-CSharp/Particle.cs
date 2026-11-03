using Random = UnityEngine.Random;
using System;
using UnityEngine;

public class Particle : Entity
{
	private tk2dAnimatedSprite _sprite;

	private Transform _transform;

	private void Awake()
	{
		_sprite = GetComponentInChildren<tk2dAnimatedSprite>();
		_transform = base.transform;
		_sprite.Stop();
	}

	public void Play(string type, Vector3 pos, float scale = 1.5f)
	{
		float num = scale * 2f;
		_transform.position = pos + new Vector3(0f, 40f, 0f);
		_sprite.Play("Particle" + type);
		_sprite.CurrentClip.fps = 30f;
		tk2dAnimatedSprite sprite = _sprite;
		sprite.animationCompleteDelegate = (tk2dAnimatedSprite.AnimationCompleteDelegate)Delegate.Combine(sprite.animationCompleteDelegate, new tk2dAnimatedSprite.AnimationCompleteDelegate(OnAnimationCompleted));
		_transform.localScale = new Vector3(num, num, 1f);
		float angle = Random.value * 360f * 57.29578f;
		_transform.Rotate(Vector3.back, angle);
	}

	private void OnAnimationCompleted(tk2dAnimatedSprite sprite, int clipId)
	{
		sprite.animationCompleteDelegate = (tk2dAnimatedSprite.AnimationCompleteDelegate)Delegate.Remove(sprite.animationCompleteDelegate, new tk2dAnimatedSprite.AnimationCompleteDelegate(OnAnimationCompleted));
		_sprite.Stop();
		ObjectPool.instance.PoolObject(this);
	}
}
