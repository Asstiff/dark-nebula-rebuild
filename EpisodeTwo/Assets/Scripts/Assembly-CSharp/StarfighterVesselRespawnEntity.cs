public class StarfighterVesselRespawnEntity : Entity
{
	private FlagCondition activateEvent;

	protected override void Awake()
	{
		base.Awake();
		base.alwaysAlive = true;
	}

	private void Start()
	{
		activateEvent = new FlagCondition(GetParameterInt("activateEventFlag"), GetParameterInt("activateEventCount"));
		activateEvent.Callback += OnActivateEvent;
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

	private void OnActivateEvent()
	{
		StarfighterVesselEntity starfighterVesselEntity = World.vessel as StarfighterVesselEntity;
		if (starfighterVesselEntity == null)
		{
			starfighterVesselEntity = ObjectPool.instance.GetObjectForType<StarfighterVesselEntity>();
			starfighterVesselEntity.PostInit();
			World.vessel = starfighterVesselEntity;
		}
		starfighterVesselEntity.IsLookingForShield = true;
		starfighterVesselEntity.transform.parent = base.transform.parent;
		starfighterVesselEntity.Spawn(base.transform.position);
	}

	public override void InitParametersInEditor()
	{
		base.InitParametersInEditor();
		ObjectPool.AddAsset<StarfighterVesselEntity>("Assets/Prefabs/Entities/StarfighterVesselEntity.prefab", 1);
		ObjectPool.AddAsset<StarfighterBulletEntity>("Assets/Prefabs/Entities/StarfighterBulletEntity.prefab", 14);
	}

	public override void InitParametersInRuntime()
	{
		base.InitParametersInRuntime();
	}
}
