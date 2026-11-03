using UnityEngine;

public class Waypoint : Entity
{
	private tk2dBaseSprite sprite;

	protected override void Awake()
	{
		base.Awake();
		sprite = GetComponent<tk2dBaseSprite>();
	}

	public void SetAlpha(float alpha)
	{
		sprite.color = new Color(1f, 1f, 1f, alpha);
	}
}
