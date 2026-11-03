using System;
using UnityEngine;

[Serializable]
public class tk2dSpriteCollectionFont
{
	public bool active;

	public UnityEngine.Object bmFont;

	public Texture2D texture;

	public bool dupeCaps;

	public bool flipTextureY;

	public int charPadX;

	public tk2dFontData data;

	public tk2dFont editorData;

	public string Name
	{
		get
		{
			if (bmFont == null || texture == null)
			{
				return "Empty";
			}
			if (data == null)
			{
				return bmFont.name + " (Inactive)";
			}
			return bmFont.name;
		}
	}

	public bool InUse
	{
		get
		{
			return active && bmFont != null && texture != null && data != null && editorData != null;
		}
	}

	public void CopyFrom(tk2dSpriteCollectionFont src)
	{
		active = src.active;
		bmFont = src.bmFont;
		texture = src.texture;
		dupeCaps = src.dupeCaps;
		flipTextureY = src.flipTextureY;
		charPadX = src.charPadX;
		data = src.data;
		editorData = src.editorData;
	}
}
