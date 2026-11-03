using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dFont")]
public class tk2dFont : MonoBehaviour
{
	public Object bmFont;

	public Material material;

	public Texture texture;

	public Texture2D gradientTexture;

	public bool dupeCaps;

	public bool flipTextureY;

	[HideInInspector]
	public bool proxyFont;

	[HideInInspector]
	public bool useTk2dCamera;

	[HideInInspector]
	public int targetHeight = 640;

	[HideInInspector]
	public float targetOrthoSize = 1f;

	public int gradientCount = 1;

	public bool manageMaterial;

	public int charPadX;

	public tk2dFontData data;
}
