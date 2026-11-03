using UnityEngine;

public class MurtyPart : Entity
{
	public bool isDead;

	public virtual void SetIsDead(bool dead)
	{
		isDead = dead;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!isDead && collision.gameObject.CompareTag("Player"))
		{
			World.vessel.ReceiveDamage(1);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!isDead && collider.CompareTag("Player"))
		{
			World.vessel.ReceiveDamage(1);
		}
	}

	public void SetLocalPos(Vector2 position)
	{
		base.transform.localPosition = new Vector3(position.x, 0f, position.y);
	}
}
