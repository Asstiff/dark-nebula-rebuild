using System;
using FLD;
using UnityEngine;

public class BattlePuck : Enemy
{
	private const float pushScale = 5f;

	public bool _hasLightEntity;

	public bool IsMovementEnabled;

	public float bladeRotation;

	public float bladeScale;

	public AudioClip MurtyWarningSound;

	public AudioClip ClusterpuckActivated_SFX;

	public AudioClip FastEnemyBladesOut_SFX;

	public AudioClip EnemyBladesOut_SFX;

	public tk2dSprite TopSprite;

	public tk2dSprite BodySprite;

	public tk2dSprite BottomSprite;

	public LightEntity lightEntity;

	protected float drag;

	private int mini;

	private int layer;

	private int alertFriendsTriggerFlag;

	private float currFrame;

	private int _frame;

	private int _bladeFrameOffset;

	private int alertEventFlag;

	private bool listensToAlert;

	private bool wasInAir;

	private float wasInAirTimer;

	private float maximumSpeed;

	private float originalMaximumSpeed;

	private float rotationspeed;

	private float timeSinceAlerted;

	private float miniRotation;

	private float targetSpeed;

	private float rotation;

	private float speed;

	private string _spriteName = "Entity_enemy_battlepuck";

	private Vector2 pushVector;

	private FlagConditionChanged alertEvent;

	private Bump _bump;

	private CapsuleCollider _capsuleCollider;

	private static int created;

	public bool hasLightEntity
	{
		get
		{
			return _hasLightEntity;
		}
		set
		{
			_hasLightEntity = value;
			if (lightEntity != null)
			{
				lightEntity.SetVisible(_hasLightEntity);
			}
		}
	}

	private tk2dSprite sprite
	{
		get
		{
			return BodySprite;
		}
	}

	private tk2dSprite bladeSprite
	{
		get
		{
			return BottomSprite;
		}
	}

	public int frame
	{
		get
		{
			return _frame;
		}
		set
		{
			if (_frame != value)
			{
				_frame = value;
				sprite.spriteId = sprite.GetSpriteIdByName(_spriteName + "/" + _frame);
			}
		}
	}

	public string SpriteNameAndBlade
	{
		get
		{
			return _spriteName;
		}
		set
		{
			_spriteName = value;
			sprite.spriteId = sprite.GetSpriteIdByName(_spriteName + "/" + _frame);
			sprite.GetComponent<Renderer>().enabled = true;
			bladeSprite.spriteId = sprite.GetSpriteIdByName(_spriteName + "/" + (_frame + _bladeFrameOffset));
			bladeSprite.GetComponent<Renderer>().enabled = true;
		}
	}

	private Bump bump
	{
		get
		{
			if (_bump == null)
			{
				_bump = GetComponentInChildren<Bump>();
			}
			return _bump;
		}
	}

	public int Mini
	{
		get
		{
			return mini;
		}
		set
		{
			mini = value;
		}
	}

	private CapsuleCollider capsuleCollider
	{
		get
		{
			if (_capsuleCollider == null)
			{
				_capsuleCollider = base.GetComponent<Collider>() as CapsuleCollider;
			}
			return _capsuleCollider;
		}
	}

	private void SetSpriteAndBlade(string name, int frame)
	{
		_frame = frame;
		SpriteNameAndBlade = name;
	}

	public void InitWithPatrol()
	{
		InitAfterStart();
		StateMachineState stateMachineState = new BattlePuckSpawn(this);
		base.state = new StateMachine(stateMachineState);
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		InitParameters(false);
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
		InitParameters(true);
	}

	private void InitParameters(bool IsRuntime)
	{
		if (!isCreatedFromLevel)
		{
			return;
		}
		hasLightEntity = GetParameterBool("hasLightEntity");
		if (GetParameterInt("mini") == 3 || GetParameterInt("mini") == 2)
		{
			Bump bump = GetComponentInChildren<Bump>();
			if (bump == null)
			{
				bump = ((!IsRuntime) ? InstatiatePrefab("Assets/Prefabs/Entities/BumpEntity.prefab").GetComponent<Bump>() : LevelEP2Runtime.Instatiate("Assets/Prefabs/Entities/BumpEntity.prefab").GetComponent<Bump>());
			}
			if (bump == null)
			{
				Debug.LogError("Could not create bump on BattlePuck");
				return;
			}
			bump.transform.parent = base.transform;
			bump.transform.localPosition = Vector3.zero;
			bump.SetRenderersEnabled(false);
			bump.SetRadius(22.5f);
		}
	}

