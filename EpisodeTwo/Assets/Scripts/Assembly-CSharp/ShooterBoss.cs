using System.Collections;
using FLD;
using UnityEngine;

public class ShooterBoss : Enemy
{
	public enum State
	{
		INITIAL_STATE = 0,
		TRANSIT_TO_TURRET_STATE = 1,
		TURRETS_STATE = 2,
		MOVE_LOWER_CLAMPS_STATE = 3,
		MOVE_UPPER_CLAMPS_STATE = 4,
		MOVE_UPPER_BRAIN_STATE = 5,
		ACTIVATE_LASERS = 6,
		MOVE_LOWER_BRAIN_STATE = 7,
		SPIN_LASERS = 8,
		FINAL_FIGHT = 9,
		MEGA_EXPLOSION = 10
	}

	private static ShooterBoss _instance;

	public static float TurretsRotationSpeed;

	public ShooterBossPart UpperLeftBrain;

	public ShooterBossPart UpperRightBrain;

	public ShooterBossPart LowerLeftBrain;

	public ShooterBossPart LowerRightBrain;

	public ShooterBossPart UpperLeftClamp;

	public ShooterBossPart UpperRightClamp;

	public ShooterBossPart LowerLeftClamp;

	public ShooterBossPart LowerRightClamp;

	public ShooterBossPart BrainGlow;

	public ShooterBossPart UpperArmor;

	public ShooterBossPart UpperTwirl;

	public ShooterBossPart Eye;

	public ShooterBossRingAndLasers Ring;

	public EndMorph EndMorphPrefab;

	public MagnetVesselSlotEntity MagnetPrefab;

	public AudioClip EnterScreenSfx;

	public AudioClip TowersOutSfx;

	public AudioClip OpenupSfx;

	public AudioClip OpenupStopSfx;

	public AudioClip OpenupLaserSfx;

	public AudioClip ExplodeSfx;

	public ShooterBossBars Bars;

	private bool stopCurrFrame;

	private bool deScaleTwirls;

	private float eyeFrame;

	private float twirlAngle;

	private float twirlAlpha = 1f;

	private float twirlScale = 1f;

	private float eyeRotation;

	private float currFrame;

	private Vector3[] explosionOrigins;

	private Vector2[] megaExplosionRandomSpeeds;

	private float[] megaExplosionRandomRotationSpeeds;

	private State _state;

	private Hashtable activatedFrames = new Hashtable();

	private StarfighterVesselEntity _ve;

	public static ShooterBoss Instance
	{
		get
		{
			return _instance;
		}
	}

