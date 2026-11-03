using UnityEngine;

public class TileCollisionInfo
{
	public TypeCollisionInfoType type;

	public Vector2 pos;

	public float width;

	public float height;

	public TileCollisionInfo(TypeCollisionInfoType pType, Vector2 pPos, float pWidth, float pHeight)
	{
		type = pType;
		pos = pPos;
		width = pWidth;
		height = pHeight;
	}
}