	private GameObject InstatiatePrefab(string path)
	{
		return null;
	}

	private void InitWithWorld(Vector3 pos)
	{
		base.transform.position = pos;
		InitAfterStart();
	}

	private void InitAfterStart()
	{
		HideSprite(BottomSprite);
		HideSprite(BodySprite);
		HideSprite(TopSprite);
		enableWaypoints = true;
		maximumSpeed = GetParameterFloat("maximumSpeed");
		alertFriendsTriggerFlag = GetParameterInt("alertFriendsTriggerFlag");
		listensToAlert = GetParameterInt("listensToAlert") == 1;
		alertEventFlag = GetParameterInt("alertEventFlag");
		switch (mini)
		{
		case 0:
			SetSize(65f);
			base.Health = (maxHealth = 3);
			_bladeFrameOffset = 4;
			SetSpriteAndBlade("Entity_enemy_battlepuck", 1);
			break;
		case 1:
			base.Health = (maxHealth = 3);
			SetSize(38f);
			_bladeFrameOffset = 4;
			SetSpriteAndBlade("Entity_enemy_minibattlepuck", (color == 1) ? 8 : 0);
			break;
		case 2:
			base.Health = 1;
			SetSize(42f);
			if (ObjectPool.instance != null && bump == null)
			{
				_bump = ObjectPool.instance.GetObjectForType<Bump>();
				_bump.SetRenderersEnabled(false);
				_bump.SetRadius(22.5f);
				_bump.transform.parent = base.transform;
				_bump.transform.localPosition = Vector3.zero;
				_bump.GetComponent<Collider>().enabled = true;
			}
			base.GetComponent<Collider>().isTrigger = false;
			_bladeFrameOffset = 2;
			SetSpriteAndBlade("triforce_small", 0);
			break;
		case 3:
			base.Health = (maxHealth = 1);
			SetSize(65f);
			SetSprite(BottomSprite, "triforce_large/1");
			SetSprite(BodySprite, "triforce_large/2");
			SetSprite(TopSprite, "triforce_small/1");
			break;
		case 4:
			base.keepalive = true;
			base.Health = (maxHealth = 1);
			SetSize(65f);
			SetSpriteAndBlade("clusterpuck_large", 1);
			SetSprite(BottomSprite, "clusterpuck_large/2");
			SetSprite(TopSprite, "clusterpuck_small/1");
			break;
		case 5:
			base.keepalive = true;
			base.Health = (maxHealth = 1);
			SetSize(65f);
			SetSprite(BodySprite, "clusterpuck_small/0");
			SetSprite(BottomSprite, "clusterpuck_small/2");
			HideSprite(TopSprite);
			base.GetComponent<Collider>().isTrigger = false;
			break;
		default:
			Debug.LogWarning("Not implemented BattlePuck mini: " + mini);
			break;
		}
		if (!hasLightEntity)
		{
			layer++;
		}
		drag = 0.6f;
		SetMaximumSpeed(maximumSpeed);
		originalMaximumSpeed = maximumSpeed;
		rotationspeed = 0.2f;
		bladeRotation = 0f;
		bladeScale = 0f;
		rotation = (float)System.Math.PI / 2f;
		currFrame = 0f;
		StateMachineState stateMachineState = new BattlePuckStandby(this);
		base.state = new StateMachine(stateMachineState);
		if (listensToAlert)
		{
			alertEventFlag = alertFriendsTriggerFlag;
		}
		else
		{
			alertEventFlag = 0;
		}
		alertEvent = new FlagConditionChanged(alertEventFlag);
		alertEvent.Callback += OnAlertEvent;
	}

	private void SetSprite(tk2dSprite pSprite, string pSpriteName)
	{
		pSprite.spriteId = pSprite.GetSpriteIdByName(pSpriteName);
		pSprite.GetComponent<Renderer>().enabled = true;
	}

	private void HideSprite(tk2dSprite pSprite)
	{
		pSprite.GetComponent<Renderer>().enabled = false;
	}

	protected override void Start()
	{
		base.Start();
		if (isCreatedFromLevel)
		{
			Mini = GetParameterInt("mini");
		}
		InitAfterStart();
	}

	private void SetSize(float diameter)
	{
		capsuleCollider.radius = diameter / 2f;
	}

