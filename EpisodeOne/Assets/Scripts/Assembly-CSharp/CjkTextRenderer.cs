using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Keeps original layout/color. Translations use fixed UVs; literal names use the full TTF.
[DisallowMultipleComponent]
public sealed class CjkTextRenderer : MonoBehaviour
{
    static readonly HashSet<CjkTextRenderer> literalUsers = new HashSet<CjkTextRenderer>();
    static readonly List<CjkTextRenderer> renderUsers = new List<CjkTextRenderer>();
    static readonly StringBuilder requestedText = new StringBuilder();
    static bool flushing;
    static bool literalRequestsDirty = true;
    static string preparedLiteralText = string.Empty;
    static int preparedLiteralGeneration = -1;
    tk2dTextMesh label;
    MeshFilter meshFilter;
    Renderer meshRenderer;
    Mesh translatedMesh, originalMesh;
    Material originalMaterial;
    Material translatedMaterial;
    tk2dFontData materialFont;
    Texture2D materialGradient;
    int materialGradientIndex, materialGradientCount;
    bool materialLiteral, lastIsLiteral;
    string lastText;
    Color lastColor, lastColor2;
    Vector3 lastScale, lastCharScale;
    TextAnchor lastAnchor;
    tk2dFontData lastFont;
    float lastSpacing, lastLineSpacing;
    bool active, dirty = true, usesLiteralFont;
    int glyphGeneration = -1;
    public bool UsesLiteralFont { get { return active && usesLiteralFont; } }
    public int GlyphGeneration { get { return glyphGeneration; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRuntime()
    {
        Camera.onPreCull -= BeforeCameraCull;
        Camera.onPreCull += BeforeCameraCull;
        literalUsers.Clear(); renderUsers.Clear(); requestedText.Length = 0; flushing = false;
        literalRequestsDirty = true; preparedLiteralText = string.Empty; preparedLiteralGeneration = -1;
    }
    public static void Restore(tk2dTextMesh source)
    {
        CjkTextRenderer renderer = source.GetComponent<CjkTextRenderer>();
        if (renderer == null) return;
        if (literalUsers.Remove(renderer)) literalRequestsDirty = true;
        if (!renderer.active) return;
        source.GetComponent<MeshFilter>().sharedMesh = renderer.originalMesh;
        source.GetComponent<Renderer>().sharedMaterial = renderer.originalMaterial;
        renderer.active = false;
    }
    public static void Render(tk2dTextMesh source)
    {
        bool needsFullFont = GameLocalization.ContainsChinese(source.text);
        // User names also accept punctuation, compatibility characters and
        // extended Latin outside the recovered bitmap alphabet.
        if (!needsFullFont && source.IsLiteralText)
            foreach (char c in source.text)
                if (c > 0x7e) { needsFullFont = true; break; }
        if (!needsFullFont) { Restore(source); return; }
        CjkTextRenderer renderer = source.GetComponent<CjkTextRenderer>();
        if (renderer == null) renderer = source.gameObject.AddComponent<CjkTextRenderer>();
        renderer.label = source;
        if (renderer.meshFilter == null) renderer.meshFilter = source.GetComponent<MeshFilter>();
        if (renderer.meshRenderer == null) renderer.meshRenderer = source.GetComponent<Renderer>();
        renderer.Apply();
    }
    void OnEnable() { dirty = true; }
    void OnDisable() { if (label != null) Restore(label); else if (literalUsers.Remove(this)) literalRequestsDirty = true; }
    void LateUpdate()
    {
        if (label == null || !label.isActiveAndEnabled) return;
        if (lastText != label.text || lastIsLiteral != label.IsLiteralText) Render(label);
        else Apply();
    }
    void OnDestroy() { if (literalUsers.Remove(this)) literalRequestsDirty = true; if (translatedMesh != null) Destroy(translatedMesh); }
    static void PrepareLiteralUsers()
    {
        if (!literalRequestsDirty && preparedLiteralGeneration == CjkFontAtlas.LiteralGeneration && CjkFontAtlas.LiteralReady) return;
        if (literalRequestsDirty)
        {
            requestedText.Length = 0;
            foreach (CjkTextRenderer renderer in literalUsers)
                if (renderer != null && renderer.isActiveAndEnabled && renderer.label != null && renderer.label.isActiveAndEnabled && renderer.label.IsLiteralText)
                    requestedText.Append(renderer.label.text);
            preparedLiteralText = requestedText.ToString();
        }
        CjkFontAtlas.PrepareLiteral(preparedLiteralText);
        preparedLiteralGeneration = CjkFontAtlas.LiteralGeneration;
        literalRequestsDirty = false;
    }
    static void BeforeCameraCull(Camera camera)
    {
        if (flushing || literalUsers.Count == 0) return;
        flushing = true;
        try
        {
            // Finish all requests before any name reads the current glyph UVs.
            PrepareLiteralUsers();
            renderUsers.Clear(); renderUsers.AddRange(literalUsers);
            foreach (CjkTextRenderer renderer in renderUsers)
                if (renderer != null && renderer.isActiveAndEnabled && renderer.label != null && renderer.label.isActiveAndEnabled)
                    renderer.Apply(false);
        }
        finally { flushing = false; }
    }
    void Apply(bool prepareLiteral = true)
    {
        if (label.font == null) return;
        if (label.IsLiteralText)
        {
            if (literalUsers.Add(this) || lastText != label.text) literalRequestsDirty = true;
        }
        else if (literalUsers.Remove(this)) literalRequestsDirty = true;
        if (label.IsLiteralText && prepareLiteral) PrepareLiteralUsers();
        bool literal = label.IsLiteralText && CjkFontAtlas.LiteralReady;
        int generation = literal ? CjkFontAtlas.LiteralGeneration : -1;
        if (!active) { originalMesh = meshFilter.sharedMesh; originalMaterial = meshRenderer.sharedMaterial; }
        Vector3 characterScale = Vector3.one;
        float spacing = label.Spacing, lineSpacing = label.LineSpacing;
        Color top = label.color, bottom = label.useGradient ? label.color2 : top;
        // Preserve the original vertex tint; Unity converts the sRGB ramp texture.
        bool rebuilt = dirty || lastText != label.text || lastFont != label.font || lastAnchor != label.anchor || lastColor != top || lastColor2 != bottom || lastScale != label.scale || lastCharScale != characterScale || lastSpacing != spacing || lastLineSpacing != lineSpacing || usesLiteralFont != literal || glyphGeneration != generation;
        if (rebuilt)
        {
            usesLiteralFont = literal;
            Build(top, bottom, characterScale, spacing, lineSpacing);
            lastText = label.text; lastFont = label.font; lastAnchor = label.anchor;
            lastColor = top; lastColor2 = bottom; lastScale = label.scale; lastCharScale = characterScale;
            lastSpacing = spacing; lastLineSpacing = lineSpacing; glyphGeneration = generation; dirty = false;
        }
        Texture2D gradient = label.font.textureGradients ? label.font.gradientTexture : null;
        int gradientIndex = label.textureGradient, gradientCount = label.font.gradientCount;
        bool materialChanged = translatedMaterial == null || materialFont != label.font || materialGradient != gradient || materialGradientIndex != gradientIndex || materialGradientCount != gradientCount || materialLiteral != literal;
        if (materialChanged)
        {
            translatedMaterial = CjkFontAtlas.MaterialFor(label.font.name, gradient, gradientIndex, gradientCount, literal);
            materialFont = label.font; materialGradient = gradient; materialGradientIndex = gradientIndex;
            materialGradientCount = gradientCount; materialLiteral = literal;
        }
        // Reassigning unchanged shared meshes/materials every frame makes Unity
        // repeat renderer work even when a translated menu label is static.
        if (!active || rebuilt) meshFilter.sharedMesh = translatedMesh;
        if (!active || materialChanged) meshRenderer.sharedMaterial = translatedMaterial;
        lastIsLiteral = label.IsLiteralText;
        active = true;
    }
    CjkFontAtlas.Glyph Glyph(char c) { return usesLiteralFont ? CjkFontAtlas.GetLiteralGlyph(c) : CjkFontAtlas.GetGlyph(c); }
    void Build(Color top, Color bottom, Vector3 characterScale, float spacing, float lineSpacing)
    {
        if (translatedMesh == null) translatedMesh = new Mesh() { name = "Chinese label", hideFlags = HideFlags.DontSave };
        tk2dFontChar reference = null;
        if (label.font.chars != null && label.font.chars.Length > 65) reference = label.font.chars[65];
        float referenceBottom = reference != null ? Mathf.Min(reference.p0.y, reference.p1.y) : label.font.lineHeight * .2f;
        float referenceHeight = reference != null ? Mathf.Abs(reference.p1.y - reference.p0.y) : label.font.lineHeight * .6f;

        if (referenceHeight < .001f) referenceHeight = label.font.lineHeight * .6f;
        float capHeight = referenceHeight * GameLocalization.ChineseTextScale;
        float unit = capHeight / (CjkFontAtlas.CapTop - CjkFontAtlas.CapBottom);
        float referenceCenter = (referenceBottom + referenceHeight * .5f) * characterScale.y
            - (1f - characterScale.y) * referenceHeight * .1f;
        float cellHeight = label.font.lineHeight + lineSpacing;
        float lineHeight = label.text.IndexOf('\n') >= 0 ? Mathf.Max(cellHeight, capHeight * 1.25f) : cellHeight;
        // Preserve fullwidth cells without bitmap tracking or character compression.
        string[] lines = label.text.Replace("\r", "").Split('\n');
        int vertical = (int)label.anchor / 3, horizontal = (int)label.anchor % 3;
        float blockHeight = cellHeight + (lines.Length - 1) * lineHeight;
        float firstBaseline = vertical == 0 ? -cellHeight : vertical == 1 ? blockHeight * .5f - cellHeight : blockHeight - cellHeight;
        if (vertical == 1 && label.font.version >= 2 && label.font.texelSize.y > 0f) firstBaseline = Mathf.Floor(firstBaseline / label.font.texelSize.y) * label.font.texelSize.y;
        var vertices = new List<Vector3>(); var uv = new List<Vector2>(); var ramp = new List<Vector2>(); var colors = new List<Color>(); var triangles = new List<int>();
        for (int row = 0; row < lines.Length; row++)
        {
            string line = lines[row]; float width = 0, minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
            foreach (char c in line)
            {
                CjkFontAtlas.Glyph glyph = Glyph(c);
                width += glyph.advance * unit;
                if (glyph.y1 > glyph.y0 && !char.IsWhiteSpace(c)) { minY = Mathf.Min(minY, glyph.y0); maxY = Mathf.Max(maxY, glyph.y1); }
            }
            // Center the actual line's glyph bounds on the original Latin cap
            // center, rather than assuming every translated glyph fills the atlas cap box.
            float center = float.IsInfinity(minY) ? 0 : (minY + maxY) * .5f * unit * characterScale.y;
            float baseline = firstBaseline - row * lineHeight + referenceCenter - center;
            float x = horizontal == 0 ? 0 : horizontal == 1 ? -width * .5f : -width;
            foreach (char c in line)
            {
                CjkFontAtlas.Glyph g = Glyph(c);
                if (g.x1 > g.x0 && g.y1 > g.y0 && !char.IsWhiteSpace(c))
                {
                    int i = vertices.Count;
                    float x0 = x + g.x0 * unit, x1 = x + g.x1 * unit;
                    float y0 = baseline + g.y0 * unit * characterScale.y, y1 = baseline + g.y1 * unit * characterScale.y;
                    vertices.Add(Vector3.Scale(new Vector3(x0, y0, 0), label.scale)); vertices.Add(Vector3.Scale(new Vector3(x0, y1, 0), label.scale)); vertices.Add(Vector3.Scale(new Vector3(x1, y1, 0), label.scale)); vertices.Add(Vector3.Scale(new Vector3(x1, y0, 0), label.scale));
                    uv.Add(g.BottomLeft); uv.Add(g.TopLeft); uv.Add(g.TopRight); uv.Add(g.BottomRight);
                    float low = (g.y0 - CjkFontAtlas.CapBottom) / (CjkFontAtlas.CapTop - CjkFontAtlas.CapBottom), high = (g.y1 - CjkFontAtlas.CapBottom) / (CjkFontAtlas.CapTop - CjkFontAtlas.CapBottom);
                    ramp.Add(new Vector2(low, 0)); ramp.Add(new Vector2(high, 0)); ramp.Add(new Vector2(high, 1)); ramp.Add(new Vector2(low, 1));
                    colors.Add(bottom); colors.Add(top); colors.Add(top); colors.Add(bottom);
                    triangles.Add(i); triangles.Add(i + 1); triangles.Add(i + 2); triangles.Add(i); triangles.Add(i + 2); triangles.Add(i + 3);
                }
                x += g.advance * unit;
            }
        }
        translatedMesh.Clear(); translatedMesh.SetVertices(vertices); translatedMesh.SetUVs(0, uv); translatedMesh.SetUVs(1, ramp); translatedMesh.SetColors(colors); translatedMesh.SetTriangles(triangles, 0); translatedMesh.RecalculateBounds();
    }
}
