using System.Collections.Generic;
using UnityEngine;

public class TileMap
{
	public const ushort Empty = ushort.MaxValue;

	public float ScrollSpeed;

	public ushort Width;

	public ushort Height;

	public float tileWidth = 64f;

	public float tileHeight = 64f;

	public bool render;

	public bool blend;

	public Vector2 origo;

	private ushort[,] map;

	public float alpha;

	private Dictionary<ushort, List<TileCollisionInfo>> tileCollisionInfoPerTile;

	public TileMap(ushort pWidth, ushort pHeight)
	{
		Width = pWidth;
		Height = pHeight;
		origo.x = Width * 64 / 2;
		origo.y = Height * 64;
		map = new ushort[Width, Height];
		for (ushort num = 0; num < Width; num++)
		{
			for (ushort num2 = 0; num2 < Height; num2++)
			{
				setTileAt(num, num2, ushort.MaxValue);
			}
		}
		alpha = -1f;
		tileCollisionInfoPerTile = new Dictionary<ushort, List<TileCollisionInfo>>();
	}

	public ushort GetTile(ushort x, ushort y)
	{
		return map[x, y];
	}

	public void defineScrollSpeed(float scrollSpeed)
	{
		ScrollSpeed = scrollSpeed;
	}

	public void setTileAt(ushort x, ushort y, ushort tileIndex)
	{
		map[x, y] = tileIndex;
	}

	public void addTileCollisionInfo(TileCollisionInfo collisionInfo, ushort tileIndex)
	{
		if (!tileCollisionInfoPerTile.ContainsKey(tileIndex))
		{
			tileCollisionInfoPerTile[tileIndex] = new List<TileCollisionInfo>();
		}
		tileCollisionInfoPerTile[tileIndex].Add(collisionInfo);
	}

	public List<TileCollisionInfo> collisionInfoForTile(ushort tileIndex)
	{
		if (!tileCollisionInfoPerTile.ContainsKey(tileIndex))
		{
			return null;
		}
		return tileCollisionInfoPerTile[tileIndex];
	}
}
