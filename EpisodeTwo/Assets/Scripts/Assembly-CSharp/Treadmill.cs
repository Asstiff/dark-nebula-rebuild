using System;
using FLD;
using UnityEngine;

public class Treadmill : Entity
{
	public float rotation;

	public tk2dAnimatedSprite originalSprite;

	private tk2dAnimatedSprite[] sprites;

	private float speed;

	private Vector2 pushVector;

	private bool isVesselOutside = true;

	private void Start()
	{
		speed = GetParameterFloat("speed");
		sprites = GetComponentsInChildren<tk2dAnimatedSprite>();
		pushVector = FLD.Math.AngleToVector(rotation);
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		tk2dAnimatedSprite[] componentsInChildren = GetComponentsInChildren<tk2dAnimatedSprite>();
		tk2dAnimatedSprite[] array = componentsInChildren;
		foreach (tk2dAnimatedSprite tk2dAnimatedSprite2 in array)
		{
			if (tk2dAnimatedSprite2 != originalSprite)
			{
				UnityEngine.Object.DestroyImmediate(tk2dAnimatedSprite2.gameObject);
			}
		}
		(base.GetComponent<Collider>() as BoxCollider).size = new Vector3(64 * GetParameterInt("columns"), 64 * GetParameterInt("rows"), 256f);
		(base.GetComponent<Collider>() as BoxCollider).center = new Vector3(32 * (GetParameterInt("columns") - 1), -32 * (GetParameterInt("rows") - 1), 0f);
		originalSprite.gameObject.transform.rotation = Quaternion.identity;
		rotation = Turret.directionAngle[GetParameterInt("direction")] + (float)System.Math.PI;
		for (int j = 0; j < GetParameterInt("rows"); j++)
		{
			for (int k = 0; k < GetParameterInt("columns"); k++)
			{
				GameObject gameObject;
				if (j == 0 && k == 0)
				{
					gameObject = originalSprite.gameObject;
				}
				else
				{
					gameObject = UnityEngine.Object.Instantiate(originalSprite.gameObject) as GameObject;
					gameObject.transform.parent = base.transform;
					gameObject.name = "clone " + k + "x" + j;
				}
				gameObject.transform.position = base.transform.position + new Vector3((float)k * 64f, 0f, (float)(-j) * 64f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.Rotate(new Vector3(0f, 0f, rotation * 57.29578f));
			}
		}
	}

	public override void InitParametersInRuntime()
	{
		InitParametersInEditor();
	}

	private void Update()
	{
		int spriteId = (int)(Time.time * 22f * speed * 2.5f * World.Difficulty) % 64;
		tk2dAnimatedSprite[] array = sprites;
		foreach (tk2dAnimatedSprite tk2dAnimatedSprite2 in array)
		{
			tk2dAnimatedSprite2.spriteId = spriteId;
		}
	}

	private void FixedUpdate()
	{
		if (!isVesselOutside)
		{
			(World.vessel as StandardVessel).ActivateTreadmill(PushVector());
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			isVesselOutside = false;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.CompareTag("Player"))
		{
			isVesselOutside = true;
		}
	}

	private Vector2 PushVector()
	{
		return pushVector * (speed * World.Difficulty);
	}
}
