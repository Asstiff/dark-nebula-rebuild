using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Reuses Episode One's recovered BankGothic outlines for the matching Episode
// Two bitmap families. Bitmap assets and their serialized layout stay intact.
public static class RecoveredVectorFont
{
    // One shared, supersampled atlas avoids separate atlases and UV churn for
    // every label size. At 192 px/em the capitals are about 99 pixels high.
    const int RasterSize = 192;
    const float UnitsPerEm = 2048f;
    static Font vectorFont;
    static Material vectorMaterial;
    static bool prepared;
    static int generation;
    static readonly Dictionary<tk2dFontData, Profile> profiles = new Dictionary<tk2dFontData, Profile>();
    static readonly Dictionary<int, CharacterInfo> characters = new Dictionary<int, CharacterInfo>();
    static readonly HashSet<tk2dTextMesh> users = new HashSet<tk2dTextMesh>();
    static readonly List<tk2dTextMesh> renderUsers = new List<tk2dTextMesh>();
    static bool flushing;

    public sealed class Profile
    {
        public readonly float EmSize, AdvanceOffset, Baseline;
        public readonly Color Tint;
        public readonly tk2dFontData Bitmap;

        internal Profile(tk2dFontData bitmap, float unitScale, float advanceOffset, float baseline, Color tint)
        {
            Bitmap = bitmap;
            EmSize = unitScale * UnitsPerEm;
            AdvanceOffset = advanceOffset;
            Baseline = baseline;
            Tint = tint;
        }
    }

    public struct Glyph
    {
        public Vector3 p0, p1;
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
        public float advance;
    }

