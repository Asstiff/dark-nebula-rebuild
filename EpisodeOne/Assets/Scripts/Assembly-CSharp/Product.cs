using FLD;
using UnityEngine;

public class Product : ScriptableObject
{
	public string Name;

	public string Description;

	public int Price;

	public string IconID;

	public tk2dSpriteCollectionData SpriteCollection;

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

	public virtual string ID
	{
		get
		{
			return Name;
		}
	}
}
