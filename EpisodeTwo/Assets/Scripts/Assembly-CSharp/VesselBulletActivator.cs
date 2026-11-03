using UnityEngine;

public class VesselBulletActivator : Entity
{
	public tk2dSprite Gfx;

	private int colorValue;

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		colorValue = Constants.GetColorInt(GetParameterString("color"));
		Gfx.spriteId = Gfx.GetSpriteIdByName("entity_vessel_bullet_activator/" + colorValue);
		if (colorValue == 0)
		{
			(base.GetComponent<Collider>() as BoxCollider).size = new Vector3(116f, 37f, 256f);
		}
	}

	public override void InitParametersInRuntime()
	{
		InitParametersInEditor();
	}

	private void Start()
	{
		colorValue = Constants.GetColorInt(GetParameterString("color"));
	}

	private void OnTriggerEnter(Collider pCollider)
	{
		if (pCollider.CompareTag("Player"))
		{
			World.vessel.TriggerVesselBullet(colorValue, GetParameterInt("hasLightEntity") != 0);
		}
	}
}
