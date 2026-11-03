using UnityEngine;

public class TheMerdinatorEntity : AnimatedEntity
{
	public tk2dSprite Base;

	public tk2dSprite AnimatedGfx;

	private float rotationSpeed;

	protected override tk2dSprite Sprite
	{
		get
		{
			return AnimatedGfx;
		}
	}

	private void Start()
	{
		rotationSpeed = 1.8849558f * ((GetParameterInt("type") != 0) ? 3f : 1f);
	}

	public override void Update()
	{
		base.Update();
		AnimatedGfx.transform.RotateAround(Vector3.up, rotationSpeed * Time.deltaTime);
	}

	public override void InitParametersInEditor()
	{
		SetParameter("width", "60");
		SetParameter("height", "60");
		SetParameter("givesDamage", "1");
		shapeType = typeof(CapsuleCollider);
		base.InitParametersInEditor();
		if (GetParameterInt("type") == 0)
		{
			Base.spriteId = Base.GetSpriteIdByName("themerdinator/1");
			AnimatedGfx.spriteId = AnimatedGfx.GetSpriteIdByName("themerdinator/0");
		}
		else
		{
			Base.spriteId = Base.GetSpriteIdByName("themerdinator_desert/1");
			AnimatedGfx.spriteId = AnimatedGfx.GetSpriteIdByName("themerdinator_desert/0");
		}
	}

	public override void InitParametersInRuntime()
	{
		InitParametersInEditor();
	}
}
