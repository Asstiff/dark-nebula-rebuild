using UnityEngine;

public class TextAttachedEntity : SimpleText
{
	private float _startTime;

	private float _timeToLive = 3.6666667f;

	private float _timeToScale = 3f;

	private Vector3 offset = new Vector3(0f, 80f, 50f);

	private Vector3 _originalCharScale = Vector3.one;

	private bool _initialized;

	private bool _play;

	private void Init()
	{
		if (!_initialized)
		{
			_initialized = true;
			_originalCharScale = base.textMesh.scale;
		}
	}

	private void Start()
	{
		Init();
	}

	public void StartText()
	{
		Init();
		_startTime = Time.time;
		base.textMesh.scale = _originalCharScale;
		base.textMesh.Commit();
		_play = true;
	}

	private void Update()
	{
		if (_play)
		{
			base.transform.position = World.vessel.transform.position + offset;
			if (_startTime + _timeToScale < Time.time)
			{
				base.textMesh.scale -= Vector3.one * 0.05f * Time.deltaTime * 30f;
				base.textMesh.Commit();
			}
			if (_startTime + _timeToLive < Time.time)
			{
				ObjectPool.instance.PoolObject(this);
				_play = false;
			}
		}
	}
}
