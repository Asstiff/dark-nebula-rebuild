using System;
using UnityEngine;

public class ShooterBossPart : MonoBehaviour
{
	private bool scaletoZero;

	private bool fadeToZero;

	private tk2dSprite _sprite;

	private string spriteName;

	public tk2dSprite sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dSprite>();
				if (_sprite != null)
				{
					spriteName = _sprite.GetCurrentSpriteDef().name;
					if (spriteName.IndexOf("/") != -1)
					{
						spriteName = spriteName.Substring(0, spriteName.IndexOf("/"));
					}
				}
			}
			return _sprite;
		}
	}

	public float LocalPositionX
	{
		get
		{
			return base.transform.localPosition.x;
		}
		set
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = value;
			base.transform.localPosition = localPosition;
		}
	}

	public Vector2 LocalPosition
	{
		get
		{
			return new Vector2(base.transform.localPosition.x, base.transform.localPosition.y);
		}
		set
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = value.x;
			localPosition.x = value.y;
			base.transform.localPosition = localPosition;
		}
	}

	public float ParentRotationAngle
	{
		get
		{
			return base.transform.localRotation.eulerAngles.z % 360f * ((float)Math.PI / 180f);
		}
		set
		{
			Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
			base.transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, value * 57.29578f);
		}
	}

	public float alpha
	{
		get
		{
			return sprite.color.a;
		}
		set
		{
			sprite.color = new Color(1f, 1f, 1f, value);
		}
	}

	public float scale
	{
		get
		{
			return base.transform.localScale.x;
		}
		set
		{
			base.transform.localScale = Vector3.one * value;
		}
	}

	public void SetFrame(int frame)
	{
		sprite.spriteId = sprite.GetSpriteIdByName(spriteName + "/" + frame);
	}

	public virtual void Update()
	{
		if (scaletoZero)
		{
			scale -= 0.01f;
			if (scale < 0f)
			{
				scale = 0f;
			}
		}
		if (fadeToZero)
		{
			alpha -= 0.005f;
			if (alpha < 0f)
			{
				alpha = 0f;
			}
		}
	}

	public void ScaleToZero()
	{
		scaletoZero = true;
	}

	public void FadeToZero()
	{
		fadeToZero = true;
	}

	public virtual void UpdateState(ShooterBoss.State state)
	{
	}
}
