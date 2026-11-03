using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Enemy
{
	public tk2dSprite Hatch;

	public float respawntime;

	private int _hatchFrame;

	private float blinkTimer;

	private bool _lightsOn;

	private bool lightsAlwaysOn;

	private float blinkTime;

	private List<Enemy> spawnedEnemies;

	private FlagCondition dieEvent;

	private tk2dSprite _sprite;

	private string _hatchName;

	private int _frame;

	private int numberofenemies;

	public bool lightsOn
	{
		get
		{
			return _lightsOn;
		}
		set
		{
			_lightsOn = value;
			sprite.spriteId = sprite.GetSpriteIdByName("enemy_spawner_bottom/" + (_lightsOn ? color : 0));
		}
	}

	private tk2dSprite sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponent<tk2dSprite>();
			}
			return _sprite;
		}
	}

	public int hatchFrame
	{
		get
		{
			return _hatchFrame;
		}
		set
		{
			_hatchFrame = value;
			Hatch.spriteId = Hatch.GetSpriteIdByName(_hatchName + "/" + _hatchFrame);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (isCreatedFromLevel)
		{
			Init();
		}
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		Init();
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		Init();
	}

	private void Init()
	{
		respawntime = GetParameterInt("respawntime");
		numberofenemies = GetParameterInt("numberofenemies");
		base.Health = (maxHealth = 3);
		_hatchName = "enemy_spawner_top";
		hatchFrame = 6;
		blinkTimer = 0f;
		base.state = new StateMachine(new EnemySpawnerIdle(this));
		lightsOn = true;
		lightsAlwaysOn = true;
		spawnedEnemies = new List<Enemy>();
		dieEvent = new FlagCondition(GetParameterInt("dieEventFlag"), GetParameterInt("dieEventCount"));
		dieEvent.Callback += onDieEvent;
		removeFromWorldOnDeath = false;
	}

	protected override void OnHealthChanged()
	{
	}

	private void onDieEvent()
	{
		OnDie(new RotationAngle());
	}

	public override void Update()
	{
		if (IsFrozen)
		{
			return;
		}
		base.Update();
		if (isDead)
		{
			lightsOn = false;
		}
		else if (lightsAlwaysOn)
		{
			blinkTimer = 0f;
			lightsOn = true;
		}
		else
		{
			blinkTimer += Time.deltaTime;
			if (blinkTimer > blinkTime)
			{
				lightsOn = !lightsOn;
				blinkTimer = 0f;
			}
		}
		List<Enemy> list = new List<Enemy>();
		foreach (Enemy spawnedEnemy in spawnedEnemies)
		{
			if (spawnedEnemy.IsDead)
			{
				list.Add(spawnedEnemy);
			}
		}
		spawnedEnemies.Exclude(list);
	}

	public bool WantsToSpawn()
	{
		return !isDead && numberofenemies - spawnedEnemies.Count > 0;
	}

	public Enemy SpawnEnemy()
	{
		BattlePuck objectForType = ObjectPool.instance.GetObjectForType<BattlePuck>();
		if (objectForType == null)
		{
			return null;
		}
		objectForType.Mini = GetParameterInt("enemyType");
		objectForType.hasLightEntity = GetParameterBool("litEnemies");
		objectForType.SetMaximumSpeed(GetParameterFloat("maximumSpeed"));
		objectForType.color = color;
		objectForType.InitWithPatrol();
		objectForType.transform.position = base.transform.position;
		objectForType.PostInit();
		objectForType.gameObject.SetActive(true);
		spawnedEnemies.Add(objectForType);
		return objectForType;
	}

	public void SetHatchOpen(int frame)
	{
		hatchFrame = frame;
	}

	public void SmashItUp()
	{
		hatchFrame = 7;
	}

	public void TurnBlinkOff()
	{
		lightsAlwaysOn = true;
		blinkTimer = 0f;
		blinkTime = 0f;
	}

	public void SetBlinkFrequency(float timesPerSecond)
	{
		blinkTime = 1f / timesPerSecond;
		lightsAlwaysOn = false;
	}

	public override void ReceiveDamage(int damage, RotationAngle impactDirectionRadian)
	{
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (dieEvent != null)
		{
			dieEvent.Callback -= onDieEvent;
			dieEvent.Dispose();
			dieEvent = null;
		}
	}
}
