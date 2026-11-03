using System.Collections.Generic;

public class CustomEntity
{
	public byte basedOnEntityTypeId;

	public byte inheritedFromId;

	public List<string> PropertyNames;

	public List<string> PropertyNormal;

	public List<string> PropertyExpert;

	public CustomEntity(Dictionary<string, string> props, byte inheritedTypeId)
	{
		inheritedFromId = inheritedTypeId;
		PropertyNames = new List<string>();
		PropertyNormal = new List<string>();
		PropertyExpert = new List<string>();
		foreach (KeyValuePair<string, string> prop in props)
		{
			SetInheritedProperty(prop.Key, prop.Value, prop.Value);
		}
	}

	public void copyFromInherited(CustomEntity other)
	{
		for (int i = 0; i < other.PropertyNames.Count; i++)
		{
			SetInheritedProperty(other.PropertyNames[i], other.PropertyNormal[i], other.PropertyExpert[i]);
		}
	}

	private void SetInheritedProperty(string name, string normal, string expert)
	{
		if (PropertyNames.IndexOf(name) <= -1)
		{
			PropertyNames.Add(name);
			PropertyNormal.Add(normal);
			PropertyExpert.Add(expert);
		}
	}

	public void setBasedOnEntityTypeId(byte typeId)
	{
		basedOnEntityTypeId = typeId;
	}
}
