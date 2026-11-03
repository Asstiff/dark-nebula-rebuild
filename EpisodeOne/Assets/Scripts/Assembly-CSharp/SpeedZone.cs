using System.Collections.Generic;
using FLD;
using UnityEngine;

public class SpeedZone : Entity
{
	public AudioClip Sfx;

	[SerializeField]
	private List<float> speedSetting = new List<float>();

	private float speed;

	private void Awake()
	{
		speed = speedSetting[UserSettings.Instance.IsExpertMode ? 1 : 0];
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !World.vessel.IsFalling && !float.IsInfinity(speed) && !float.IsNaN(speed))
		{
			if (speed != 0f && World.cameraFollow.targetScrollSpeed == 0f)
			{
				Audio.Play("speedzone", Sfx);
				World.hud.ShowMessage("SPEED ZONE!".Localize());
			}
			World.cameraFollow.targetScrollSpeed = speed;
		}
	}
}
