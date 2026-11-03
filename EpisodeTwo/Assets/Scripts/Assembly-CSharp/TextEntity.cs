using UnityEngine;

public class TextEntity : SimpleText
{
	private float _startTime;

	private float _timeToLive = 1.5f;

	private float _timeToScale = 1.3f;

	private Vector3 _screenPos;

	private Vector3 _originalCharScale = Vector3.one;

	private bool _inited;

	private bool IsActive;

	private void Init()
	{
		if (!_inited)
		{
			_inited = true;
			_originalCharScale = base.textMesh.charScale;
			base.alwaysAlive = true;
			freezable = false;
		}
	}

	private void StartText()
	{
		Init();
		_startTime = Time.time;
		_screenPos = World.cameraFollow.GetComponent<Camera>().transform.position - base.gameObject.transform.position;
		base.textMesh.text = base.textMesh.text;
		base.textMesh.charScale = _originalCharScale;
		base.textMesh.Commit();
		IsActive = true;
		base.keepalive = true;
	}

	public override void SetText(string text)
	{
		base.SetText(text);
		StartText();
	}

	public override void SetStringID(string stringID)
	{
		base.SetStringID(stringID);
		StartText();
	}

	public void LateUpdate()
	{
		if (!IsActive)
		{
			return;
		}
		if (_startTime + _timeToLive < Time.time)
		{
			IsActive = false;
			ObjectPool.instance.PoolObject(this);
			return;
		}
		_screenPos += new Vector3(0f, 0f, -0.85f * Time.deltaTime * 30f);
		base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 0f - _screenPos.z + World.cameraFollow.GetComponent<Camera>().transform.position.z);
		if (_startTime + _timeToScale < Time.time)
		{
			base.textMesh.charScale -= new Vector3(0.1f, 0.1f, 0.1f) * Time.deltaTime * 30f;
			base.textMesh.Commit();
		}
	}
}
