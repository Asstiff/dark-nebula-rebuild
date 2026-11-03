using UnityEngine;

public class PlayWhenVisible : MonoBehaviour
{
	private tk2dAnimatedSprite _sprite;

	private bool _isStarted;

	private void Awake()
	{
		_sprite = GetComponent<tk2dAnimatedSprite>();
		_sprite.Stop();
	}

	private void OnBecameVisible()
	{
		if (!_isStarted && !(_sprite == null))
		{
			_isStarted = true;
			_sprite.Play();
		}
	}

	private void OnBecameInvisible()
	{
		if (_isStarted && !(_sprite == null))
		{
			_isStarted = false;
			_sprite.Pause();
		}
	}
}
