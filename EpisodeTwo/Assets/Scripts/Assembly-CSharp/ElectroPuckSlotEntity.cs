using FLD;
using UnityEngine;

public class ElectroPuckSlotEntity : Entity
{
	private const float removeEpilon = 1f;

	public int color;

	public int onoffModifier;

	public AudioClip TriggerSound;

	public int trigger;

	private ElectroPuckEntity centerPuck;

	private tk2dBaseSprite _sprite;

	public tk2dBaseSprite Sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dBaseSprite>();
			}
			return _sprite;
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
		color = GetParameterInt("color");
		trigger = GetParameterInt("trigger");
		onoffModifier = 1;
		UpdateGfx();
	}

	private void OnTriggerStay(Collider pCollider)
	{
		if (!pCollider.gameObject.CompareTag("ElectroPuck"))
		{
			return;
		}
		ElectroPuckEntity component = pCollider.gameObject.GetComponent<ElectroPuckEntity>();
		int num = component.color;
		if (num == color)
		{
			centerPuck = component;
		}
		if (!(centerPuck == null))
		{
			Vector3 vector = base.transform.position - centerPuck.transform.position;
			float magnitude = vector.magnitude;
			centerPuck.GetComponent<Rigidbody>().MovePosition(centerPuck.transform.position + vector * Mathf.Min(magnitude, 0.04f));
			if (magnitude < 1f)
			{
				World.TriggerFlag(trigger);
				Object.Destroy(centerPuck.gameObject);
				onoffModifier = 0;
				centerPuck = null;
				UpdateGfx();
				ObjectPool.instance.GetObjectForType<Shockwave>().Play(string.Empty, base.transform.position, 0.6f, 12);
				Audio.Play("TriggerSound", TriggerSound);
			}
		}
	}

	private void UpdateGfx()
	{
		Sprite.spriteId = Sprite.GetSpriteIdByName("electropuck_slot/" + (color * 2 + onoffModifier));
	}
}
