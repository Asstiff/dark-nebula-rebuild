using UnityEngine;

public class Shockwave : Entity
{
	private float _scaleSpeed;

	private float _scale = 0.1f;

	private bool _isPlaying;

	private tk2dAnimatedSprite _sprite;

	private Transform _transform;

	private void Awake()
	{
		_sprite = GetComponentInChildren<tk2dAnimatedSprite>();
		_transform = base.transform;
		_sprite.Stop();
	}

	private void Update()
	{
		if (_isPlaying)
		{
			_scale += _scaleSpeed * Time.deltaTime * 30f;
			_transform.localScale = new Vector3(_scale, _scale, 1f);
		}
	}

	public void Play(string type, Vector3 pos, float scaleSpeed = 0.3f, int framesToLive = 10)
	{
		_scale = 0.1f;
		_isPlaying = true;
		float num = (float)framesToLive / 30f;
		Invoke("Remove", num);
		_scaleSpeed = scaleSpeed;
		_transform.position = pos + new Vector3(0f, 30f, 0f);
		_sprite.Play("Shockwave" + type);
		_sprite.CurrentClip.fps = (float)_sprite.CurrentClip.frames.Length / num;
	}

	private void Remove()
	{
		ObjectPool.instance.PoolObject(this);
		_sprite.Stop();
		_isPlaying = false;
	}
}
