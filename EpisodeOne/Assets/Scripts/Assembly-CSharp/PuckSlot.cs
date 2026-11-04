using FLD;
using UnityEngine;

public class PuckSlot : Entity
{
	private static int audioIndex;

	public AudioClip Sfx;

	public int flag;

	private bool _occupied;

	private tk2dSprite _sprite;

	private void Awake()
	{
		_sprite = GetComponentInChildren<tk2dSprite>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (!_occupied && other.CompareTag("Puck"))
		{
			other.GetComponent<Rigidbody>().linearDamping = 3f;
			Vector2 vector = new Vector2(other.transform.position.x - base.transform.position.x, other.transform.position.z - base.transform.position.z) * 2f;
			if (vector.sqrMagnitude < 1f)
			{
				_occupied = true;
				Object.Destroy(other.gameObject);
				_sprite.spriteId = _sprite.GetSpriteIdByName("puck/1");
				Invoke("PlayEffects", 0.3f);
				World.TriggerFlag(flag);
			}
			else
			{
				other.GetComponent<Rigidbody>().AddForce(new Vector3(0f - vector.x, 0f, 0f - vector.y), ForceMode.Impulse);
			}
		}
	}

	private void PlayEffects()
	{
		_sprite.spriteId = _sprite.GetSpriteIdByName("puck/2");
		ObjectPool.instance.GetObjectForType<Shockwave>().Play(string.Empty, base.transform.position, 0.6f, 15);
		ObjectPool.instance.GetObjectForType<Blast>().Play(base.transform.position, 0.6f);
		Audio.Play("PuckSlot" + audioIndex, Sfx);
		audioIndex = (audioIndex + 1) % 5;
	}
}
