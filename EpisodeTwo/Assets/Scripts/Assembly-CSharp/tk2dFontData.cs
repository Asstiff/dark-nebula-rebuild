using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dFontData")]
public class tk2dFontData : MonoBehaviour
{
	public const int CURRENT_VERSION = 1;

	[HideInInspector]
	public int version;

	public float lineHeight;

	public tk2dFontChar[] chars;

	[SerializeField]
	private List<int> charDictKeys;

	[SerializeField]
	private List<tk2dFontChar> charDictValues;

	public Dictionary<int, tk2dFontChar> charDict;

	public bool useDictionary;

	public tk2dFontKerning[] kerning;

	public float largestWidth;

	public Material material;

	public Texture2D gradientTexture;

	public bool textureGradients;

	public int gradientCount = 1;

	[HideInInspector]
	public float invOrthoSize = 1f;

	[HideInInspector]
	public float halfTargetHeight = 1f;

	public void InitDictionary()
	{
		if (useDictionary && charDict == null)
		{
			charDict = new Dictionary<int, tk2dFontChar>(charDictKeys.Count);
			for (int i = 0; i < charDictKeys.Count; i++)
			{
				charDict[charDictKeys[i]] = charDictValues[i];
			}
		}
	}

	public void SetDictionary(Dictionary<int, tk2dFontChar> dict)
	{
		charDictKeys = new List<int>(dict.Keys);
		charDictValues = new List<tk2dFontChar>();
		for (int i = 0; i < charDictKeys.Count; i++)
		{
			charDictValues.Add(dict[charDictKeys[i]]);
		}
	}

    // Recovered atlases contain empty slots, including slot zero. They are not
    // renderable fallback glyphs and can lack the four gradient coordinates.
    public tk2dFontChar GetSafeCharacter(int code)
    {
        InitDictionary();
        tk2dFontChar glyph = FindCharacter(code);
        if (glyph == null && code >= 0 && code <= char.MaxValue)
            glyph = FindCharacter(char.ToUpperInvariant((char)code));
        if (glyph == null) glyph = FindCharacter('?');
        if (glyph == null) glyph = FindCharacter(' ');
        if (glyph == null) glyph = new tk2dFontChar { advance = Mathf.Max(1f, lineHeight * .5f) };
        if (textureGradients && (glyph.gradientUv == null || glyph.gradientUv.Length < 4))
        {
            float width = 1f / Mathf.Max(1, gradientCount);
            glyph.gradientUv = new[] { new Vector2(0, 1), new Vector2(width, 1), Vector2.zero, new Vector2(width, 0) };
        }
        return glyph;
    }

    tk2dFontChar FindCharacter(int code)
    {
        tk2dFontChar glyph = null;
        if (useDictionary)
        {
            if (charDict != null) charDict.TryGetValue(code, out glyph);
        }
        else if (chars != null && code >= 0 && code < chars.Length) glyph = chars[code];
        return glyph != null && (glyph.advance > 0 || glyph.p0 != glyph.p1) ? glyph : null;
    }
}
