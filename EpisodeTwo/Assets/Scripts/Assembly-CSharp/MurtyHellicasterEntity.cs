using System;
using FLD;
using UnityEngine;

public class MurtyHellicasterEntity : Enemy
{
	private const float maxRotationAdjustment = (float)System.Math.PI / 50f;

	public float armFactor;

	public float bladeFactor;

	public float spinVelocityFactor;

	public MurtyArmAttachment rightArmAttachment;

	public MurtyArm rightArm;

	public MurtyArmAttachment leftArmAttachment;

	public MurtyArm leftArm;

	public AudioClip SfxWarning;

	public AudioClip SfxActivating;

	public AudioClip SfxArmOut;

	public AudioClip SfxSwing1;

	public AudioClip SfxWindup1;

	public AudioClip SfxWindup2;

	public AudioClip SfxEnd;

	public AudioClip SfxEntry;

	public AudioClip SfxBladeFoldOutSound;

	public float spinVelocity;

	public float eyeFactor;

	public float speed;

	public float targetSpeed;

	public bool reversedSpin;

	public bool wantsToGoFrenzy;

	public bool showWarning;

	public bool collisionGivesDamage;

	protected bool lightsOn;

	public tk2dSprite body;

	public tk2dSprite eye;

	public tk2dSprite spikesAroundBody;

	public tk2dSprite warningLights;

	public GameObject ring;

	public float eyeIntensity;

	protected bool rotateBodySpikes;

	protected float spikesAroundBodyScale;

	protected float warningLightsTimer;

	protected float warningLightsTimeUntilToggle;

	protected bool warningLightsOn;

	protected bool renderDeadLook;

	protected int swingCycle;

	protected int prevSwingCycle;

	private Vector3 direction;

	private float rotationInRad;

	private bool clickSound;

	protected override void Start()
	{
		base.Start();
		base.Health = (maxHealth = 15);
		enableWaypoints = true;
		targetSpeed = 0.8f;
		stunTime = 0.6f;
		base.state = new StateMachine(new MurtyStandby(this));
		color = 1;
		warningLightsTimeUntilToggle = 0.5f;
		removeFromWorldOnDeath = false;
		SetCollisionGivesDamage(false);
	}

	public override void Update()
	{
		base.Update();
		UpdateNormalMovement();
		UpdateWarningLights();
		float num = (float)System.Math.PI * 3f;
		float num2 = spinVelocityFactor * ((!reversedSpin) ? 1f : (-1f)) * num;
		float num3 = num2 - spinVelocity;
		spinVelocity += num3 * ((spinVelocityFactor != 0f) ? 0.3f : 3f) * Time.deltaTime;
		if (!(ring == null))
		{
			ring.transform.RotateAround(Vector3.down, 0f - spinVelocity * Time.deltaTime);
			leftArm.HandDistanceFactor = armFactor;
			leftArm.Hand.BladeFactor = bladeFactor;
			leftArm.Hand.ReversedDirection = true;
			rightArm.HandDistanceFactor = armFactor;
			rightArm.Hand.BladeFactor = bladeFactor;
			rightArm.Hand.ReversedDirection = true;
			if ((float)base.Health == 0f)
			{
				eyeFactor = 0f;
			}
			eyeIntensity += (eyeFactor - eyeIntensity) * Time.deltaTime;
			spikesAroundBodyScale += (bladeFactor - spikesAroundBodyScale) * 2f * Time.deltaTime;
		}
	}

	public void UpdateWarningLights()
	{
		if (!showWarning)
		{
			warningLightsTimer = 0f;
			warningLightsOn = false;
			return;
		}
		warningLightsTimer += Time.deltaTime;
		if (!(warningLightsTimer < warningLightsTimeUntilToggle))
		{
			warningLightsOn = !warningLightsOn;
			warningLightsTimer = 0f;
			if (warningLightsOn)
			{
				Audio.Play("warning", SfxWarning, transform);
			}
		}
	}

	protected override void OnHealthChanged()
	{
		base.OnHealthChanged();
		if (!renderDeadLook)
		{
			int num = base.Health / 10;
			body.spriteId = body.GetSpriteIdByName("murty_" + (3 - num));
			if (base.Health == 0)
			{
				tk2dSprite component = ring.GetComponent<tk2dSprite>();
				component.spriteId = component.GetSpriteIdByName("murty_ring1");
			}
		}
	}