	public void LateUpdate()
	{
		if (!IsPooled)
		{
			int num = mini;
			if (num == 3)
			{
				BottomSprite.transform.localRotation = Quaternion.Euler(0f, 0f, miniRotation * 57.29578f);
				BodySprite.transform.localRotation = Quaternion.Euler(0f, 0f, bladeRotation * 57.29578f);
			}
			else
			{
				BottomSprite.transform.localRotation = Quaternion.Euler(0f, 0f, bladeRotation * 57.29578f);
				BottomSprite.transform.localScale = new Vector3(bladeScale, 1f, bladeScale);
			}
		}
	}

	protected override void OnHealthChanged()
	{
		base.OnHealthChanged();
		if (mini == 0 || mini == 1)
		{
			int num = ((color != 1) ? 8 : 0);
			if (mini == 1)
			{
				num = ((color == 1) ? 8 : 0);
			}
			if (_health == 2)
			{
				frame = 2 + num;
			}
			else if (_health == 1)
			{
				frame = 3 + num;
			}
			else if (isActivated)
			{
				frame = 1 + num;
			}
			else
			{
				frame = 0 + num;
			}
		}
	}

	public override void Update()
	{
		if (IsPooled)
		{
			return;
		}
		base.Update();
		if (isAlerted)
		{
			timeSinceAlerted += Time.deltaTime;
			if (timeSinceAlerted >= 10f)
			{
				isAlerted = false;
			}
		}
		switch (mini)
		{
		case 2:
		case 3:
		{
			UpdateKusokoodo();
			StandardVessel standardVessel = (StandardVessel)World.vessel;
			if (standardVessel != null)
			{
				if (!standardVessel.IsInAir && wasInAirTimer <= 0f)
				{
					if (bump != null)
					{
						_bump.DontBump = true;
					}
					float magnitude = (base.transform.position - standardVessel.transform.position).magnitude;
					if (magnitude <= (float)((mini != 2) ? 80 : 45) && currFrame > 0.5f)
					{
						standardVessel.ReceiveDamage(1);
					}
					if (base.gameObject.layer != 0)
					{
						base.gameObject.layer = 0;
					}
				}
				else
				{
					if (bump != null)
					{
						_bump.DontBump = false;
					}
					if (base.gameObject.layer != 13)
					{
						base.gameObject.layer = 13;
					}
				}
			}
			if (bump != null && _bump.HasBeenHit())
			{
				ReceiveDamage(1, new RotationAngle());
			}
			break;
		}
		}
		currFrame += Time.deltaTime;
		givesDamage = bladeScale > 1f;
		bladeRotation += rotationspeed * Time.deltaTime;
		if (mini > 0)
		{
			miniRotation += 0.02f * Time.deltaTime * 0.5f;
		}
	}

	private void UpdateKusokoodo()
	{
		StandardVessel standardVessel = World.vessel as StandardVessel;
		if (standardVessel == null)
		{
			return;
		}
		if (!standardVessel.IsInAir)
		{
			if (wasInAir)
			{
				wasInAirTimer = 1f;
			}
			wasInAir = false;
		}
		else
		{
			wasInAir = true;
		}
		if (wasInAirTimer > 0f)
		{
			wasInAirTimer -= Time.deltaTime;
		}
	}

	public void UpdateNormalMovement()
	{
		if (Time.timeScale != 0f && !(World.vessel == null))
		{
			float a = FLD.Math.AngleFromPositions(World.vessel.transform.position, base.transform.position);
			float num = (float)System.Math.PI / 250f * Time.deltaTime * 30f;
			float num2 = (0f - SmallestDifference(a, rotation)) * Time.deltaTime * 30f;
			float num3 = num2 - num;
			rotation += num3 * Time.deltaTime * 9f;
			Vector3 vector = FLD.Math.AngleToVector3Z(rotation);
			float num4 = Mathf.Min(World.Difficulty, 1f);
			float num5 = targetSpeed * num4;
			if (isStunned)
			{
				num5 = 0f;
			}
			speed += (num5 - speed) * Time.deltaTime * 5f * num4;
			Vector3 vector2 = vector * speed * (1f - drag);
			if ((mini == 2 || mini == 5) && (pushVector.x != 0f || pushVector.y != 0f))
			{
				vector2.x += pushVector.x;
				vector2.z += pushVector.y;
				pushVector.x *= 0.95f;
				pushVector.y *= 0.95f;
			}
			base.GetComponent<Rigidbody>().MovePosition(base.transform.position + vector2);
		}
	}

	public static float SmallestDifference(float a, float b)
	{
		float f = b - a;
		if (Mathf.Abs(f) < 0.01f)
		{
			return 0f;
		}
		return Mathf.Atan2(Mathf.Sin(f), Mathf.Cos(f));
	}

