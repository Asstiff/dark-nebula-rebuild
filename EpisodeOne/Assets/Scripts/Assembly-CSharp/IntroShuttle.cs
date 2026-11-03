using UnityEngine;

public class IntroShuttle : Entity
{
	private static string ANIMATION_NAME = "animation";

	private static float DURATION = 6f;

	[SerializeField]
	private tk2dSprite _avatar;

	[SerializeField]
	private tk2dSpriteAnimator _shieldAnimator;

	private void Start()
	{
		if (World.vessel == null)
		{
			Debug.Log("vessel null");
		}
		SetAvatar();
		SetShield();
		Invoke("Remove", DURATION);
		base.GetComponent<Animation>().Play(ANIMATION_NAME);
	}

	private void SetShield()
	{
		if (!(_shieldAnimator == null))
		{
			if (World.vessel.HaveShield)
			{
				_shieldAnimator.gameObject.SetActive(true);
			}
			else
			{
				_shieldAnimator.gameObject.SetActive(false);
			}
		}
	}

	private void SetAvatar()
	{
		if (World.isInMatrixMode && !World.isEndOfMatrix)
		{
			_avatar.SetSprite(World.vessel.VesselAvatar.SpriteCollection, 29);
		}
		else if (UserSettings.Instance.IsExpertMode)
		{
			_avatar.SetSprite(World.vessel.VesselAvatar.SpriteCollection, 30);
		}
		else
		{
			_avatar.SetSprite(World.vessel.VesselAvatar.SpriteCollection, 24);
		}
	}

	private void Remove()
	{
		base.GetComponent<Animation>().Stop();
		base.transform.position = new Vector3(0f, 1000f, 0f);
	}
}
