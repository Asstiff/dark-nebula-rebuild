using UnityEngine;

public class VesselDetection : MonoBehaviour
{
	public bool AwareOfVessel;

	private float squaredRange;

	private float timeSinceLastDetection;

	private Transform _transform;

	private void Awake()
	{
		AwareOfVessel = false;
		squaredRange = 57600f;
		_transform = base.transform;
	}

	private void Update()
	{
		if (World.vessel == null)
		{
			return;
		}
		if ((_transform.position - World.vessel.transform.position).sqrMagnitude <= squaredRange)
		{
			timeSinceLastDetection = 0f;
			AwareOfVessel = true;
			return;
		}
		timeSinceLastDetection += Time.deltaTime;
		if (timeSinceLastDetection > 5f)
		{
			AwareOfVessel = false;
		}
	}
}