	public new State state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
		}
	}

	private StarfighterVesselEntity ve
	{
		get
		{
			if (_ve == null)
			{
				_ve = World.vessel as StarfighterVesselEntity;
			}
			return _ve;
		}
	}

	public EndMorph CreateEndMorph(Vector3 bossPos)
	{
		EndMorph endMorph = Object.Instantiate(EndMorphPrefab) as EndMorph;
		endMorph.transform.position = bossPos;
		MagnetVesselSlotEntity magnetVesselSlotEntity = Object.Instantiate(MagnetPrefab) as MagnetVesselSlotEntity;
		magnetVesselSlotEntity.GetComponent<MagnetVesselSlotEntity>().InitWithWorldEndSequence(bossPos);
		return endMorph;
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		ObjectPool.AddAsset<ShooterEnemyBulletEntity>("Assets/Prefabs/Entities/ShooterEnemyBulletEntity.prefab", 16);
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
	}

	protected override void Start()
	{
		base.Start();
		_instance = this;
		InitExplosionOrigins();
		InitMegaExplosionRandomSpeeds();
		TurretsRotationSpeed = ((!UserSettings.I.expert) ? 0.03f : 0.07f);
	}

	private void InitExplosionOrigins()
	{
		explosionOrigins = new Vector3[10];
		Vector3 position = base.transform.position;
		explosionOrigins[0].x = position.x + 20f;
		explosionOrigins[0].z = position.z + 20f;
		explosionOrigins[1].x = position.x - 20f;
		explosionOrigins[1].z = position.z - 20f;
		explosionOrigins[2].x = position.x + 20f;
		explosionOrigins[2].z = position.z - 20f;
		explosionOrigins[3].x = position.x - 20f;
		explosionOrigins[3].z = position.z + 20f;
		explosionOrigins[4].x = position.x;
		explosionOrigins[4].z = position.z;
		explosionOrigins[5].x = position.x - 40f;
		explosionOrigins[5].z = position.z - 40f;
		explosionOrigins[6].x = position.x + 40f;
		explosionOrigins[6].z = position.z + 40f;
		explosionOrigins[7].x = position.x - 40f;
		explosionOrigins[7].z = position.z + 40f;
		explosionOrigins[8].x = position.x + 40f;
		explosionOrigins[8].z = position.z + 40f;
		explosionOrigins[9].x = position.x;
		explosionOrigins[9].z = position.z;
	}

	public override void Update()
	{
		base.Update();
		twirlAngle += 0.005f * Time.deltaTime * 30f;
		eyeFrame += 1f * Time.deltaTime * 30f;
		if (eyeFrame > 15f)
		{
			eyeFrame = 0f;
		}
		Eye.SetFrame((int)eyeFrame);
		eyeRotation += 0.01f * Time.deltaTime * 30f;
		UpdateState();
		float num = 0.0025f;
		float num2 = 0.1f;
		float num3 = 0.8f;
		if (state == State.MEGA_EXPLOSION)
		{
			LowerLeftClamp.LocalPosition += megaExplosionRandomSpeeds[0] * Time.deltaTime * 30f;
			LowerLeftClamp.ParentRotationAngle -= megaExplosionRandomRotationSpeeds[0] * Time.deltaTime * 30f;
			LowerRightClamp.LocalPosition += megaExplosionRandomSpeeds[1] * Time.deltaTime * 30f;
			LowerRightClamp.ParentRotationAngle -= megaExplosionRandomRotationSpeeds[1] * Time.deltaTime * 30f;
			UpperRightClamp.LocalPosition += megaExplosionRandomSpeeds[2] * Time.deltaTime * 30f;
			UpperRightClamp.ParentRotationAngle -= megaExplosionRandomRotationSpeeds[2] * Time.deltaTime * 30f;
			UpperLeftClamp.LocalPosition += megaExplosionRandomSpeeds[2] * Time.deltaTime * 30f;
			UpperLeftClamp.ParentRotationAngle -= megaExplosionRandomRotationSpeeds[2] * Time.deltaTime * 30f;
		}
		else
		{
			if (state >= State.MOVE_LOWER_CLAMPS_STATE)
			{
				if (LowerRightClamp.ParentRotationAngle < 0.3f)
				{
					LowerRightClamp.ParentRotationAngle += num * Time.deltaTime * 30f;
				}
				if (LowerLeftClamp.ParentRotationAngle > 5.8f || LowerLeftClamp.ParentRotationAngle == 0f)
				{
					LowerLeftClamp.ParentRotationAngle -= num * Time.deltaTime * 30f;
				}
			}
			if (state >= State.MOVE_UPPER_CLAMPS_STATE)
			{
				if (UpperRightClamp.ParentRotationAngle < 0.5f)
				{
					UpperRightClamp.ParentRotationAngle += num * Time.deltaTime * 30f;
				}
				if (UpperLeftClamp.ParentRotationAngle > 5.8f || UpperLeftClamp.ParentRotationAngle == 0f)
				{
					UpperLeftClamp.ParentRotationAngle -= num * Time.deltaTime * 30f;
				}
			}
			if (state >= State.MOVE_UPPER_BRAIN_STATE)
			{
				if (UpperLeftBrain.LocalPositionX > -18f)
				{
					UpperLeftBrain.LocalPositionX -= num2 * Time.deltaTime * 30f;
				}
				if (UpperRightBrain.LocalPositionX < 18f)
				{
					UpperRightBrain.LocalPositionX += num2 * Time.deltaTime * 30f;
				}
			}
			if (state >= State.MOVE_LOWER_BRAIN_STATE)
			{
				if (LowerLeftBrain.LocalPositionX > -15f)
				{
					LowerLeftBrain.LocalPositionX -= num3 * Time.deltaTime * 30f;
				}
				if (LowerRightBrain.LocalPositionX < 15f)
				{
					LowerRightBrain.LocalPositionX += num3 * Time.deltaTime * 30f;
				}
			}
		}
		if (deScaleTwirls)
		{
			twirlScale -= 0.005f * Time.deltaTime * 30f;
			if (twirlScale < 0f)
			{
				twirlScale = 0f;
			}
			UpperTwirl.scale = twirlScale;
			twirlAlpha -= 0.01f * Time.deltaTime * 30f;
			if (twirlAlpha < 0f)
			{
				twirlAlpha = 0f;
			}
		}
		UpperTwirl.ParentRotationAngle = twirlAngle;
		UpperTwirl.scale = 4.5f * twirlScale;
		UpperTwirl.alpha = 0.2f * twirlAlpha;
	}

	private void InitMegaExplosionRandomSpeeds()
	{
		megaExplosionRandomSpeeds = new Vector2[4];
		megaExplosionRandomRotationSpeeds = new float[4];
		for (int i = 0; i < 4; i++)
		{
			megaExplosionRandomSpeeds[i] = new Vector2(0.08f * Random.Range(-1f, 1f), 0.08f * Random.Range(-1f, 1f));
			megaExplosionRandomRotationSpeeds[i] = 0.0005f;
		}
	}

	private bool TimeToActivate(int frame, int currFrameDo)
	{
		if (frame > currFrameDo)
		{
			return false;
		}
		if (activatedFrames.ContainsKey(frame))
		{
			return false;
		}
		activatedFrames.Add(frame, true);
		return true;
	}

	private void UpdateStateTransitions()
	{
		int num = (int)currFrame;
		if (TimeToActivate(30, num))
		{
			Audio.Play("enterscreen", EnterScreenSfx);
		}
		if (TimeToActivate(200, num))
		{
			state = State.TRANSIT_TO_TURRET_STATE;
			Audio.Play("towersout", TowersOutSfx);
		}
		if (TimeToActivate(210, num))
		{
			state = State.TURRETS_STATE;
			stopCurrFrame = true;
			currFrame += Time.deltaTime * 30f;
		}
		if (TimeToActivate(212, num))
		{
			Audio.Play("openup", OpenupSfx);
			state = State.MOVE_LOWER_CLAMPS_STATE;
		}
		if (TimeToActivate(222, num))
		{
			state = State.MOVE_UPPER_CLAMPS_STATE;
		}
		if (TimeToActivate(270, num))
		{
			state = State.MOVE_UPPER_BRAIN_STATE;
		}
		if (TimeToActivate(400, num))
		{
			Audio.Play("openupstop", OpenupStopSfx);
		}
		if (TimeToActivate(420, num))
		{
			Audio.Play("expandlaser", OpenupLaserSfx);
			state = State.ACTIVATE_LASERS;
		}
		if (TimeToActivate(520, num))
		{
			state = State.SPIN_LASERS;
		}
		if (TimeToActivate(521, num))
		{
			state = State.MOVE_LOWER_BRAIN_STATE;
		}
		if (TimeToActivate(522, num))
		{
			state = State.FINAL_FIGHT;
			stopCurrFrame = true;
		}
		if (num >= 523)
		{
			if (TimeToActivate(523, num))
			{
				state = State.MEGA_EXPLOSION;
				ve.DeActivateWeapons();
				ve.SetAlwaysInvincible(true);
				deScaleTwirls = true;
				Ring.StopLaser();
				LowerLeftClamp.ScaleToZero();
				LowerRightClamp.ScaleToZero();
				UpperLeftClamp.ScaleToZero();
				UpperRightClamp.ScaleToZero();
				LowerLeftClamp.FadeToZero();
				LowerRightClamp.FadeToZero();
				UpperLeftClamp.FadeToZero();
				UpperRightClamp.FadeToZero();
				SpawnExplosion(explosionOrigins[0]);
				SpawnExplosion(explosionOrigins[4]);
			}
			if (TimeToActivate(530, num))
			{
				SpawnExplosion(explosionOrigins[1]);
			}
			if (TimeToActivate(540, num))
			{
				SpawnExplosion(explosionOrigins[2]);
			}
			if (TimeToActivate(550, num))
			{
				SpawnExplosion(explosionOrigins[3]);
			}
			if (TimeToActivate(560, num))
			{
				SpawnExplosion(explosionOrigins[4]);
			}
			if (TimeToActivate(570, num))
			{
				SpawnExplosion(explosionOrigins[0]);
				LowerLeftBrain.FadeToZero();
				UpperArmor.FadeToZero();
				LowerRightBrain.FadeToZero();
				UpperLeftBrain.FadeToZero();
				UpperRightBrain.FadeToZero();
				SpawnMegaShockwave(base.transform.position, true);
			}
			if (TimeToActivate(575, num))
			{
				SpawnExplosion(explosionOrigins[5]);
			}
			if (TimeToActivate(580, num))
			{
				SpawnExplosion(explosionOrigins[1]);
			}
			if (TimeToActivate(585, num))
			{
				SpawnExplosion(explosionOrigins[6]);
			}
			if (TimeToActivate(590, num))
			{
				SpawnExplosion(explosionOrigins[2]);
			}
			if (TimeToActivate(595, num))
			{
				SpawnExplosion(explosionOrigins[7]);
			}
			if (TimeToActivate(600, num))
			{
				SpawnExplosion(explosionOrigins[3]);
			}
			if (TimeToActivate(605, num))
			{
				SpawnExplosion(explosionOrigins[8]);
			}
			if (TimeToActivate(610, num))
			{
				SpawnExplosion(explosionOrigins[4]);
			}
			if (TimeToActivate(615, num))
			{
				World.TriggerFlag(96);
				SpawnExplosion(explosionOrigins[9]);
			}
			if (TimeToActivate(625, num))
			{
				World.TriggerFlag(102);
				SpawnMegaExplosion(base.transform.position);
				SpawnMegaShockwave(base.transform.position, false);
				SetRenderersEnabled(false);
				FadeToBlack();
			}
			if (TimeToActivate(650, num))
			{
				World.TriggerFlag(109);
				World.TriggerFlag(98);
			}
		}
	}

	private void FadeToBlack()
	{
		World.TriggerFlag(109);
		CameraFade component = World._world.GetComponent<CameraFade>();
		component.StartFade(Color.black, 0.5f);
		Invoke("InitEndMorph", 0.5f);
	}

	private void InitEndMorph()
	{
		World.vessel.EndMorph();
		CameraFade component = World._world.GetComponent<CameraFade>();
		component.FadeIn();
	}

	private void SpawnMegaShockwave(Vector3 pPosition, bool pInverted)
	{
		Shockwave shockwave = ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", pPosition, 0.5f, 60);
		if (pInverted)
		{
			shockwave.Inverse();
		}
	}

	private void SpawnMegaExplosion(Vector3 pPosition)
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", pPosition, 3f);
		Audio.Play("ShooterBossMegaExpl", ExplodeSfx);
	}

	private void SpawnExplosion(Vector3 pPosition)
	{
		ObjectPool.instance.GetObjectForType<Particle>().Play("_explosion", pPosition);
		Audio.Play("ShooterBossExpl", ExplodeSfx);
	}

	private void UpdateState()
	{
		if (!stopCurrFrame)
		{
			currFrame += Time.deltaTime * 30f;
		}
		UpdateStateTransitions();
		Bars.UpdateState(state);
		Ring.UpdateState(state);
		LowerLeftBrain.UpdateState(state);
		LowerRightBrain.UpdateState(state);
		UpperLeftBrain.UpdateState(state);
		UpperRightBrain.UpdateState(state);
		LowerLeftClamp.UpdateState(state);
		LowerRightClamp.UpdateState(state);
		UpperLeftClamp.UpdateState(state);
		UpperRightClamp.UpdateState(state);
		UpperArmor.UpdateState(state);
		BrainGlow.UpdateState(state);
	}

	public void OnBrainDestroyed()
	{
		stopCurrFrame = false;
	}

	public void OnTurretsDestroyed()
	{
		stopCurrFrame = false;
	}
}
