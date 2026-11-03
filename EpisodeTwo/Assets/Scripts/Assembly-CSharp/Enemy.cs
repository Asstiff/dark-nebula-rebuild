using FLD;
using UnityEngine;

public class Enemy : Entity
{
	public int _health;

	public bool enableWaypoints;

	public AudioClip DamageSound;

	public AudioClip ExplodeSound;

	public int color;

	protected int maxHealth;

	public int dieFlag;

	protected bool isDead;

	protected bool isActivated;

	protected bool givesDamage;

	protected bool removeFromWorldOnDeath;

	protected bool isStunned;

	protected bool enableDamageShake;

	protected bool enableDieShake;

	protected bool enableDamageDebris;

	protected bool enableDamageShockWave;

	protected bool isAlerted;

	protected float invincibleTimer;

	protected float invincibleTimerMax;

	protected float stunTimer;

	protected float stunTime;

	protected float timeSinceDamage = 50f;

	protected VesselDetection _detection;

	private StateMachine _state;

	private StateMachineBehaviour _stateMachineBehaviour;

	protected VesselDetection detection
	{
		get
		{
			if (_detection == null)
			{
				_detection = base.gameObject.GetOrAddComponent<VesselDetection>();
			}
			return _detection;
		}
	}

	public StateMachine state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			if (_state == null)
			{
				if (_stateMachineBehaviour != null)
				{
					_stateMachineBehaviour.Host = null;
					Object.Destroy(_stateMachineBehaviour);
					_stateMachineBehaviour = null;
				}
			}
			else
			{
				if (_stateMachineBehaviour == null)
				{
					_stateMachineBehaviour = base.gameObject.GetOrAddComponent<StateMachineBehaviour>();
				}
				_stateMachineBehaviour.Host = this;
			}
		}
	}

	public bool IsDead
	{
		get
		{
			return isDead;
		}
	}

	public bool IsActivated
	{
		get
		{
			return isActivated;
		}
		set
		{
			isActivated = value;
		}
	}

	public float Scale
	{
		set
		{
			base.transform.localScale = new Vector3(value, value, value);
		}
	}

	public int Health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = value;
			OnHealthChanged();
		}
	}

	public bool IsAwareOfVessel
	{
		get
		{
			return isAlerted || timeSinceDamage < 5f || (detection != null && detection.AwareOfVessel);
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

	private void InitParameters()
	{
		color = Constants.GetColorInt(GetParameterString("color"));
	}

	protected virtual void OnHealthChanged()
	{
	}

	public override void OnTakenFromPool()
	{
		base.OnTakenFromPool();
		isDead = false;
	}

	protected virtual void Start()
	{
		stunTimer = 0f;
		stunTime = 0.3f;
		removeFromWorldOnDeath = true;
		enableDamageShockWave = true;
		enableDieShake = true;
		enableDamageDebris = true;
		enableDamageShake = true;
		givesDamage = false;
		isDead = false;
		enableWaypoints = false;
		Dead = false;
		invincibleTimerMax = 0f;
		if (isCreatedFromLevel)
		{
			dieFlag = GetParameterInt("dieFlag");
			maxHealth = GetParameterInt("maxHealth");
			Health = maxHealth;
		}
	}

	public virtual void ReceiveDamage(int damage, RotationAngle impactDirectionRadian = null)
	{
		if (Health != 0 && !(invincibleTimer > 0f))
		{
			Health -= damage;
			if (Health <= 0)
			{
				Health = 0;
				OnDie(impactDirectionRadian);
			}
			else
			{
				OnDamage(impactDirectionRadian);
			}
			invincibleTimer = invincibleTimerMax;
		}
	}

	private void Die()
	{
		if (Health != 0)
		{
			Health = 0;
			OnDie(new RotationAngle());
		}
	}

	protected virtual void OnDamage(RotationAngle pImpactAngle)
	{
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		if (objectForType != null)
		{
			if (color < 1 || color > 3)
			{
				objectForType.Play(string.Empty, base.transform.position, 1f);
			}
			else
			{
				objectForType.Play("-" + Constants.GetColorString(color), base.transform.position, 1f);
			}
		}
		if (enableDamageShockWave)
		{
			Shockwave objectForType2 = ObjectPool.instance.GetObjectForType<Shockwave>();
			if (objectForType2 != null)
			{
				objectForType2.Play("Big", base.transform.position, 0.7f, 8);
			}
		}
		Audio.Play("damageSound" + Random.value, DamageSound, base.transform.position);
		if (enableDamageShake)
		{
			World.TriggerFlag(100);
		}
		stunTimer = 0f;
		isStunned = true;
		if (enableDamageDebris)
		{
			SpawnDebrisParticles(pImpactAngle);
		}
		timeSinceDamage = 0f;
	}

	protected virtual void OnDie(RotationAngle pImpactDirection)
	{
		if (!isDead && !Dead)
		{
			isDead = true;
			Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
			if (objectForType != null)
			{
				objectForType.Play("_explosion", base.transform.position, 1f);
			}
			if (enableDamageShockWave)
			{
				Shockwave objectForType2 = ObjectPool.instance.GetObjectForType<Shockwave>();
				objectForType2.Play("Big", base.transform.position, 0.7f, 8);
			}
			Audio.Play("damageSound" + Random.value, DamageSound, base.transform.position);
			Audio.Play("explodeSound" + Random.value, ExplodeSound, base.transform.position);
			if (dieFlag != 0)
			{
				World.TriggerFlag(dieFlag);
			}
			if (enableDieShake)
			{
				World.TriggerFlag(101);
			}
			SpawnDebrisParticles(pImpactDirection);
			if (removeFromWorldOnDeath)
			{
				RemoveFromWorld();
			}
		}
	}

	public override void RemoveFromWorld()
	{
		Object.Destroy(base.gameObject);
	}

	public void SpawnDebrisParticles(RotationAngle pRadian)
	{
		for (int i = 0; i < 7; i++)
		{
			SpawnDebrisParticle(0.5f);
		}
	}

	public void SpawnDebrisParticle(float pScale)
	{
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		if (objectForType == null)
		{
			Debug.LogError("Could not create <Particle> from objectpool.");
		}
		else
		{
			objectForType.Play("_debris" + Random.Range(1, 4), base.transform.position, pScale, Random.value * 200f + 100f, true);
		}
	}

	protected virtual void OnCollisionEnter(Collision pCollision)
	{
		if (givesDamage && pCollision.gameObject.CompareTag("Player"))
		{
			World.vessel.ReceiveDamage(1);
		}
	}

	public virtual void Update()
	{
		if (!IsPooled)
		{
			timeSinceDamage += Time.deltaTime;
			invincibleTimer -= Time.deltaTime;
			stunTimer += Time.deltaTime;
			if (stunTimer > stunTime)
			{
				isStunned = false;
				stunTimer = stunTime;
			}
			if (enableWaypoints && !IsPooled && !Dead && !isDead && World.vessel != null && !World.EntityWithinScreen(this))
			{
				World.vessel.WaypointEnemy(this);
			}
		}
	}
}
