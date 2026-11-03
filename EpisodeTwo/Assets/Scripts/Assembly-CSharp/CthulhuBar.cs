using UnityEngine;

public class CthulhuBar : MonoBehaviour
{
	public tk2dSprite Bar;

	public tk2dSprite Joint;

	public CthulhuJoint cthulhuJoint;

	public bool doesDamage = true;

	public void Setup()
	{
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
		tk2dSprite[] array = componentsInChildren;
		foreach (tk2dSprite tk2dSprite2 in array)
		{
			if (tk2dSprite2.gameObject.name == "Bar")
			{
				Bar = tk2dSprite2;
				break;
			}
		}
		cthulhuJoint = Joint.GetComponent<CthulhuJoint>();
		Joint = cthulhuJoint.gameObject.GetComponent<tk2dSprite>();
	}

	private void OnTriggerEnter(Collider c)
	{
		if (doesDamage && c.CompareTag("Player"))
		{
			World.vessel.TriggerRecieveDamage();
		}
	}
}