	public void LateUpdate()
	{
		if (!renderDeadLook)
		{
			warningLights.GetComponent<Renderer>().enabled = warningLightsOn;
			float num = spikesAroundBodyScale * 0.2f + 0.8f;
			spikesAroundBody.transform.localScale = new Vector3(num, 1f, num);
			eye.color = new Color(1f, 1f, 1f, eyeIntensity);
		}
	}

	public void UpdateNormalMovement()
	{
		if (!(World.vessel == null))
		{
			float a = FLD.Math.AngleFromPositions(World.vessel.transform.position, base.transform.position);
			float pAngle = BattlePuck.SmallestDifference(a, rotationInRad);
			RotationAngle rotationAngle = RotationAngleMin(pAngle, (float)System.Math.PI / 50f);
			rotationInRad -= rotationAngle.Radians * Time.deltaTime * 30f;
			direction = FLD.Math.AngleToVector3Z(rotationInRad);
			float num = targetSpeed * 2f;
			if (isStunned)
			{
				num = 0f;
			}
			else
			{
				speed += (num - speed) * Time.deltaTime * 5f;
				base.GetComponent<Rigidbody>().MovePosition(base.transform.position + direction * speed * Time.deltaTime * 30f);
			}
			base.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
			UpdateSwingingSound();
		}
	}

	private RotationAngle RotationAngleMin(float pAngle, float maxAngle)
	{
		if (Mathf.Abs(pAngle) > maxAngle)
		{
			return new RotationAngle(maxAngle * ((!(pAngle < 0f)) ? 1f : (-1f)));
		}
		return new RotationAngle(pAngle);
	}

	private void UpdateSwingingSound()
	{
		AudioClip audioClip = null;
		float num = Mathf.Abs(spinVelocity / 8f);
		audioClip = ((!((double)num < 0.05)) ? SfxSwing1 : null);
		float num2 = Mathf.Abs(base.transform.localRotation.eulerAngles.z);
		swingCycle = (int)(num2 / 180f);
		if (swingCycle != prevSwingCycle && audioClip != null)
		{
			Audio.Play("swing", audioClip, transform);
			Debug.Log("Play swing " + audioClip);
		}
		prevSwingCycle = swingCycle;
	}

	private new void OnCollisionEnter(Collision collision)
	{
		if (collisionGivesDamage && collision.gameObject.CompareTag("Player"))
		{
			World.vessel.ReceiveDamage(1);
		}
	}

	protected override void OnDamage(RotationAngle angle)
	{
		base.OnDamage(angle);
	}

	protected override void OnDie(RotationAngle r)
	{
		base.OnDie(r);
		SetCollisionGivesDamage(false);
	}

	public void SetCollisionGivesDamage(bool on)
	{
		leftArm.SetIsDead(!on);
		leftArm.SetIsDead(!on);
		collisionGivesDamage = on;
	}

	public void SpawnRandomExplosion()
	{
		Vector2 vector = default(Vector2);
		vector.x = UnityEngine.Random.value * 128f;
		vector.y = UnityEngine.Random.value * 20f;
		Vector2 vector2 = RotateVector2(vector, new RotationAngle(rotationInRad));
		Vector3 position = base.transform.position + new Vector3(vector2.x, 0f, vector2.y);
		ObjectPool.instance.GetObjectForType<ExplosionEntity>().Explode(position, 1f);
		SpawnDebrisParticles(new RotationAngle());
	}

	public void PrepareDeadLook()
	{
		ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.5f, 60).Inverse();
	}

	private Vector2 RotateVector2(Vector2 vector, RotationAngle rotation)
	{
		float radians = rotation.Radians;
		Vector2 result = default(Vector2);
		result.x = vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians);
		result.y = vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians);
		return result;
	}

	public void SwitchToDeadLook()
	{
		for (int i = 0; i < 4; i++)
		{
			SpawnDebrisParticle(1f);
		}
		for (int j = 0; j < 6; j++)
		{
			SpawnDebrisParticle(4f);
		}
		ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.5f, 60);
		World.TriggerFlag(102);
		World.TriggerFlag(101);
		UnityEngine.Object.Destroy(ring);
		ObjectPool.instance.GetObjectForType<ExplosionEntity>().Explode(base.transform.position, 2f);
		body.spriteId = body.GetSpriteIdByName("murty_broken");
		base.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		warningLights.GetComponent<Renderer>().enabled = false;
		renderDeadLook = true;
	}

	private void PlayClickSound()
	{
		AudioClip audioClip = null;
		Audio.Play(pClip: (!clickSound) ? SfxWindup2 : SfxWindup1, pID: "clickSound" + UnityEngine.Random.value);
		clickSound = !clickSound;
		Invoke("PlayClickSound", 0.33f);
	}
}
