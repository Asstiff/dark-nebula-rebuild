using UnityEngine;

public class StarfighterWeaponsPickupEntity : Entity
{
	public tk2dSprite TwirlSprite;

	public tk2dSprite Sprite;

	private float angle;

	private float spriteFrame;

	private void Update()
	{
		if (!IsPooled)
		{
			angle += 0.03f;
			spriteFrame += 0.5f;
			if (spriteFrame > 15f)
			{
				spriteFrame = 0f;
			}
			Vector3 zero = Vector3.zero;
			StarfighterCamera starfighterCamera = (StarfighterCamera)World.CameraManager.FindHighestPriorityCamera();
			if (starfighterCamera != null)
			{
				zero.z -= starfighterCamera.Speed * 0.4f * Time.deltaTime * 30f;
				base.transform.position = base.transform.position + zero;
				Sprite.spriteId = Sprite.GetSpriteIdByName("starfighterweapons_pickup/" + (int)spriteFrame);
				TwirlSprite.transform.localRotation = Quaternion.Euler(0f, 0f, angle * 57.29578f);
			}
		}
	}

	public void Init(Vector3 p)
	{
		base.transform.position = p;
		TwirlSprite.color = new Color(1f, 1f, 1f, 0.5f);
		TwirlSprite.transform.localScale = new Vector3(0.75f, 1f, 0.75f);
		angle = 0f;
		spriteFrame = 0f;
	}

	public override void RemoveFromWorld()
	{
		ObjectPool.instance.PoolObject(this);
	}
}
