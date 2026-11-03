using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class ShooterEnemyWaveEntity : Entity
{
	private int enemyType;

	private int deadEnemies;

	private bool firstFrame;

	private bool dropWeapon;

	private float numBullets;

	private float numEnemies;

	private float initialAngle;

	private int releasedEnemies;

	private int releasedBullets;

	private float bulletVelocity;

	private float timeLastReleased;

	private float initialVelocity;

	private float timeToNextBullet;

	private float dontTriggerUntilLength;

	private float initialAngularVelocity;

	private float initialStrafeVelocityY;

	private float initialStrafeVelocityX;

	private float secondsBetweenConsecutiveEnemies;

	private float secondsBetweenConsecutiveBullets;

	private bool compensateStrafeForCameraMovement;

	private bool compensateSpawnPointForCameraMovement;

	private List<ShooterEnemyEntity> enemies = new List<ShooterEnemyEntity>();

	protected override void Awake()
	{
		base.keepalive = true;
		base.Awake();
	}

	private void Start()
	{
		enemyType = GetParameterInt("enemyType");
		dropWeapon = GetParameterBool("dropWeapon");
		numEnemies = GetParameterFloat("numEnemies");
		numBullets = GetParameterFloat("numBullets");
		initialAngle = GetParameterFloat("initialAngle");
		bulletVelocity = GetParameterFloat("bulletVelocity");
		initialVelocity = GetParameterFloat("initialVelocity");
		timeToNextBullet = GetParameterFloat("initialBulletDelay");
		dontTriggerUntilLength = GetParameterFloat("dontTriggerUntilLength");
		initialAngularVelocity = GetParameterFloat("initialAngularVelocity") * ((float)System.Math.PI / 180f);
		initialStrafeVelocityX = GetParameterFloat("initialStrafeVelocityX");
		initialStrafeVelocityY = GetParameterFloat("initialStrafeVelocityY");
		secondsBetweenConsecutiveEnemies = GetParameterFloat("secondsBetweenConsecutiveEnemies");
		secondsBetweenConsecutiveBullets = GetParameterFloat("secondsBetweenConsecutiveBullets");
		compensateStrafeForCameraMovement = GetParameterBool("compensateStrafeForCameraMovement");
		compensateSpawnPointForCameraMovement = GetParameterBool("compensateSpawnPointForCameraMovement");
		releasedEnemies = 0;
		deadEnemies = 0;
		firstFrame = true;
	}

	private ShooterEnemyEntity CreateEnemy(int i)
	{
		ShooterEnemyEntity shooterEnemyEntity = null;
		switch (enemyType)
		{
		case 0:
			shooterEnemyEntity = ObjectPool.instance.GetObjectForType<TurnerEnemyEntity>();
			(shooterEnemyEntity as TurnerEnemyEntity).Init(base.transform.position, (float)System.Math.PI / 180f * (180f - initialAngle), i, dropWeapon);
			break;
		case 1:
			shooterEnemyEntity = ObjectPool.instance.GetObjectForType<SpinnerEnemyEntity>();
			(shooterEnemyEntity as SpinnerEnemyEntity).Init(base.transform.position, (float)System.Math.PI / 180f * (180f - initialAngle), i);
			break;
		default:
			Debug.LogError("Unknown shooter enemy type used");
			break;
		}
		shooterEnemyEntity.enabled = true;
		shooterEnemyEntity.Dead = false;
		shooterEnemyEntity.Died += OnDie;
		shooterEnemyEntity.PostInit();
		enemies.Add(shooterEnemyEntity);
		return shooterEnemyEntity;
	}

	private void Update()
	{
		if (Time.deltaTime == 0f)
		{
			return;
		}
		float num = World.cameraFollow.windowPosZ - base.transform.position.z + 240f;
		if (num > dontTriggerUntilLength)
		{
			if (firstFrame || (Time.time - timeLastReleased > secondsBetweenConsecutiveEnemies && (float)releasedEnemies < numEnemies))
			{
				ShooterEnemyEntity shooterEnemyEntity = CreateEnemy(releasedEnemies);
				shooterEnemyEntity.preAllocated = false;
				if (compensateSpawnPointForCameraMovement)
				{
					float num2 = num - dontTriggerUntilLength;
					shooterEnemyEntity.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z + num2);
				}
				releasedEnemies++;
				timeLastReleased = Time.time;
			}
			StarfighterCamera starfighterCamera = World.CameraManager.FindHighestPriorityCamera() as StarfighterCamera;
			if (starfighterCamera == null)
			{
				return;
			}
			Vector2 pStrafeVector = new Vector2(initialStrafeVelocityX, initialStrafeVelocityY) * Time.deltaTime;
			if (compensateStrafeForCameraMovement)
			{
				pStrafeVector.y -= starfighterCamera.Speed * Time.deltaTime * 30f;
			}
			for (int i = 0; i < releasedEnemies; i++)
			{
				ShooterEnemyEntity shooterEnemyEntity2 = enemies[i];
				if (shooterEnemyEntity2 != null && !shooterEnemyEntity2.IsPooled && !shooterEnemyEntity2.Dead && !shooterEnemyEntity2.IsFrozen)
				{
					shooterEnemyEntity2.SetVel(initialVelocity * Time.deltaTime, pStrafeVector, initialAngularVelocity * Time.deltaTime);
				}
			}
			if (timeToNextBullet <= 0f && (float)releasedBullets < numBullets)
			{
				for (int j = 0; j < releasedEnemies; j++)
				{
					ShooterEnemyEntity shooterEnemyEntity3 = enemies[j];
					if (shooterEnemyEntity3 != null && !shooterEnemyEntity3.IsPooled && !shooterEnemyEntity3.Dead && !shooterEnemyEntity3.IsFrozen)
					{
						Vector3 position = shooterEnemyEntity3.transform.position;
						Vector2 normalized = FLD.Math.Delta2d(World.vessel.transform.position, position).normalized;
						normalized.x *= bulletVelocity;
						normalized.y *= bulletVelocity;
						normalized.y += starfighterCamera.Speed;
						ShooterEnemyBulletEntity objectForType = ObjectPool.instance.GetObjectForType<ShooterEnemyBulletEntity>();
						objectForType.transform.parent = base.transform.parent;
						objectForType.Init(position, normalized, "bullet2", 1f, 10f, false);
						break;
					}
				}
				timeToNextBullet = secondsBetweenConsecutiveBullets;
				releasedBullets++;
			}
			timeToNextBullet -= Time.deltaTime;
		}
		if (firstFrame)
		{
			firstFrame = false;
		}
	}

	private void OnDie(Entity e)
	{
		ShooterEnemyEntity shooterEnemyEntity = e as ShooterEnemyEntity;
		shooterEnemyEntity.Died -= OnDie;
		deadEnemies++;
		for (int i = 0; i < enemies.Count; i++)
		{
			if (enemies[i] == shooterEnemyEntity)
			{
				enemies[i] = null;
				break;
			}
		}
		if (dropWeapon && (float)deadEnemies == numEnemies)
		{
			StarfighterWeaponsPickupEntity objectForType = ObjectPool.instance.GetObjectForType<StarfighterWeaponsPickupEntity>();
			objectForType.Init(shooterEnemyEntity.transform.position);
		}
	}

	public override void RemoveFromWorld()
	{
		foreach (ShooterEnemyEntity enemy in enemies)
		{
			if (enemy != null)
			{
				enemy.RemoveFromWorld();
			}
		}
		enemies.Clear();
		base.RemoveFromWorld();
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
	}
}
