using UnityEngine;

public class VesselMatrixSpawnEntityAnimation
{
	private float startScale;

	private float endScale;

	private float startRotation;

	private float endRotation;

	private float startAlpha;

	private float endAlpha;

	private tk2dSprite sprite;

	private GameObject gameObject;

	private float scale;

	private float alpha;

	private float rotation;

	public VesselMatrixSpawnEntityAnimation(float ss, float es, float sr, float er, float sf, float ef, GameObject s)
	{
		startScale = ss;
		endScale = es;
		startRotation = sr;
		endRotation = er;
		startAlpha = sf;
		endAlpha = ef;
		gameObject = s;
		sprite = s.GetComponent<tk2dSprite>();
	}

	public void Update(float factor)
	{
		float num = factor * factor;
		scale = (endScale - startScale) * num;
		scale += startScale;
		scale = 0.5f + scale * scale;
		alpha = (endAlpha - startAlpha) * factor;
		alpha += startAlpha;
		rotation = (endRotation - startRotation) * num + startRotation;
		sprite.color = new Color(1f, 1f, 1f, alpha);
		gameObject.transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);
		gameObject.transform.localScale = Vector3.one * scale;
	}
}
