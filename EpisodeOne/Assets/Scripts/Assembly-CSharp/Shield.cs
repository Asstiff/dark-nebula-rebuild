using FLD;
using UnityEngine;

public class Shield : Entity
{
	public AudioClip sfx;

	private bool _isActive = true;

	private bool _isTaken;

	private Renderer _renderer;

	private Vector3 _orgRendererPosition;

	private void Awake()
	{
		_renderer = GetComponentInChildren<Renderer>();
		_orgRendererPosition = _renderer.transform.localPosition;
		Hide();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isActive && other.gameObject.CompareTag("Player"))
		{
			if (World.vessel.TriggerGiveShield())
			{
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
			_renderer.transform.localPosition = Vector3.right * 1000f;
		}
	}
}
