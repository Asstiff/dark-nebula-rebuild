using UnityEngine;

public class Puck : Entity
{
	private bool _collisionWithVessel;

	private bool _collisionWithOther;

	private Vector3 _repulse = Vector3.zero;

	private Rigidbody _rigidbody;

	private void Start()
	{
		_rigidbody = base.GetComponent<Rigidbody>();
	}

	private void OnCollisionStay(Collision pCollision)
	{
		if (pCollision.gameObject.layer == 8 && pCollision.contacts.Length > 0)
		{
			_rigidbody.AddForce(pCollision.contacts[0].normal * 10f, ForceMode.Impulse);
		}
		if (pCollision.gameObject.CompareTag("Player"))
		{
			_collisionWithVessel = true;
		}
		else if (pCollision.contacts.Length > 0)
		{
			_collisionWithOther = true;
			_repulse = pCollision.contacts[0].normal * 20f;
		}
	}

	private void FixedUpdate()
	{
		if (_collisionWithVessel && _collisionWithOther)
		{
			_rigidbody.AddForce(_repulse, ForceMode.Impulse);
		}
		_collisionWithVessel = (_collisionWithOther = false);
	}
}
