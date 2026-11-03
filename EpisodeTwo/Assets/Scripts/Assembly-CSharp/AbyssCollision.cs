using UnityEngine;

public class AbyssCollision : MonoBehaviour, IFreezable
{
	private static bool _isInitialized = false;

	private static bool _hasRunAbyssFallCheckThisStep = false;

	public float LayerHeight;

	private static Vector3 p = Vector3.zero;

	private static Ray r = new Ray(Vector3.zero, Vector3.down);

	private float z;

	private static int AbyssCountInCollision;

	private static bool LastColliderWasWater;

	private static int layermask = 32768;

	private static void Initialize()
	{
		if (!_isInitialized)
		{
			_isInitialized = true;
			World.SignalUpdate += onClearFrame;
		}
	}

	private static void onClearFrame()
	{
		_hasRunAbyssFallCheckThisStep = false;
	}

	private void Start()
	{
		z = base.transform.position.z;
		Initialize();
	}

	private void OnTriggerStay(Collider other)
	{
		if (Time.frameCount % 3 == 0)
		{
			return;
		}
		if (World.IsFrozen(z))
		{
			base.enabled = false;
			base.GetComponent<Collider>().enabled = false;
			World.FreezeManager.AddToFrozenObjects(this);
		}
		else
		{
			if (_hasRunAbyssFallCheckThisStep)
			{
				return;
			}
			GameObject gameObject = other.gameObject;
			if (!gameObject.CompareTag("Player"))
			{
				return;
			}
			_hasRunAbyssFallCheckThisStep = true;
			StandardVessel standardVessel = World.vessel as StandardVessel;
			if (standardVessel == null || standardVessel.IsOnFlipFloor > 0 || standardVessel.IsInAir)
			{
				return;
			}
			p = standardVessel.transform.position;
			p.y += 256f;
			float num = 15f;
			if (!findEntityAtTag(p, "Abyss"))
			{
				return;
			}
			p.x += num;
			if (!findEntityAtTag(p, "Abyss"))
			{
				return;
			}
			p.x -= num * 2f;
			if (!findEntityAtTag(p, "Abyss"))
			{
				return;
			}
			p.x += num;
			p.z += num;
			if (!findEntityAtTag(p, "Abyss"))
			{
				return;
			}
			p.z -= num * 2f;
			if (findEntityAtTag(p, "Abyss"))
			{
                if (standardVessel.FindPlatformSupport()) return;
				standardVessel.AbyssPositionZ = (float)(standardVessel.Layer - 1) * 5f - 5f;
				if (standardVessel.AbyssPositionZ < LayerHeight - 5f)
				{
					standardVessel.AbyssPositionZ = LayerHeight - 5f;
				}
				TriggerFall();
			}
		}
	}

	public virtual void TriggerFall()
	{
		World.vessel.TriggerFall(false);
	}

	private bool findEntityAtTag(Vector3 p, string tag)
	{
		r.origin = p;
		RaycastHit[] array = Physics.RaycastAll(r, 256f, 1);
		RaycastHit[] array2 = array;
		foreach (RaycastHit raycastHit in array2)
		{
			if (raycastHit.collider.CompareTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	private static bool findEntityAt(Vector3 p)
	{
		r.origin = p;
		return Physics.Raycast(r, 256f, layermask);
	}

	public void Defrost()
	{
		base.GetComponent<Collider>().enabled = true;
		base.enabled = true;
	}
}
