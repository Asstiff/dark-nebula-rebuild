using FLD;
using UnityEngine;

public class OutroShuttle : Entity
{
	private static string ANIMATION_NAME = "animation";

	public GameObject GfxShuttle;

	public GameObject GfxShuttleLeft;

	public GameObject GfxShuttleRight;

	public AudioClip SfxClaws;

	public AudioClip SfxFlyAway;

	public AudioClip SfxVesselStop;

	private float catchCounter = -1000f;

	private float clamps;

	private bool _hasPlayedEndLevelFlyAway;

	private bool _hasPlayedEndLevelClaws;

	private Transform _transform;

	private Transform _vesselTransform;

	private bool _hasTouchedShuttle;

	private void OnEnable()
	{
		_transform = base.transform;
	}

	private void OnDisable()
	{
		GfxShuttle = null;
		GfxShuttleLeft = null;
		GfxShuttleRight = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (_vesselTransform == null)
			{
				_vesselTransform = World.vessel.transform;
				World.vessel.GetComponent<Rigidbody>().isKinematic = true;
			}
			_hasTouchedShuttle = true;
		}
	}

	private void MoveVesselTowardsShuttle()
	{
		Vector3 position = _vesselTransform.position;
		position += new Vector3((_transform.position.x - position.x) * 0.2f, 0f, (_transform.position.z - position.z) * 0.08f) * Time.deltaTime * 30f;
		_vesselTransform.position = position;
		if (position.z > _transform.position.z)
		{
			_vesselTransform.position = new Vector3(position.x, 0f, _transform.position.z);
		}
		if ((_vesselTransform.position - _transform.position).magnitude < 5f && catchCounter == -1000f)
		{
			catchCounter = 0f;
			Audio.Play("vesselstop", SfxVesselStop);
		}
	}

	private void Update()
	{
		if (Time.deltaTime == 0f || !_hasTouchedShuttle)
		{
			return;
		}
		if (catchCounter == -1000f)
		{
			MoveVesselTowardsShuttle();
			return;
		}
		if (catchCounter >= 4f && !_hasPlayedEndLevelClaws)
		{
			_hasPlayedEndLevelClaws = true;
			Audio.Play("claws", SfxClaws);
			_vesselTransform.parent = _transform;
			_vesselTransform.localPosition = new Vector3(0f, 0f, 0f);
		}
		if (catchCounter >= 0f)
		{
			clamps += 5f * Time.deltaTime * 30f;
			if (clamps >= 97f)
			{
				clamps = 97f;
			}
		}
		if (catchCounter > 40f && !_hasPlayedEndLevelFlyAway)
		{
			_hasPlayedEndLevelFlyAway = true;
			Audio.Play("flyaway", SfxFlyAway);
			base.GetComponent<Animation>().Play(ANIMATION_NAME);
		}
		catchCounter += Time.deltaTime * 30f;
		GfxShuttleLeft.transform.localPosition = new Vector3(-168f + clamps, 64f, 0f);
		GfxShuttleRight.transform.localPosition = new Vector3(100f - clamps, 64f, 0f);
	}
}
