using System;
using UnityEngine;

[Serializable]
public class tk2dSpriteCollectionDefinition
{
	public enum Anchor
	{
		UpperLeft = 0,
		UpperCenter = 1,
		UpperRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		LowerLeft = 6,
		LowerCenter = 7,
		LowerRight = 8,
		Custom = 9
	}

	public enum Pad
	{
		Default = 0,
		BlackZeroAlpha = 1,
		Extend = 2
	}

	public enum ColliderType
	{
		Unset = 0,
		None = 1,
		BoxTrimmed = 2,
		BoxCustom = 3,
		Polygon = 4
	}

	public enum PolygonColliderCap
	{
		None = 0,
		FrontAndBack = 1,
		Front = 2,
		Back = 3
	}

	public enum ColliderColor
	{
		Default = 0,
		Red = 1,
		White = 2,
		Black = 3
	}

	public enum Source
	{
		Sprite = 0,
		SpriteSheet = 1,
		Font = 2
	}

	public string name = string.Empty;

	public bool disableTrimming;

	public bool additive;

	public Vector3 scale = new Vector3(1f, 1f, 1f);

	[HideInInspector]
	public Texture2D texture;

	[NonSerialized]
	[HideInInspector]
	public Texture2D thumbnailTexture;

	public int materialId;

	public Anchor anchor = Anchor.MiddleCenter;

	public float anchorX;

	public float anchorY;

	public UnityEngine.Object overrideMesh;

	public bool customSpriteGeometry;

	public tk2dSpriteColliderIsland[] geometryIslands = new tk2dSpriteColliderIsland[0];

	public bool dice;

	public int diceUnitX = 64;

	public int diceUnitY;

	public Pad pad;

	public int extraPadding;

	public Source source;

	public bool fromSpriteSheet;

	public bool hasSpriteSheetId;

	public int spriteSheetId;

	public int spriteSheetX;

	public int spriteSheetY;

	public bool extractRegion;

	public int regionX;

	public int regionY;

	public int regionW;

	public int regionH;

	public int regionId;

	public ColliderType colliderType;

	public Vector2 boxColliderMin;

	public Vector2 boxColliderMax;

	public tk2dSpriteColliderIsland[] polyColliderIslands;

	public PolygonColliderCap polyColliderCap;

	public bool colliderConvex;

	public bool colliderSmoothSphereCollisions;

	public ColliderColor colliderColor;

	public void CopyFrom(tk2dSpriteCollectionDefinition src)
	{
		name = src.name;
		disableTrimming = src.disableTrimming;
		additive = src.additive;
		scale = src.scale;
		texture = src.texture;
		materialId = src.materialId;
		anchor = src.anchor;
		anchorX = src.anchorX;
		anchorY = src.anchorY;
		overrideMesh = src.overrideMesh;
		customSpriteGeometry = src.customSpriteGeometry;
		geometryIslands = src.geometryIslands;
		dice = src.dice;
		diceUnitX = src.diceUnitX;
		diceUnitY = src.diceUnitY;
		pad = src.pad;
		source = src.source;
		fromSpriteSheet = src.fromSpriteSheet;
		hasSpriteSheetId = src.hasSpriteSheetId;
		spriteSheetX = src.spriteSheetX;
		spriteSheetY = src.spriteSheetY;
		spriteSheetId = src.spriteSheetId;
		extractRegion = src.extractRegion;
		regionX = src.regionX;
		regionY = src.regionY;
		regionW = src.regionW;
		regionH = src.regionH;
		regionId = src.regionId;
		colliderType = src.colliderType;
		boxColliderMin = src.boxColliderMin;
		boxColliderMax = src.boxColliderMax;
		polyColliderCap = src.polyColliderCap;
		colliderColor = src.colliderColor;
		colliderConvex = src.colliderConvex;
		colliderSmoothSphereCollisions = src.colliderSmoothSphereCollisions;
		extraPadding = src.extraPadding;
		if (src.polyColliderIslands != null)
		{
			polyColliderIslands = new tk2dSpriteColliderIsland[src.polyColliderIslands.Length];
			for (int i = 0; i < polyColliderIslands.Length; i++)
			{
				polyColliderIslands[i] = new tk2dSpriteColliderIsland();
				polyColliderIslands[i].CopyFrom(src.polyColliderIslands[i]);
			}
		}
		else
		{
			polyColliderIslands = new tk2dSpriteColliderIsland[0];
		}
		if (src.geometryIslands != null)
		{
			geometryIslands = new tk2dSpriteColliderIsland[src.geometryIslands.Length];
			for (int j = 0; j < geometryIslands.Length; j++)
			{
				geometryIslands[j] = new tk2dSpriteColliderIsland();
				geometryIslands[j].CopyFrom(src.geometryIslands[j]);
			}
		}
		else
		{
			geometryIslands = new tk2dSpriteColliderIsland[0];
		}
	}

