using FLD;
using UnityEngine;

public class IAPItem : ScriptableObject
{
	public string ID;

	public string Name;

	public string Description;

	public bool IsConsumable;

	public string LocalisedName
	{
		get
		{
			return Name.Localize();
		}
	}

	public string LocalisedDescription
	{
		get
		{
			return Description.Localize();
		}
	}

	public virtual void GiveItem()
	{
		Debug.Log("Implement this in child");
	}
}
