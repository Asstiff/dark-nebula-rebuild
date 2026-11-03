using UnityEngine;

public abstract class Vessel : Entity
{
	private const int LIVES = 3;

	protected static int _lives = 3;

	protected static int _lifeOrbsCollected;

	public GameObject Gfx;

	public GameObject GfxShield;

	public GameObject GfxPuck;

	public GameObject GfxEnergy;

	public AudioClip SfxExtraLife;

	public AudioClip SfxExplode;

	public bool IsLookingForShield;

	[HideInInspector]
	public int shield;

	[HideInInspector]
	public int startShield;

	[HideInInspector]
	public int currentEnergy;

	[HideInInspector]
	public Vector2 lastFilteredAccelerometerPosition = Vector2.zero;

	protected int _shieldsLostOnThisLevel;

	protected int _livesLostOnThisLevel;

	protected int _lifeOrbsCollectedOnThisLevel;

	protected bool _isFalling;

	protected float _z;

	protected float _dz;

	public bool IsFalling
	{
		get
		{
			return _isFalling;
		}
	}

	public virtual bool IsInAir
	{
		get
		{
			return _z > 45f || _isFalling;
		}
	}

	public static int Lives
	{
		get
		{
			return _lives;
		}
	}

	public static int Orbs
	{
		get
		{
			return _lifeOrbsCollected;
		}
	}

	public int OrbsCollectedOnThisLevel
	{
		get
		{
			return _lifeOrbsCollectedOnThisLevel;
		}
	}

	public int LivesLostOnThisLevel
	{
		get
		{
			return _livesLostOnThisLevel;
		}
	}

	public int ShieldsLostOnThisLevel
	{
		get
		{
			return _shieldsLostOnThisLevel;
		}
	}

	public virtual void DisableMovement()
	{
	}

	public virtual void EndMorph()
	{
	}

	public void GainLives()
	{
		_lives = 3;
		World.hud.UpdateLives();
		Reset();
	}

	public virtual void Reset()
	{
	}

	public override bool PostInit()
	{
		base.alwaysAlive = true;
		if (base.PostInit())
		{
			return true;
		}
		if (UserSettings.I.expert)
		{
			startShield = 10;
		}
		else
		{
			startShield = 40;
		}
		if (_lives <= 0)
		{
			_lives = 3;
			_lifeOrbsCollected = 0;
		}
		return false;
	}

	public bool WantsSuperLifeOrbWithMaxLives(int maxlives)
	{
		return _lives <= maxlives + 1;
	}

	public abstract void TriggerFall(bool crashInWater);

	public abstract void TriggerDeathCollision();

	public abstract bool TriggerEnergy(int pEnergy);

	public abstract bool TriggerBulletCollision();

	public abstract void TriggerJump();

	public abstract void TriggerCheckpoint(Vector3 pCheckpoint);

	public abstract void ResetDrag();

	public abstract void ResetShield();

	public abstract void UpdateVesselGfx();

	public abstract bool TriggerLifeOrb(Vector3 position);

	public abstract void TriggerExtraLife();

	public abstract void TriggerSpike();

	public abstract void TriggerRecieveDamage();

	public abstract bool TriggerGiveShield();

	public abstract void TriggerVesselBullet(int color, bool hasLightEntity);

	public virtual void ReceiveDamage(int pDamage)
	{
	}

	public virtual void WaypointEnemy(Entity pEnemy)
	{
	}
}
