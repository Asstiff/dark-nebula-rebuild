using UnityEngine;

public class MatrixChaser : Entity
{
	private Transform _transform;

	private Rigidbody _rigidbody;

	private void Start()
	{
		_transform = base.transform;
		_rigidbody = base.GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (!IsFrozen)
		{
			_rigidbody.MovePosition(_transform.position + new Vector3(0f, 0f, 120f * Time.deltaTime));
		}
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (!IsFrozen && pCollider.CompareTag("Player"))
		{
			World.vessel.TriggerSpike();
		}
	}
}
