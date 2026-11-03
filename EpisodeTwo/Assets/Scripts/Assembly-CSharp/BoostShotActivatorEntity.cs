using FLD;
using UnityEngine;

public class BoostShotActivatorEntity : Entity
{
	public AudioClip TriggerSound;

	public int color;

	private int damage;

	private float speed;

	private tk2dBaseSprite _sprite;

	public tk2dBaseSprite Sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dBaseSprite>();
			}
			return _sprite;
		}
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		speed = GetParameterFloat("speed");
		damage = GetParameterInt("damage");
		color = GetParameterInt("color");
		UpdateGfx();
		ObjectPool.AddAsset<BoostShotBullet>("Assets/Prefabs/Entities/BoostShotBullet.prefab", 1);
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		speed = GetParameterFloat("speed");
		damage = GetParameterInt("damage");
		color = GetParameterInt("color");
		UpdateGfx();
	}

	private void UpdateGfx()
	{
		Sprite.spriteId = Sprite.GetSpriteIdByName("boost_shot_activator/" + color);
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.CompareTag("Player"))
		{
			Vector3 velocityThisFrame = ((StandardVessel)World.vessel).velocityThisFrame;
			velocityThisFrame.y = 0f;
			velocityThisFrame.Normalize();
			if (velocityThisFrame.x == 0f && velocityThisFrame.z == 0f)
			{
				velocityThisFrame.x = 1f;
			}
			ObjectPool.instance.GetObjectForType<BoostShotBullet>().Fire(base.transform.position, velocityThisFrame * speed, color, damage);
			ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.6f, 9);
			Audio.Play("TriggerBoost", TriggerSound);
		}
	}
}
