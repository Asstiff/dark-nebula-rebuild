using System;
using FLD;
using UnityEngine;

public class VesselMatrixSpawnEntity : VesselSpawnEntity
{
	private const int secondLensDoneFrame = 15;

	private const int animationDoneFrame = 30;

	public AudioClip endOfSpawnSound;

	public AudioClip spawnSound;

	public tk2dSprite balls;

	private VesselMatrixSpawnEntityAnimation balls1;

	private VesselMatrixSpawnEntityAnimation balls2;

	public tk2dSprite flare;

	private VesselMatrixSpawnEntityAnimation flare1;

	private VesselMatrixSpawnEntityAnimation flare2;

	private VesselMatrixSpawnEntityAnimation flare3;

	private VesselMatrixSpawnEntityAnimation flare4;

	public tk2dSprite lens;

	private VesselMatrixSpawnEntityAnimation lens1;

	private VesselMatrixSpawnEntityAnimation lens2;

	private float twirlsFrame;

	private int doneFlag;

	private bool startedTwirl;

	private bool isActive;

	public override void Init(Vector3 p, int length, int scrollTime)
	{
		throw new InvalidOperationException("Cannot call this function, override it");
	}

	public override void Init(Vector3 p, bool respawn, int flag, int length, int scrollTime)
	{
		base.Init(p, length, scrollTime);
		if (balls1 == null)
		{
			balls1 = new VesselMatrixSpawnEntityAnimation(3.5f, 0.3f, 100f, 300f, 0f, 1f, balls.gameObject);
			balls2 = new VesselMatrixSpawnEntityAnimation(1.8f, 0.15f, 45f, 110f, 0f, 1f, MakeCopy(balls.gameObject));
			flare1 = new VesselMatrixSpawnEntityAnimation(4.5f, 0.45f, -90f, 38f, 0f, 1f, flare.gameObject);
			flare2 = new VesselMatrixSpawnEntityAnimation(1.9f, 0.15f, 0f, 170f, 0f, 1f, MakeCopy(flare.gameObject));
			flare3 = new VesselMatrixSpawnEntityAnimation(0.05f, 0.65f, 0f, -90f, 1f, 1f, MakeCopy(flare.gameObject));
			flare4 = new VesselMatrixSpawnEntityAnimation(1.9f, 0.15f, 65f, 300f, 0f, 1f, MakeCopy(flare.gameObject));
			lens1 = new VesselMatrixSpawnEntityAnimation(0.8f, 0.9f, 0f, -220f, 0f, 1f, lens.gameObject);
			lens2 = new VesselMatrixSpawnEntityAnimation(0.55f, 0.65f, -145f, -290f, 0f, 1f, MakeCopy(lens.gameObject));
		}
		if (respawn)
		{
			twirlsFrame = 30f;
		}
		doneFlag = flag;
		base.transform.Translate(-1000f, 0f, 0f);
		isActive = true;
	}

	private GameObject MakeCopy(GameObject original)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		return gameObject;
	}

	public override void Update()
	{
		if (isActive && !IsPooled && Time.timeScale != 0f)
		{
			base.Update();
			if (startedTwirl)
			{
				UpdateTwirl();
			}
		}
	}

	private void UpdateTwirl()
	{
		if (twirlsFrame <= 30f)
		{
			float num = twirlsFrame / 30f;
			(World.vessel as StandardVessel).SetTwirlFactor(num);
			if (World.Darkness != null)
			{
				(World.vessel as StandardVessel).SetLightScale(World.Darkness.playerLightScale, World.Darkness.playerLightMinScale, World.Darkness.playerLightMaxScale);
			}
			if (balls1 != null)
			{
				balls1.Update(num);
				balls2.Update(num);
				flare1.Update(num);
				flare2.Update(num);
				flare3.Update(num);
				flare4.Update(num);
				lens1.Update(num);
				lens2.Update(num);
			}
			else
			{
				Debug.Log("balls1 == null");
			}
		}
		if (IsAnimationDone())
		{
			World.TriggerFlag(102);
			StandardVessel standardVessel = (StandardVessel)World.vessel;
			standardVessel.Spawn(skinType);
			standardVessel.DropFromSky();
			standardVessel.DisableMovementInAir();
			ObjectPool.instance.GetObjectForType<Shockwave>().Play("Big", base.transform.position, 0.7f, 8);
			Audio.Play("endOfSpawnSound", endOfSpawnSound, transform);
		}
		if (IsEffectDone())
		{
			(World.vessel as StandardVessel).EnableMovement();
			World.TriggerFlag(doneFlag);
			isActive = false;
			ObjectPool.instance.PoolObject(this);
		}
		twirlsFrame += Time.deltaTime * 30f;
	}

	public override void OnIntroductionDone()
	{
		startedTwirl = true;
		base.transform.Translate(1000f, 0f, 0f);
		Audio.Play("spawnSound", spawnSound, transform);
	}

	public bool IsAnimationDone()
	{
		return twirlsFrame >= 30f;
	}

	protected bool IsEffectDone()
	{
		return twirlsFrame > 30f;
	}
}
