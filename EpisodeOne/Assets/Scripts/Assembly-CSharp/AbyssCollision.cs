using AssemblyCSharp;
using UnityEngine;

public class AbyssCollision : MonoBehaviour, IFreezable
{
	private static bool _isInitialized;

	private static bool _hasRunAbyssFallCheckThisFrame;

	private Vector3 p = Vector3.zero;

	private Ray r = new Ray(Vector3.zero, Vector3.down);

	private float z;

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
		_hasRunAbyssFallCheckThisFrame = false;
	}

	private void Start()
	{
		z = base.transform.position.z;
		Initialize();
	}

	private void OnTriggerStay(Collider other)
	{
		if (World.IsFrozen(z))
		{
			base.GetComponent<Collider>().enabled = false;
			World.AddToFrozenObjects(this);
		}
		else
		{
			if (_hasRunAbyssFallCheckThisFrame)
			{
				return;
			}
			GameObject gameObject = other.gameObject;
			if (!gameObject.CompareTag("Player"))
			{
				return;
			}
			_hasRunAbyssFallCheckThisFrame = true;
			p = gameObject.transform.position;
			p.y = 100f;
			float num = 15f;
			if (!findEntityAt(p, "Abyss"))
			{
				return;
			}
			p.x += num;
			if (!findEntityAt(p, "Abyss"))
			{
				return;
			}
			p.x -= num * 2f;
			if (!findEntityAt(p, "Abyss"))
			{
				return;
			}
			p.x += num;
			p.z += num;
			if (findEntityAt(p, "Abyss"))
			{
				p.z -= num * 2f;
				if (findEntityAt(p, "Abyss"))
				{
					World.vessel.TriggerFall();
				}
			}
		}
	}

	private bool findEntityAt(Vector3 p, string tag)
	{
		r.origin = p;
		RaycastHit[] array = Physics.RaycastAll(r, 100f);
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

	public void Defrost()
	{
		base.GetComponent<Collider>().enabled = true;
	}
}
