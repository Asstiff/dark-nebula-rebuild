using UnityEngine;

public class LightEntity : Entity
{
	private tk2dSprite _sprite;

	public tk2dSprite Sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dSprite>();
			}
			return _sprite;
		}
		set
		{
			_sprite = value;
		}
	}

	public float Scale
	{
		set
		{
			base.transform.localScale = new Vector3(value, value, 1f);
		}
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		InitParameters();
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		InitParameters();
	}

	protected void InitParameters()
	{
		Scale = GetParameterFloat("scale");
	}

	public void SetVisible(bool hasLightEntity)
	{
		if (Sprite != null && Sprite.GetComponent<Renderer>() != null)
		{
			Sprite.GetComponent<Renderer>().enabled = hasLightEntity;
		}
	}
}