	public void Clear()
	{
		tk2dSpriteCollectionDefinition src = new tk2dSpriteCollectionDefinition();
		CopyFrom(src);
	}

	public bool CompareTo(tk2dSpriteCollectionDefinition src)
	{
		if (name != src.name)
		{
			return false;
		}
		if (additive != src.additive)
		{
			return false;
		}
		if (scale != src.scale)
		{
			return false;
		}
		if (texture != src.texture)
		{
			return false;
		}
		if (materialId != src.materialId)
		{
			return false;
		}
		if (anchor != src.anchor)
		{
			return false;
		}
		if (anchorX != src.anchorX)
		{
			return false;
		}
		if (anchorY != src.anchorY)
		{
			return false;
		}
		if (overrideMesh != src.overrideMesh)
		{
			return false;
		}
		if (dice != src.dice)
		{
			return false;
		}
		if (diceUnitX != src.diceUnitX)
		{
			return false;
		}
		if (diceUnitY != src.diceUnitY)
		{
			return false;
		}
		if (pad != src.pad)
		{
			return false;
		}
		if (extraPadding != src.extraPadding)
		{
			return false;
		}
		if (customSpriteGeometry != src.customSpriteGeometry)
		{
			return false;
		}
		if (geometryIslands != src.geometryIslands)
		{
			return false;
		}
		if (geometryIslands != null && src.geometryIslands != null)
		{
			if (geometryIslands.Length != src.geometryIslands.Length)
			{
				return false;
			}
			for (int i = 0; i < geometryIslands.Length; i++)
			{
				if (!geometryIslands[i].CompareTo(src.geometryIslands[i]))
				{
					return false;
				}
			}
		}
		if (source != src.source)
		{
			return false;
		}
		if (fromSpriteSheet != src.fromSpriteSheet)
		{
			return false;
		}
		if (hasSpriteSheetId != src.hasSpriteSheetId)
		{
			return false;
		}
		if (spriteSheetId != src.spriteSheetId)
		{
			return false;
		}
		if (spriteSheetX != src.spriteSheetX)
		{
			return false;
		}
		if (spriteSheetY != src.spriteSheetY)
		{
			return false;
		}
		if (extractRegion != src.extractRegion)
		{
			return false;
		}
		if (regionX != src.regionX)
		{
			return false;
		}
		if (regionY != src.regionY)
		{
			return false;
		}
		if (regionW != src.regionW)
		{
			return false;
		}
		if (regionH != src.regionH)
		{
			return false;
		}
		if (regionId != src.regionId)
		{
			return false;
		}
		if (colliderType != src.colliderType)
		{
			return false;
		}
		if (boxColliderMin != src.boxColliderMin)
		{
			return false;
		}
		if (boxColliderMax != src.boxColliderMax)
		{
			return false;
		}
		if (polyColliderIslands != src.polyColliderIslands)
		{
			return false;
		}
		if (polyColliderIslands != null && src.polyColliderIslands != null)
		{
			if (polyColliderIslands.Length != src.polyColliderIslands.Length)
			{
				return false;
			}
			for (int j = 0; j < polyColliderIslands.Length; j++)
			{
				if (!polyColliderIslands[j].CompareTo(src.polyColliderIslands[j]))
				{
					return false;
				}
			}
		}
		if (polyColliderCap != src.polyColliderCap)
		{
			return false;
		}
		if (colliderColor != src.colliderColor)
		{
			return false;
		}
		if (colliderSmoothSphereCollisions != src.colliderSmoothSphereCollisions)
		{
			return false;
		}
		if (colliderConvex != src.colliderConvex)
		{
			return false;
		}
		return true;
	}
}
