using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteCollection")]
public class tk2dSpriteCollection : MonoBehaviour
{
	public enum NormalGenerationMode
	{
		None = 0,
		NormalsOnly = 1,
		NormalsAndTangents = 2
	}

	public enum TextureCompression
	{
		Uncompressed = 0,
		Reduced16Bit = 1,
		Compressed = 2,
		Dithered16Bit_4444 = 3,
		Dithered16Bit_565 = 4
	}

	[HideInInspector]
	public tk2dSpriteCollectionDefinition[] textures;

	public Texture2D[] textureRefs;

	public tk2dSpriteSheetSource[] spriteSheets;

	public tk2dSpriteCollectionFont[] fonts;

	public tk2dSpriteCollectionDefault defaults;

	[HideInInspector]
	public int maxTextureSize = 1024;

	public bool forceTextureSize;

	public int forcedTextureWidth = 1024;

	public int forcedTextureHeight = 1024;

	[HideInInspector]
	public TextureCompression textureCompression;

	[HideInInspector]
	public int atlasWidth;

	[HideInInspector]
	public int atlasHeight;

	[HideInInspector]
	public bool forceSquareAtlas;

	[HideInInspector]
	public float atlasWastage;

	[HideInInspector]
	public bool allowMultipleAtlases;

	[HideInInspector]
	public tk2dSpriteCollectionDefinition[] textureParams;

	public tk2dSpriteCollectionData spriteCollection;

	public bool premultipliedAlpha;

	public Material[] altMaterials;

	public Material[] atlasMaterials;

	public Texture2D[] atlasTextures;

	[HideInInspector]
	public bool useTk2dCamera;

	[HideInInspector]
	public int targetHeight = 640;

	[HideInInspector]
	public float targetOrthoSize = 1f;

	public bool pixelPerfectPointSampled;

	public float physicsDepth = 0.1f;

	public bool disableTrimming;

	public NormalGenerationMode normalGenerationMode;

	[HideInInspector]
	public int padAmount = -1;

	[HideInInspector]
	public bool autoUpdate = true;

	public float editorDisplayScale = 1f;
}
