using FLD;
using UnityEngine;

public class LaserDoor : Entity
{
	private static int _audioIndex;

	public bool isVertical;

	public int color = 1;

	public AudioClip Sfx;

	private bool _isOpen;

	private tk2dSprite _sprite;

	private void Awake()
	{
		_sprite = GetComponentInChildren<tk2dSprite>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player") && World.vessel.currentEnergy == color)
		{
			Open();
		}
	}

	private void Open()
	{
		if (!_isOpen)
		{
			_isOpen = true;
			World.vessel.TriggerEnergy(0);
			string type = "-red";
			switch (color)
			{
			case 2:
				type = "-blue";
				break;
			case 3:
				type = "-green";
				break;
			}
			ObjectPool.instance.GetObjectForType<Particle>().Play(type, base.transform.position);
			if (isVertical)
			{
				ObjectPool.instance.GetObjectForType<Particle>().Play(type, base.transform.position + new Vector3(0f, 0f, 25f));
				ObjectPool.instance.GetObjectForType<Particle>().Play(type, base.transform.position + new Vector3(0f, 0f, -25f));
				ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position + new Vector3(0f, 0f, -32f), 0.7f, 8);
			}
			else
			{
				ObjectPool.instance.GetObjectForType<Particle>().Play(type, base.transform.position + new Vector3(25f, 0f, 0f));
				ObjectPool.instance.GetObjectForType<Particle>().Play(type, base.transform.position + new Vector3(50f, 0f, 0f));
				ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position + new Vector3(32f, 0f, 0f), 0.7f, 8);
			}
			Audio.Play("laserdoor" + _audioIndex, Sfx, base.transform);
			_audioIndex = (_audioIndex + 1) % 5;
			Object.Destroy(base.GetComponent<Collider>());
			color = 0;
			SetDoorColor();
		}
	}

	private void SetDoorColor()
	{
		string text = "laserDoor";
		if (isVertical)
		{
			text += "Vertical";
		}
		text = text + "/" + color;
		_sprite.SetSprite(text);
	}

	private void UpdateGfx()
	{
		if (!_isOpen && !(_sprite == null))
		{
			SetDoorColor();
		}
	}
}
