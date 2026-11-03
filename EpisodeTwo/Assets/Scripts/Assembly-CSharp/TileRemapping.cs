using System.Collections.Generic;

public class TileRemapping
{
	public string Name;

	public string ReplacedName;

	public Dictionary<ushort, ushort> Map = new Dictionary<ushort, ushort>();

	public int GetTile(ushort source)
	{
		ushort value = source;
		if (Map.TryGetValue(source, out value))
		{
			return value;
		}
		return -1;
	}
}
