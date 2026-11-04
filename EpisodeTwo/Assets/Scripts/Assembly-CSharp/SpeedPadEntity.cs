using FLD;
using UnityEngine;

public class SpeedPadEntity : Entity
{
	private bool isActivated;

	public AudioClip activationSound;

	public tk2dAnimatedSprite Sprite;

	private float rotation;

	private float speed;

	private bool activatePlayerMovement;

	private bool deactivatePlayerMovement;

	private void Start()
	{
		speed = GetParameterFloat("speed");
		rotation = 0f - Turret.directionAngle[GetParameterInt("direction")];
		Sprite.transform.localEulerAngles = new Vector3(0f, 0f, rotation * 57.29578f);
		ResumeNormalAnimation();
		activatePlayerMovement = GetParameterBool("activatePlayerMovement");
		deactivatePlayerMovement = GetParameterBool("deactivatePlayerMovement");
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		rotation = 0f - Turret.directionAngle[GetParameterInt("direction")];
		Sprite.transform.localEulerAngles = new Vector3(0f, 0f, rotation * 57.29578f);
	}

	public override void InitParametersInRuntime()
	{
		InitParametersInEditor();
	}

	private void SpawnShockwave()
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play(string.Empty, base.transform.position, 0.4f);
		ObjectPool.instance.GetObjectForType<Shockwave>().Play(string.Empty, base.transform.position);
	}

	private void OnTriggerStay(Collider collider)
	{
		if (isActivated || !collider.CompareTag("Player"))
		{
			return;
		}
		StandardVessel standardVessel = World.vessel as StandardVessel;
		if (GetParameterBool("triggerInCenter"))
		{
			float sqrMagnitude = (standardVessel.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude > 400f)
			{
				return;
			}
		}
		SpawnShockwave();
		if (activatePlayerMovement)
		{
			standardVessel.SetCanMove(true);
		}
		else if (deactivatePlayerMovement)
		{
			standardVessel.SetCanMove(false);
		}
		Vector2 vector = Math.AngleToVector(rotation);
		vector = vector * speed * 1.5f;
		standardVessel.ResetForcedVelocity();
		standardVessel.SetForcedVelocity(vector, true);
		standardVessel.ResetVelocity();
		Audio.Play("speedpad", activationSound, transform);
		Sprite.Play("SpeedPadActive");
		isActivated = true;
		Invoke("ResumeNormalAnimation", 0.5f);
	}

	private void ResumeNormalAnimation()
	{
		Sprite.Play("SpeedPad");
		isActivated = false;
	}

	private void OnDisable()
	{
		Sprite.enabled = false;
	}

	private void OnEnable()
	{
		Sprite.enabled = true;
	}
}
