using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class Cthulhu : Entity
{
	private enum CthulhuState
	{
		CTHULHU_SLEEPING = 0,
		CTHULHU_INTRODUCTION = 1,
		CTHULHU_SWAYING = 2,
		CTHULHU_MEGAEXPLOSION = 3
	}

	private float currFrame;

	private float currAttackFrame;

	private float _eyeAlpha;

	private float armsAngle;

	public tk2dSprite Head;

	public tk2dSprite LeftEye;

	public tk2dSprite RightEye;

	public CthulhuArm arm1;

	public CthulhuArm arm2;

	public CthulhuArm arm3;

	public AudioClip spottedSound;

	public AudioClip explodeSound;

	public AudioClip dieSound;

	public AudioClip attack1;

	public AudioClip attack2;

	private FlagCondition activateEvent;

	private CthulhuState _state;

	private Dictionary<int, bool> stateTimeFlag = new Dictionary<int, bool>();

	private int repeatingActionValue;

	private int repeatingActionValueCounter = 280;

	public float eyeAlpha
	{
		get
		{
			return _eyeAlpha;
		}
		set
		{
			_eyeAlpha = value;
			if (LeftEye != null)
			{
				LeftEye.color = new Color(1f, 1f, 1f, _eyeAlpha);
			}
			if (RightEye != null)
			{
				RightEye.color = new Color(1f, 1f, 1f, _eyeAlpha);
			}
		}
	}

	private CthulhuState state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			stateTimeFlag.Clear();
		}
	}

	private bool TimeForAction(int StateFrame)
	{
		if (currFrame < (float)StateFrame)
		{
			return false;
		}
		if (stateTimeFlag.ContainsKey(StateFrame))
		{
			return false;
		}
		stateTimeFlag.Add(StateFrame, true);
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		arm2 = (UnityEngine.Object.Instantiate(arm1.gameObject) as GameObject).GetComponent<CthulhuArm>();
		arm2.transform.parent = base.transform;
		arm2.transform.localPosition = Vector3.zero;
		arm2.transform.localRotation = arm1.transform.localRotation;
		arm3 = (UnityEngine.Object.Instantiate(arm1.gameObject) as GameObject).GetComponent<CthulhuArm>();
		arm3.transform.parent = base.transform;
		arm3.transform.localPosition = Vector3.zero;
		arm3.transform.localRotation = arm1.transform.localRotation;
		arm1.Setup(this, -120f, -45f);
		arm2.Setup(this, 120f, 45f);
		arm3.Setup(this, 0f, 0f);
		base.keepalive = true;
		activateEvent = new FlagCondition(111, 1);
		activateEvent.Callback += OnActivateEvent;
		currAttackFrame = 279f;
	}

	private void OnStart()
	{
		eyeAlpha = 0f;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (activateEvent != null)
		{
			activateEvent.Callback -= OnActivateEvent;
			activateEvent.Dispose();
			activateEvent = null;
		}
	}

	private void UpdateState()
	{
		switch (state)
		{
		case CthulhuState.CTHULHU_INTRODUCTION:
			currFrame += Time.deltaTime * 30f;
			if (TimeForAction(30))
			{
				arm1.TransitTo(CthulhuArm.CthulhuArmState.CTHULHUARM_INTRODUCTION);
				arm2.TransitTo(CthulhuArm.CthulhuArmState.CTHULHUARM_INTRODUCTION);
				arm3.TransitTo(CthulhuArm.CthulhuArmState.CTHULHUARM_INTRODUCTION);
			}
			if (TimeForAction(90))
			{
				state = CthulhuState.CTHULHU_SWAYING;
				arm1.TransitTo(CthulhuArm.CthulhuArmState.CTHULHUARM_SWAYING);
				arm2.TransitTo(CthulhuArm.CthulhuArmState.CTHULHUARM_SWAYING);
				arm3.TransitTo(CthulhuArm.CthulhuArmState.CTHULHUARM_SWAYING);
			}
			eyeAlpha += 0.01f;
			if (eyeAlpha > 1f)
			{
				eyeAlpha = 1f;
			}
			break;
		case CthulhuState.CTHULHU_SWAYING:
			if (DeadArms() == 3)
			{
				state = CthulhuState.CTHULHU_MEGAEXPLOSION;
				currFrame = 0f;
			}
			break;
		case CthulhuState.CTHULHU_MEGAEXPLOSION:
			if (TimeForAction(0))
			{
				SpawnExplosion(base.transform.position);
				SpawnDebrisParticles(base.transform.position);
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(5))
			{
				SpawnExplosion(base.transform.position + p2d(80, 80));
			}
			if (TimeForAction(10))
			{
				SpawnExplosion(base.transform.position + p2d(-80, 80));
			}
			if (TimeForAction(15))
			{
				SpawnExplosion(base.transform.position + p2d(0, -80));
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(20))
			{
				SpawnExplosion(base.transform.position + p2d(0, 0));
			}
			if (TimeForAction(25))
			{
				SpawnExplosion(base.transform.position + p2d(-80, -80));
				SpawnDebrisParticles(base.transform.position);
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(3))
			{
				SpawnExplosion(base.transform.position + p2d(40, 40));
				SpawnMegaShockwaveInverted(base.transform.position + p2d(18, 0), true);
			}
			if (TimeForAction(12))
			{
				SpawnExplosion(base.transform.position + p2d(-40, 40));
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(17))
			{
				SpawnExplosion(base.transform.position + p2d(0, -40));
			}
			if (TimeForAction(23))
			{
				SpawnExplosion(base.transform.position + p2d(0, 0));
			}
			if (TimeForAction(27))
			{
				SpawnExplosion(base.transform.position + p2d(-40, -40));
			}
			if (TimeForAction(30))
			{
				SpawnExplosion(base.transform.position);
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(35))
			{
				SpawnExplosion(base.transform.position + p2d(80, 80));
			}
			if (TimeForAction(40))
			{
				SpawnExplosion(base.transform.position + p2d(-80, 80));
				SpawnDebrisParticles(base.transform.position);
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(45))
			{
				SpawnExplosion(base.transform.position + p2d(0, -80));
			}
			if (TimeForAction(50))
			{
				SpawnExplosion(base.transform.position + p2d(0, 0));
			}
			if (TimeForAction(55))
			{
				SpawnExplosion(base.transform.position + p2d(-80, -80));
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(33))
			{
				SpawnExplosion(base.transform.position + p2d(40, 40));
			}
			if (TimeForAction(43))
			{
				SpawnExplosion(base.transform.position + p2d(-40, 40));
				SpawnDebrisParticles(base.transform.position);
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(47))
			{
				SpawnExplosion(base.transform.position + p2d(0, -40));
				SpawnMegaShockwaveInverted(base.transform.position + p2d(18, 0), false);
			}
			if (TimeForAction(53))
			{
				SpawnExplosion(base.transform.position + p2d(0, 0));
				World.TriggerFlag(100);
				Audio.Play("explodeSound " + UnityEngine.Random.value, explodeSound, base.transform.position);
			}
			if (TimeForAction(57))
			{
				SpawnExplosion(base.transform.position + p2d(-40, -40));
			}
			if (TimeForAction(60))
			{
				SpawnMegaExplosion(base.transform.position + p2d(18, 0));
				World.TriggerFlag(101);
			}
			if (TimeForAction(62))
			{
				eyeAlpha = 0f;
				Head.spriteId = Head.GetSpriteIdByName("cthulhu-head/1");
				Audio.Play("dieSound " + UnityEngine.Random.value, dieSound, base.transform.position);
				World.TriggerFlag(99);
			}
			currFrame += Time.deltaTime * 30f;
			break;
		}
	}

	private Vector3 p2d(int x, int y)
	{
		return new Vector3(x, 0f, y);
	}

	public void SpawnMegaShockwaveInverted(Vector3 p, bool inverse)
	{
		Shockwave shockwave = ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.5f, 60);
		if (inverse)
		{
			shockwave.Inverse();
		}
	}

	public void SpawnExplosion(Vector3 spawnPosition)
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", spawnPosition);
	}

	public void SpawnMegaExplosion(Vector3 spawnPosition)
	{
		SpawnMegaDebrisParticles(spawnPosition);
		ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", spawnPosition, 3f);
	}

	public void SpawnDebrisParticles(Vector3 spawnPosition)
	{
		for (int i = 0; i < 10; i++)
		{
			SpawnDebrisParticleAndSize(spawnPosition, 1f);
		}
	}

	public void SpawnMegaDebrisParticles(Vector3 spawnPosition)
	{
		for (int i = 0; i < 30; i++)
		{
			SpawnDebrisParticleAndSize(spawnPosition, 2f);
		}
	}

	public Vector3 Point2dRandomOrientation()
	{
		return randomRotation2d().ToVector3();
	}

	public RotationAngle randomRotation2d()
	{
		return new RotationAngle(UnityEngine.Random.Range(0f, (float)System.Math.PI * 2f));
	}

	public void SpawnDebrisParticleAndSize(Vector3 spawnPosition, float size)
	{
		RotationAngle rotationAngle = new RotationAngle(UnityEngine.Random.value * (float)System.Math.PI * 2f);
		spawnPosition += rotationAngle.ToVector3() * (UnityEngine.Random.value * 10f + 20f);
		float scale = size + UnityEngine.Random.value * 0.5f;
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		if (objectForType == null)
		{
			Debug.LogError("Could not create <Particle> from objectpool.");
		}
		else
		{
			objectForType.Play("_debris" + UnityEngine.Random.Range(1, 4), spawnPosition, scale, UnityEngine.Random.value * 200f + 100f, true, rotationAngle);
		}
	}

	private bool TimeForRepeatingAction(int StateFrame)
	{
		if (currAttackFrame < (float)StateFrame)
		{
			return false;
		}
		if (repeatingActionValue == StateFrame)
		{
			return false;
		}
		repeatingActionValue = StateFrame;
		return true;
	}

	private void Update()
	{
		UpdateState();
		if (arm1 != null)
		{
			arm1.UpdateArm(armsAngle);
		}
		if (arm2 != null)
		{
			arm2.UpdateArm(armsAngle);
		}
		if (arm3 != null)
		{
			arm3.UpdateArm(armsAngle);
		}
		if (state != CthulhuState.CTHULHU_SWAYING)
		{
			return;
		}
		armsAngle += 0.4f * (1f + (float)DeadArms()) * ((!UserSettings.I.expert) ? 1f : 1.2f) * Time.deltaTime * 30f;
		currAttackFrame += Time.deltaTime * 30f;
		if (TimeForRepeatingAction(repeatingActionValueCounter))
		{
			repeatingActionValueCounter += 280;
			if ((double)UnityEngine.Random.value > 0.5)
			{
				Audio.Play("attack1" + UnityEngine.Random.value, attack1);
			}
			else
			{
				Audio.Play("attack2" + UnityEngine.Random.value, attack2);
			}
		}
	}

	private int DeadArms()
	{
		int num = 0;
		if (arm1.IsArmDead())
		{
			num++;
		}
		if (arm2 != null && arm2.IsArmDead())
		{
			num++;
		}
		if (arm3 != null && arm3.IsArmDead())
		{
			num++;
		}
		return num;
	}

	private void OnActivateEvent()
	{
		state = CthulhuState.CTHULHU_INTRODUCTION;
		Audio.Play("spottedSound", spottedSound, base.transform.position);
	}

	private void OnCollisionEnter(Collision e)
	{
		if (e.gameObject.CompareTag("Player"))
		{
			World.vessel.ReceiveDamage(1);
		}
	}
}
