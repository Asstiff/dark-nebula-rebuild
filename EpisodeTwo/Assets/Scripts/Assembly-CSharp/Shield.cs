using FLD;
using UnityEngine;

public class Shield : Entity
{
	private bool _isActive = true;

	private bool _isTaken;

	public AudioClip sfx;

	private Renderer _renderer;

	private Vector3 _orgRendererPosition;

	private tk2dSpriteAnimation _sprite;

	protected override void Awake()
	{
		base.Awake();
		_renderer = GetComponentInChildren<Renderer>();
		_sprite = GetComponentInChildren<tk2dSpriteAnimation>();
		_orgRendererPosition = _renderer.transform.localPosition;
	}

	private void OnEnable()
	{
		if (_sprite != null)
		{
			_sprite.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (_sprite != null)
		{
			_sprite.enabled = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isActive && other.gameObject.CompareTag("Player"))
		{
			if (World.vessel.TriggerGiveShield())
			{
                GameHaptics.Play(HapticEvent.Pickup);
				Audio.Play("shieldup", sfx, World.vessel.transform);
			}
			Hide();
			_isTaken = true;
		}
	}

	public void Show()
	{
		if (!_isActive && !_isTaken)
		{
			_isActive = true;
			_renderer.transform.localPosition = _orgRendererPosition;
		}
	}

	public void Hide()
	{
		if (_isActive)
		{
			_isActive = false;
			_renderer.transform.localPosition = Vector3.right * 10000f;
		}
	}
}
