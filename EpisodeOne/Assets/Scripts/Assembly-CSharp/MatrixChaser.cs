using UnityEngine;

public class MatrixChaser : Entity
{
	private Transform _transform;

	private Rigidbody _rigidbody;

	private bool _isFrozen;

	public bool IsFrozen
	{
		get
		{
			return _isFrozen;
		}
		set
		{
			_isFrozen = value;
		}
	}

	private void Start()
	{
		_transform = base.transform;
		_rigidbody = base.GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (!_isFrozen)
		{
			_rigidbody.MovePosition(_transform.position + new Vector3(0f, 0f, 120f * Time.deltaTime));
		}
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (!_isFrozen && pCollider.CompareTag("Player"))
		{
			World.vessel.TriggerSpike();
		}
	}
}
