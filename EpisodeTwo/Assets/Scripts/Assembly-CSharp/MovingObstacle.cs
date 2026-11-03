using System;
using UnityEngine;

public class MovingObstacle : Entity, IFreezable
{
	public float amplitude = 2f;

	public float[] speedSetting = new float[2] { 3f, 3f };

	public float phase;

	public bool isVertical;

	private float speed;

	private float originalSpeed;

	private Transform _transform;

	private Rigidbody _rigidbody;

	private bool _isFrozen;

	private float startpos;

	private Vector3 tmp = Vector3.zero;

	protected override void Awake()
	{
		base.Awake();
		World.SignalDifficultyChanged += SetDifficultyTo;
		speed = (originalSpeed = UserSettings.GetDifficultValue(speedSetting));
		_transform = base.transform;
		_rigidbody = base.GetComponent<Rigidbody>();
	}

	private void SetDifficultyTo(float delta)
	{
		speed = delta * originalSpeed;
	}

	private void OnDisable()
	{
		World.SignalDifficultyChanged -= SetDifficultyTo;
	}

	private void Start()
	{
		if (isVertical)
		{
			startpos = _transform.position.z;
		}
		else
		{
			startpos = _transform.position.x;
		}
		tmp = _transform.position;
	}

	private void Update()
	{
		if (!_isFrozen && World.IsFrozen(_transform.position.z))
		{
			World.FreezeManager.AddToFrozenObjects(this);
			_isFrozen = true;
		}
	}

	private void FixedUpdate()
	{
		if (!_isFrozen)
		{
			float num = (float)Math.PI / 5f * speed;
			float num2 = startpos + Mathf.Sin(Time.time * num + phase * (float)Math.PI * 2f) * amplitude;
			if (isVertical)
			{
				tmp.z = num2;
			}
			else
			{
				tmp.x = num2;
			}
			_rigidbody.MovePosition(tmp);
		}
	}

	public new void Defrost()
	{
		_isFrozen = false;
	}
}
