using UnityEngine;

public class EndMorph : MonoBehaviour
{
	public tk2dSprite BigFlake;

	public tk2dSprite SmallFlake1;

	public tk2dSprite SmallFlake2;

	public tk2dSprite Lampor1;

	public tk2dSprite Lampor2;

	public tk2dSprite Gubbe;

	public tk2dSprite BlackBG;

	public bool endMorphSlottedIn;

	public float endMorphSpriteAlpha;

	public float currFrame;

	private float endMorphBackgroundAlpha = 1f;

	private void Awake()
	{
		BigFlake.transform.localScale = new Vector3(2f, 2f, 1f);
		Lampor1.transform.localScale = new Vector3(2f, 2f, 1f);
		Lampor2.transform.localScale = new Vector3(1f, 1f, 1f);
		SmallFlake1.transform.localScale = new Vector3(2.15f, 2.15f, 1f);
		SmallFlake2.transform.localScale = new Vector3(2f, 2f, 1f);
	}

	public void UpdateEndMorphBoss()
	{
		float num = endMorphBackgroundAlpha * endMorphSpriteAlpha;
		BigFlake.color = new Color(1f, 1f, 1f, num * alpha(currFrame + 30f, 60f, 0.3f));
		SmallFlake1.color = new Color(1f, 1f, 1f, num * alpha(currFrame + 45f, 60f, 0.2f) * 0.5f);
		SmallFlake2.color = new Color(1f, 1f, 1f, num * alpha(currFrame, 60f, 0.5f));
		Lampor1.color = new Color(1f, 1f, 1f, num * 0.3f);
		Lampor2.color = new Color(1f, 1f, 1f, num * 0.5f);
		float num2 = -2f * Time.deltaTime * 30f;
		RotateBy(BigFlake, 0.01f * num2);
		RotateBy(Lampor1, 0.005f * num2);
		RotateBy(Lampor2, 0.006f * num2);
		RotateBy(SmallFlake1, 0.007f * num2);
		RotateBy(SmallFlake2, 0.006f * num2);
		if (endMorphSlottedIn)
		{
			endMorphBackgroundAlpha -= 0.01f;
			if (endMorphBackgroundAlpha < 0f)
			{
				endMorphBackgroundAlpha = 0f;
				World.GameCompleted();
			}
		}
	}

	private void RotateBy(tk2dSprite sprite, float radian)
	{
		Vector3 eulerAngles = sprite.transform.localRotation.eulerAngles;
		sprite.transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + radian * 57.29578f);
	}

	private float alpha(float currFrame, float duration, float constant)
	{
		float num = currFrame % duration / duration;
		if (num > 0.5f)
		{
			num = 1f - num;
		}
		num *= 2f;
		return constant + num * (1f - constant);
	}
}
