using System;
using UnityEngine;

public class DN2WindmillEntity : Windmill
{
	private float rotationoffset;

	private float rotation;

	private float rotationSpeedScale;

	private float rotationspeed;

	private float swayingMin;

	private float swayingMax;

	private float quadraticSpeed;

	private float constantSpeed;

	private float linearSpeed;

	private float dir = 1f;

	public override void Start()
	{
		base.Start();
		quadraticSpeed = GetParameterFloat("quadraticSpeed");
		constantSpeed = GetParameterFloat("constantSpeed");
		swayingMax = GetParameterFloat("swayingMax") * ((float)Math.PI / 180f);
		swayingMin = GetParameterFloat("swayingMin") * ((float)Math.PI / 180f);
		linearSpeed = GetParameterFloat("linearSpeed");
		rotationspeed = GetParameterFloat("rotationspeed");
		rotationoffset = swayingMin + GetParameterFloat("rotationoffset") * ((float)Math.PI / 180f);
		rotation = rotationoffset;
		rotationSpeedScale = 1f;
		base.keepalive = true;
		freezable = false;
		dir = 1f;
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
		if (GetParameterInt("texture") == 1)
		{
			tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
			foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
			{
				tk2dSpriteDefinition currentSpriteDef = tk2dSprite2.GetCurrentSpriteDef();
				tk2dSprite2.spriteId = tk2dSprite2.GetSpriteIdByName(currentSpriteDef.name.Replace("windmill", "windmill_shadow"));
			}
		}
	}

	public override void Update()
	{
		if (!IsPooled && World.IsRunning)
		{
			float f = ((rotation - swayingMin) / (swayingMax - swayingMin) - 0.5f) * 2f;
			float num = 1f - Mathf.Abs(f);
			if (num < 0f)
			{
				num = 0f;
			}
			rotationspeed = (quadraticSpeed * num * num + linearSpeed * num + constantSpeed) * rotationSpeedScale;
			rotation += 0.06f * rotationspeed * dir * Time.deltaTime * 30f;
			if (rotation <= swayingMin)
			{
				dir = 1f;
			}
			else if (rotation >= swayingMax)
			{
				dir = -1f;
			}
			rotationSpeedScale = World.Difficulty;
			base.armsTransform.localRotation = Quaternion.AngleAxis(rotation * 57.29578f, Vector3.forward);
		}
	}

	protected override void SetDifficultyTo(float delta)
	{
		rotationSpeedScale = delta;
	}
}
