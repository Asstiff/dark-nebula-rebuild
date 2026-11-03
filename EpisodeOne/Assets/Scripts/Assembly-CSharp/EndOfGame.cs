using FLD;
using UnityEngine;

public class EndOfGame : Entity
{
	public AudioClip Sfx;

	private bool _isActivated;

	private float _startTime;

	private int _soundCounter;

	private CameraFade _fade;

	private Vector3 _brokenMainframePos;

	private int lastCounterActedUpon = -1;

	private void Start()
	{
		Object obj = Object.FindObjectOfType(typeof(BrokenMainframeEntity));
		_brokenMainframePos = (obj as BrokenMainframeEntity).gameObject.transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			_startTime = Time.time;
			_isActivated = true;
			base.GetComponent<Collider>().enabled = false;
			World.vessel.AllowedToMove = false;
			World.vessel.TriggerEnergy(0);
			Object[] array = Object.FindObjectsOfType(typeof(MatrixChaser));
			foreach (Object obj in array)
			{
				(obj as MatrixChaser).gameObject.GetComponent<Collider>().enabled = false;
			}
			_fade = base.gameObject.AddComponent<CameraFade>();
			_fade.FadeOut(3.3333333f);
			Invoke("SetEndOfMatrix", 6.6666665f);
			Invoke("Kill", 8.333333f);
		}
	}

	private void Kill()
	{
		Object.Destroy(base.gameObject);
	}

	private void SetEndOfMatrix()
	{
		World.isEndOfMatrix = true;
		World.distortion = 0f;
		Vessel vessel = World.vessel;
		Vector3 position = new Vector3(158f, vessel.transform.position.y, _brokenMainframePos.z + 54f);
		World.cameraFollow.windowPosZ = position.z;
		vessel.transform.position = position;
		vessel.AllowedToMove = true;
		vessel.GetComponent<Rigidbody>().velocity = Vector3.zero;
		vessel.ResetDrag();
		Object[] array = Object.FindObjectsOfType(typeof(MatrixChaser));
		foreach (Object obj in array)
		{
			Object.Destroy((obj as MatrixChaser).gameObject);
		}
		MeshDistortion[] componentsInChildren = vessel.gameObject.GetComponentsInChildren<MeshDistortion>();
		foreach (MeshDistortion meshDistortion in componentsInChildren)
		{
			meshDistortion.Release();
		}
		vessel.UpdateVesselGfx();
		_fade.FadeIn(1.6666666f);
	}

	private void FixedUpdate()
	{
		if (!_isActivated)
		{
			return;
		}
		float num = (Time.time - _startTime) * 30f;
		if (!(num < 200f))
		{
			return;
		}
		Vector3 position = World.vessel.transform.position;
		position.x += (base.transform.position.x + 128f - position.x) * 0.2f * Time.deltaTime * 30f;
		position.z += (base.transform.position.z - position.z) * 0.08f * Time.deltaTime * 30f;
		World.vessel.transform.position = position;
		if (num < 100f)
		{
			float num2 = World.vessel.GetComponent<Rigidbody>().drag - 0.05f * Time.deltaTime * 30f;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			World.vessel.GetComponent<Rigidbody>().drag = num2;
		}
	}

	private void Update()
	{
		if (!_isActivated)
		{
			return;
		}
		int num = Mathf.FloorToInt((Time.time - _startTime) * 30f);
		if (lastCounterActedUpon != num)
		{
			lastCounterActedUpon = num;
			if (num > 100 && num < 200 && GetRandom())
			{
				PlayExplosionSfx();
			}
			if (num > 20 && num < 100 && GetRandom())
			{
				PlayExplosionSfx();
				Vector3 position = base.transform.position;
				position.x = 160f;
				position.x += Random.value * 100f - 50f;
				position.z += Random.value * 100f - 50f;
				ObjectPool.instance.GetObjectForType<Particle>().Play("-red", position, 2f);
			}
			if (num >= 202 && num <= 250 && GetRandom())
			{
				PlayExplosionSfx();
				Vector3 brokenMainframePos = _brokenMainframePos;
				brokenMainframePos.x = 160f;
				brokenMainframePos.x += Random.value * 200f - 100f;
				brokenMainframePos.z += Random.value * 200f - 100f;
				ObjectPool.instance.GetObjectForType<Particle>().Play(string.Empty, brokenMainframePos, 2f);
			}
		}
	}

	private bool GetRandom()
	{
		return Random.Range(0, 3) == 0;
	}

	private void PlayExplosionSfx()
	{
		if (_soundCounter % 2 == 0)
		{
			Audio.Play("explode" + _soundCounter, Sfx);
			_soundCounter %= 6;
		}
		_soundCounter++;
	}
}