	protected override void OnDamage(RotationAngle angle)
	{
		if (IsMovementEnabled)
		{
			base.OnDamage(angle);
			SpawnSparks(angle);
			timeSinceDamage = 0f;
		}
	}

	protected override void OnDie(RotationAngle angle)
	{
		if (mini == 3 || mini == 4)
		{
			int pNewMini = ((mini != 3) ? 5 : 2);
			bool flag = mini == 3;
			Vector3 zero = Vector3.zero;
			for (float num = 0f; num < 3f; num++)
			{
				float f = miniRotation - (float)System.Math.PI / 180f * (30f - num * -120f);
				zero.x = Mathf.Cos(f) * 30f;
				zero.z = Mathf.Sin(f) * 30f;
				BattlePuck battlePuck = CreateBattlePuck(zero * 2f, pNewMini, 1);
				if (flag)
				{
					battlePuck.maximumSpeed = 2f;
				}
				battlePuck.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
				battlePuck.GetComponent<Rigidbody>().AddForce(zero * 5f * 2f);
			}
		}
		SpawnSparks(angle);
		base.OnDie(angle);
	}

	private BattlePuck CreateBattlePuck(Vector3 p, int pNewMini, int pColor)
	{
		BattlePuck objectForType = ObjectPool.instance.GetObjectForType<BattlePuck>();
		objectForType.ResetParameters();
		objectForType.maximumSpeed = maximumSpeed;
		objectForType.Mini = pNewMini;
		objectForType.transform.parent = base.transform.parent;
		objectForType.dieFlag = dieFlag;
		objectForType.color = pColor;
		objectForType.InitWithWorld(p + base.transform.position);
		objectForType.gameObject.SetActive(true);
		created++;
		Debug.Log("created " + created + " BattlePucks. This one is mini: " + pNewMini);
		return objectForType;
	}

	private void SpawnSparks(RotationAngle angle)
	{
		SpawnSparkWidthFactor(angle, 0.7f);
		SpawnSparkWidthFactor(new RotationAngle(angle.Radians - (float)System.Math.PI), 0.7f);
	}

	private void SpawnSparkWidthFactor(RotationAngle spriteRotation, float scale)
	{
		Vector3 position = base.transform.position;
		Vector3 vector = FLD.Math.AngleToVector3Z(spriteRotation.Radians);
		position += vector * 60f;
		Particle objectForType = ObjectPool.instance.GetObjectForType<Particle>();
		if (!(objectForType == null))
		{
			objectForType.Play("-sparks", position, scale, false, spriteRotation);
		}
	}

	public void SetChildedLight(bool b)
	{
		lightEntity.SetVisible(b);
	}

	public void SetSpeed(float s)
	{
		targetSpeed = s * maximumSpeed;
	}

	public void SetRotationSpeedAndSpikeScale(float r, float s)
	{
		rotationspeed = r * 10f;
		bladeScale = s;
	}

	public void AlertFriends()
	{
		OnHealthChanged();
		World.TriggerFlag(alertFriendsTriggerFlag);
	}

	private void OnAlertEvent()
	{
		isAlerted = true;
		timeSinceAlerted = 0f;
	}

	public void SetMaximumSpeed(float maxSpeed)
	{
		if (maxSpeed == 0f)
		{
			maxSpeed = 3.1f;
		}
		maximumSpeed = maxSpeed;
		SetParameter("maximumSpeed", maximumSpeed.ToString(), maximumSpeed.ToString());
	}

	private void SetDifficultyTo(float d)
	{
		maximumSpeed = originalMaximumSpeed * 0.3f * d + originalMaximumSpeed * 0.7f;
	}

	protected override void OnCollisionEnter(Collision pCollision)
	{
		if (mini == 0 || mini == 1 || mini == 4 || mini == 5)
		{
			base.OnCollisionEnter(pCollision);
		}
	}

	public override void ReceiveDamage(int damage, RotationAngle r)
	{
		if ((mini != 5 && mini != 2) || !(currFrame < 0.5f))
		{
			base.ReceiveDamage(damage, r);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (alertEvent != null)
		{
			alertEvent.Callback -= OnAlertEvent;
			alertEvent.Dispose();
			alertEvent = null;
		}
	}

	public override void RemoveFromWorld()
	{
		alertEvent.Callback -= OnAlertEvent;
		if (isCreatedFromLevel)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (bump != null)
		{
			ObjectPool.instance.PoolObject(_bump);
			_bump = null;
		}
		HideSprite(TopSprite);
		HideSprite(BottomSprite);
		ObjectPool.instance.PoolObject(this);
		base.gameObject.SetActive(false);
	}
}