    public static int Generation { get { return generation; } }
    public static Material Material { get { return vectorMaterial; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRuntime()
    {
        Font.textureRebuilt -= OnTextureRebuilt;
        Camera.onPreCull -= BeforeCameraCull;
        if (vectorMaterial != null) Object.Destroy(vectorMaterial);
        vectorMaterial = null;
        vectorFont = null;
        prepared = false;
        generation++;
        profiles.Clear();
        characters.Clear();
        users.Clear();
        renderUsers.Clear();
        flushing = false;
    }

    public static Profile FindProfile(tk2dFontData bitmap)
    {
        if (bitmap == null || bitmap.textureGradients || bitmap.useDictionary) return null;
        Profile profile;
        if (profiles.TryGetValue(bitmap, out profile)) return profile;
        const float smallScale = 0.007044230012494074f;
        const float largeScale = 0.009472087525307501f;
        switch (bitmap.name)
        {
            case "FontSmalldata":
                profile = new Profile(bitmap, smallScale, 7.58855086f, 3.4f, new Color(1f, 1f, 1f, .4f));
                break;
            case "FontSmall_Dimmeddata":
                profile = new Profile(bitmap, smallScale, 7.58855086f, 3.4f, new Color(1f, 1f, 1f, 68f / 255f));
                break;
            case "FontSmall_Orangedata":
                profile = new Profile(bitmap, smallScale, 7.58855086f, 3.4f, new Color(238f / 255f, .6f, 0f, 1f));
                break;
            case "FontLargedata":
                profile = new Profile(bitmap, largeScale, 3.59522253f, 4.6f, new Color(1f, 1f, 1f, .4f));
                break;
            case "FontLarge_Orangedata":
                profile = new Profile(bitmap, largeScale, 3.19522253f, 4.6f, new Color(238f / 255f, .6f, 0f, 1f));
                break;
            default:
                // Tonad has a baked multicolor fill; other families have not
                // been matched to this OTF. Keep those original bitmaps.
                profile = null;
                break;
        }
        profiles.Add(bitmap, profile);
        return profile;
    }

    public static bool Prepare(Profile profile, string text)
    {
        if (profile == null) return false;
        if (vectorFont == null)
        {
            vectorFont = Resources.Load<Font>("Fonts/darknebula");
        }
        if (vectorFont == null) return false;

        // Unsupported scripts or characters keep the whole label on its
        // original bitmap, rather than mixing two atlases in one text mesh.
        foreach (char c in text)
        {
            if (c == '\n') continue;
            if (c >= profile.Bitmap.chars.Length || !vectorFont.HasCharacter(c) ||
                (c != ' ' && profile.Bitmap.chars[c].advance <= 0f)) return false;
        }
        if (vectorMaterial == null)
        {
            Shader shader = Resources.Load<Shader>("RecoveredVectorText");
            if (shader == null) return false;
            vectorMaterial = new Material(shader) { name = "Recovered BankGothic vector text" };
            Font.textureRebuilt += OnTextureRebuilt;
            Camera.onPreCull += BeforeCameraCull;
        }
        if (!prepared)
        {
            // Prewarm the complete recovered alphabet once. No per-frame text
            // requests, font instances, or per-label material instances are needed.
            var alphabet = new StringBuilder();
            for (int c = 32; c < 256; c++)
                if (vectorFont.HasCharacter((char)c)) alphabet.Append((char)c);
            vectorFont.RequestCharactersInTexture(alphabet.ToString(), RasterSize, FontStyle.Normal);
            characters.Clear();
            for (int c = 32; c < 256; c++)
            {
                CharacterInfo info;
                if (vectorFont.GetCharacterInfo((char)c, out info, RasterSize, FontStyle.Normal))
                    characters.Add(c, info);
            }
            vectorMaterial.mainTexture = vectorFont.material.mainTexture;
            prepared = true;
        }
        foreach (char c in text)
            if (c != '\n' && !characters.ContainsKey(c)) return false;
        return true;
    }

    public static bool TryGetGlyph(Profile profile, int character, out Glyph glyph)
    {
        CharacterInfo info;
        glyph = new Glyph();
        if (!characters.TryGetValue(character, out info)) return false;
        float scale = profile.EmSize / RasterSize;
        // tk2d stores the top-left and bottom-right corners in p0/p1.
        glyph.p0 = new Vector3(info.minX * scale, info.maxY * scale + profile.Baseline, 0f);
        glyph.p1 = new Vector3(info.maxX * scale, info.minY * scale + profile.Baseline, 0f);
        glyph.advance = info.advance * scale;
        glyph.topLeft = info.uvTopLeft;
        glyph.topRight = info.uvTopRight;
        glyph.bottomLeft = info.uvBottomLeft;
        glyph.bottomRight = info.uvBottomRight;
        return true;
    }

    public static float Advance(Profile profile, int current, int next, float spacing, float pairKerning, float characterScale, float textScale)
    {
        Glyph glyph;
        if (!TryGetGlyph(profile, current, out glyph)) return 0f;
        // The legacy bitmap advance includes tracking. Translate it once;
        // applying legacy -7 directly to a pure OTF advance would crush text.
        float advance = glyph.advance + profile.AdvanceOffset + spacing + pairKerning;
        Glyph following;
        bool narrow = IsNarrowLetter(current) || IsNarrowLetter(next);
        if (narrow && next >= 0 && char.IsLetterOrDigit((char)current) && char.IsLetterOrDigit((char)next)
            && TryGetGlyph(profile, next, out following))
        {
            float offsetScale = Mathf.Abs(textScale) > .00001f ? (1f - characterScale) / textScale : 0f;
            float right = glyph.p1.x * characterScale + offsetScale * (glyph.p1.x - glyph.p0.x) * .33f;
            float left = following.p0.x * characterScale + offsetScale * (following.p1.x - following.p0.x) * .33f;
            // Preserve tight tracking until it would actually overlap an I's
            // horizontal bars. The same advance is used to measure and draw.
            advance = Mathf.Max(advance, right - left + profile.EmSize * .01f);
        }
        return advance;
    }

    static bool IsNarrowLetter(int c)
    {
        return c == 'I' || c == 'i' || (c >= 0xCC && c <= 0xCF);
    }

    public static void Register(tk2dTextMesh text) { users.Add(text); }

    public static void Release(tk2dTextMesh text)
    {
        if (!users.Remove(text) || users.Count != 0) return;
        Font.textureRebuilt -= OnTextureRebuilt;
        Camera.onPreCull -= BeforeCameraCull;
        if (vectorMaterial != null) Object.Destroy(vectorMaterial);
        vectorMaterial = null;
        vectorFont = null; // Resources owns the font asset.
        characters.Clear();
        prepared = false;
    }

    static void OnTextureRebuilt(Font changed)
    {
        if (changed != vectorFont) return;
        prepared = false;
        generation++;
        if (vectorMaterial != null) vectorMaterial.mainTexture = changed.material.mainTexture;
        foreach (tk2dTextMesh text in users) if (text != null) text.InvalidateVectorFont();
    }

    static void BeforeCameraCull(Camera camera)
    {
        if (flushing) return;
        flushing = true;
        try
        {
            renderUsers.Clear();
            renderUsers.AddRange(users);
            foreach (tk2dTextMesh text in renderUsers)
                if (text != null && text.isActiveAndEnabled) text.RefreshVectorFont();
        }
        finally { flushing = false; }
    }
}
