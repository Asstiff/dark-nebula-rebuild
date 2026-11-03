using FLD;
using UnityEngine;

public class MagnetVesselSlotEntity : Entity
{
	private float caughtDistance = 4f;

	public int flag;

	public AudioClip endLevelVesselStopSound;

	private bool catched;

	private bool vesselCloseby;

	protected override void Awake()
	{
		base.Awake();
		catched = false;
	}

	public void InitWithWorldEndSequence(Vector3 p)
	{
		flag = 97;
		base.transform.position = p;
		caughtDistance = 1f;
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

	protected void InitParameters()
	{
		flag = GetParameterInt("flag");
	}

	private void FixedUpdate()
	{
		if (!catched && vesselCloseby)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = World.vessel.transform.position;
			position2.x += (position.x - position2.x) * 0.2f * Time.deltaTime * 30f;
			position2.z += (position.z - position2.z) * 0.08f * Time.deltaTime * 30f;
			World.vessel.GetComponent<Rigidbody>().MovePosition(position2);
			World.vessel.DisableMovement();
			if (!catched && Math.Distance2d(World.vessel.transform.position, position) < caughtDistance)
			{
				OnCatched();
			}
		}
	}

	private void OnCatched()
	{
		catched = true;
		World.vessel.GetComponent<Rigidbody>().MovePosition(base.transform.position);
		Audio.Play("endLevelVesselStopSound", endLevelVesselStopSound);
		World.TriggerFlag(flag);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (!catched && collider.CompareTag("Player"))
		{
			vesselCloseby = true;
			World.vessel.DisableMovement();
		}
	}
}
