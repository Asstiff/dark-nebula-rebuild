using UnityEngine;

public class Blast : Entity
{
	private const float ScaleSpeed = -0.3f;

	private float _scale = 1f;

	private bool _isPlaying;

	private Vector3 scaleVector = Vector3.one;

	private Transform _transform;

	private tk2dSprite _sprite;

	private void Update()
	{
		if (_isPlaying)
		{
			_scale += -0.3f * Time.deltaTime * 30f;
			scaleVector.x = _scale;
			scaleVector.y = _scale;
			_transform.localScale = scaleVector;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_transform = base.transform;
		_sprite = GetComponentInChildren<tk2dSprite>();
	}

	public void Play(Vector3 pos, float scale = 1f, int type = 3)
	{
		_scale = scale;
		scaleVector.x = _scale;
		scaleVector.y = _scale;
		base.gameObject.transform.localScale = scaleVector;
		_isPlaying = true;
		_transform.position = pos + new Vector3(0f, 80f, 0f);
		_sprite.spriteId = _sprite.GetSpriteIdByName("blast/" + type);
		Invoke("Remove", 0.1f);
	}

	private void Remove()
	{
		CancelInvoke("Remove");
		if (_isPlaying)
		{
			_isPlaying = false;
			ObjectPool.instance.PoolObject(this);
		}
	}
}
