using System;
using UnityEngine;

[Serializable]
public class tk2dBatchedSprite
{
	public string name = string.Empty;

	public int parentId = -1;

	public int spriteId;

	public Quaternion rotation = Quaternion.identity;

	public Vector3 position = Vector3.zero;

	public Vector3 localScale = Vector3.one;

	public Color color = Color.white;

	public bool alwaysPixelPerfect;

	public bool IsDrawn
	{
		get
		{
			return spriteId != -1;
		}
	}

	public tk2dBatchedSprite()
	{
		parentId = -1;
	}
}
