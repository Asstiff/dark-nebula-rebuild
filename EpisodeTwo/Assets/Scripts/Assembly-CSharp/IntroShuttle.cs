using UnityEngine;

public class IntroShuttle : Entity
{
	private static string ANIMATION_NAME = "animation";

	private static float DURATION = 6f;

	private void Start()
	{
		if (UserSettings.I.expert)
		{
			GetComponentInChildren<tk2dAnimatedSprite>().Play("IntroShuttleExpert");
		}
		Invoke("Remove", DURATION);
		base.GetComponent<Animation>().Play(ANIMATION_NAME);
	}

	private void Remove()
	{
		base.GetComponent<Animation>().Stop();
		base.transform.position = new Vector3(0f, 1000f, 0f);
	}
}
