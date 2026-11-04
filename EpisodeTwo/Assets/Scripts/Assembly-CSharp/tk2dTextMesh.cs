using System;
using UnityEngine;
using tk2dRuntime;

[AddComponentMenu("2D Toolkit/Text/tk2dTextMesh")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dTextMesh : MonoBehaviour, ISpriteCollectionForceBuild
{
	[Flags]
	private enum UpdateFlags
	{
		UpdateNone = 0,
		UpdateText = 1,
		UpdateColors = 2,
		UpdateBuffers = 4
	}

	[SerializeField]
	private tk2dFontData _font;

	[SerializeField]
	private string _text = string.Empty;

	[SerializeField]
	private Color _color = Color.white;

	[SerializeField]
	private Color _color2 = Color.white;

	[SerializeField]
	private bool _useGradient;

	[SerializeField]
	private int _textureGradient;

	[SerializeField]
	private TextAnchor _anchor = TextAnchor.LowerLeft;

	[SerializeField]
	private Vector3 _scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private Vector3 _charScale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private bool _kerning;

	[SerializeField]
	private int _maxChars = 16;

	[SerializeField]
	private bool _inlineStyling;

	public bool pixelPerfect;

	public float spacing;

	public float lineSpacing;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private Vector2[] uv2;

	private Color[] colors;

	private UpdateFlags updateFlags = UpdateFlags.UpdateBuffers;

	private Mesh mesh;

	private MeshFilter meshFilter;

	private RecoveredVectorFont.Profile vectorProfile;
	private int vectorGeneration = -1;

	public bool UsesRecoveredVectorFont { get { return vectorProfile != null; } }

	private void PrepareVectorFont()
	{
		bool wasVector = vectorProfile != null;
		// The accepted result screen deliberately keeps its recovered renderer.
		RecoveredVectorFont.Profile candidate = Application.isPlaying && gameObject.scene.IsValid()
			&& gameObject.scene.name != "PostComplete" ? RecoveredVectorFont.FindProfile(_font) : null;
		if (candidate != null && !RecoveredVectorFont.Prepare(candidate, _text)) candidate = null;
		if (candidate != vectorProfile)
		{
			if (candidate == null && vectorProfile != null) RecoveredVectorFont.Release(this);
			vectorProfile = candidate;
			updateFlags |= UpdateFlags.UpdateText | UpdateFlags.UpdateColors;
		}
		if (vectorProfile != null) RecoveredVectorFont.Register(this);
		Material material = vectorProfile != null ? RecoveredVectorFont.Material : (_font != null ? _font.material : null);
		if ((vectorProfile != null || wasVector) && material != null && GetComponent<Renderer>().sharedMaterial != material) GetComponent<Renderer>().sharedMaterial = material;
		if (vectorProfile != null && vectorGeneration != RecoveredVectorFont.Generation)
		{
			vectorGeneration = RecoveredVectorFont.Generation;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public void InvalidateVectorFont() { updateFlags |= UpdateFlags.UpdateText; }

	public void RefreshVectorFont()
	{
		if (vectorProfile != null && (vectorGeneration != RecoveredVectorFont.Generation || updateFlags != UpdateFlags.UpdateNone)) Commit();
	}

	private void OnEnable() { if (Application.isPlaying) { if(!IsLiteralText) text = GameLocalization.Text(text); updateFlags |= UpdateFlags.UpdateText | UpdateFlags.UpdateColors; Commit(); } }

	private void OnDisable()
	{
		// A hidden label can change to an unsupported glyph or font. Restore its
		// bitmap before the last vector user releases the shared runtime material.
		if (vectorProfile != null) GetComponent<Renderer>().sharedMaterial = _font != null ? _font.material : null;
		RecoveredVectorFont.Release(this);
		vectorProfile = null;
		vectorGeneration = -1;
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying) return;
		if (vectorProfile != null) RefreshVectorFont();
		else if (updateFlags != UpdateFlags.UpdateNone && gameObject.scene.name != "PostComplete" && RecoveredVectorFont.FindProfile(_font) != null) Commit();
	}

	private Color FontColor(Color color)
	{
		if (vectorProfile == null) return color;
		Color tint = QualitySettings.activeColorSpace == ColorSpace.Linear ? vectorProfile.Tint.linear : vectorProfile.Tint;
		return color * tint;
	}

	private float CharacterAdvance(int current, int next, tk2dFontChar bitmapCharacter)
	{
		float pairKerning = 0f;
		if (_kerning && next >= 0)
		{
			foreach (tk2dFontKerning pair in _font.kerning)
				if (pair.c0 == current && pair.c1 == next) { pairKerning = pair.amount; break; }
		}
		return vectorProfile != null
			? RecoveredVectorFont.Advance(vectorProfile, current, next, spacing, pairKerning, _charScale.x, _scale.x)
			: bitmapCharacter.advance + spacing + pairKerning;
	}

	public tk2dFontData font
	{
		get
		{
			return _font;
		}
		set
		{
			_font = value;
			updateFlags |= UpdateFlags.UpdateText;
			if (base.GetComponent<Renderer>().sharedMaterial != _font.material)
			{
				base.GetComponent<Renderer>().material = _font.material;
			}
		}
	}

    public bool IsLiteralText { get; private set; }
    public void SetLiteralText(string value)
    {
        IsLiteralText = true;
        _text = value ?? string.Empty;
        updateFlags |= UpdateFlags.UpdateText;
    }

	public string text
	{
		get
		{
			return _text;
		}
		set
		{
			IsLiteralText = false;
            _text = Application.isPlaying ? GameLocalization.Text(value) : value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public Color color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
			updateFlags |= UpdateFlags.UpdateColors;
		}
	}

	public Color color2
	{
		get
		{
			return _color2;
		}
		set
		{
			_color2 = value;
			updateFlags |= UpdateFlags.UpdateColors;
		}
	}

	public bool useGradient
	{
		get
		{
			return _useGradient;
		}
		set
		{
			_useGradient = value;
			updateFlags |= UpdateFlags.UpdateColors;
		}
	}

	public TextAnchor anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			_anchor = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public Vector3 scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public Vector3 charScale
	{
		get
		{
			return _charScale;
		}
		set
		{
			_charScale = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public bool kerning
	{
		get
		{
			return _kerning;
		}
		set
		{
			_kerning = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public int maxChars
	{
		get
		{
			return _maxChars;
		}
		set
		{
			_maxChars = value;
			updateFlags |= UpdateFlags.UpdateBuffers;
		}
	}

	public int textureGradient
	{
		get
		{
			return _textureGradient;
		}
		set
		{
			_textureGradient = value % Mathf.Max(1,font.gradientCount);
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public bool inlineStyling
	{
		get
		{
			return _inlineStyling;
		}
		set
		{
			_inlineStyling = value;
			updateFlags |= UpdateFlags.UpdateText;
		}
	}

	public float Spacing
	{
		get
		{
			return spacing;
		}
		set
		{
			if (spacing != value)
			{
				spacing = value;
				updateFlags |= UpdateFlags.UpdateText;
			}
		}
	}

	public float LineSpacing
	{
		get
		{
			return lineSpacing;
		}
		set
		{
			if (lineSpacing != value)
			{
				lineSpacing = value;
				updateFlags |= UpdateFlags.UpdateText;
			}
		}
	}

	private bool useInlineStyling
	{
		get
		{
			return inlineStyling && _font.textureGradients;
		}
	}

	private void Awake()
	{
        if (Application.isPlaying) if(!IsLiteralText) text = GameLocalization.Text(text);
		if (pixelPerfect)
		{
			MakePixelPerfect();
		}
		updateFlags = UpdateFlags.UpdateBuffers;
		Init();
	}

	protected void OnDestroy()
	{
		RecoveredVectorFont.Release(this);
		// Only Init-created meshes belong to this component. A hidden label can
		// still reference a recovered asset which must survive object destruction.
		if (mesh == null) return;
		if (meshFilter != null && meshFilter.sharedMesh == mesh)
			meshFilter.sharedMesh = null;
		if (Application.isPlaying) UnityEngine.Object.Destroy(mesh);
		else UnityEngine.Object.DestroyImmediate(mesh);
		mesh = null;
	}

	public int NumDrawnCharacters()
	{
		bool flag = useInlineStyling;
		int num = 0;
		for (int i = 0; i < _text.Length && num < _maxChars; i++)
		{
			int num2 = _text[i];
			if (num2 == 10)
			{
				continue;
			}
			if (flag && num2 == 94 && i + 1 < _text.Length)
			{
				i++;
				if (_text[i] != '^')
				{
					continue;
				}
			}
			num++;
		}
		return num;
	}

	public int NumTotalCharacters()
	{
		bool flag = useInlineStyling;
		int num = 0;
		for (int i = 0; i < _text.Length; i++)
		{
			int num2 = _text[i];
			if (num2 == 10)
			{
				continue;
			}
			if (flag && num2 == 94 && i + 1 < _text.Length)
			{
				i++;
				if (_text[i] != '^')
				{
					continue;
				}
			}
			num++;
		}
		return num;
	}

	private void PostAlignTextData(int targetStart, int targetEnd, float offsetX)
	{
		for (int i = targetStart * 4; i < targetEnd * 4; i++)
		{
			Vector3 vector = vertices[i];
			vector.x += offsetX;
			vertices[i] = vector;
		}
	}

	private int FillTextData()
	{
		Vector2 vector = new Vector2((float)_textureGradient / (float)Mathf.Max(1, font.gradientCount), 0f);
		float yAnchorForHeight = GetYAnchorForHeight(GetMeshDimensionsForString(_text).y);
		bool flag = useInlineStyling;
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < _text.Length && num3 < _maxChars; i++)
		{
			int num5 = _text[i];
			tk2dFontChar tk2dFontChar2 = _font.GetSafeCharacter(num5);
			if (num5 == 10)
			{
				float lineWidth = num;
				int targetEnd = num3;
				if (num4 != num3)
				{
					float xAnchorForWidth = GetXAnchorForWidth(lineWidth);
					PostAlignTextData(num4, targetEnd, xAnchorForWidth);
				}
				num4 = num3;
				num = 0f;
				num2 -= (_font.lineHeight + lineSpacing) * _scale.y;
				continue;
			}
			if (flag && num5 == 94 && i + 1 < _text.Length)
			{
				i++;
				if (_text[i] != '^')
				{
					int num6 = _text[i] - 48;
					vector = new Vector2((float)num6 / (float)Mathf.Max(1, font.gradientCount), 0f);
					continue;
				}
			}
			Vector3 p0 = tk2dFontChar2.p0, p1 = tk2dFontChar2.p1;
			RecoveredVectorFont.Glyph vectorGlyph = new RecoveredVectorFont.Glyph();
			bool vectorCharacter = vectorProfile != null && RecoveredVectorFont.TryGetGlyph(vectorProfile, num5, out vectorGlyph);
			if (vectorCharacter) { p0 = vectorGlyph.p0; p1 = vectorGlyph.p1; }
			float num7 = (p1.x - p0.x) * 0.33f;
			float num8 = (p1.y - p0.y) * 0.1f;
			float num9 = (1f - _charScale.x) * num7;
			float num10 = (1f - _charScale.y) * num8;
			vertices[num3 * 4] = new Vector3(num + p0.x * _scale.x * _charScale.x + num9, yAnchorForHeight + num2 + p0.y * _scale.y * _charScale.y + num10, 0f);
			vertices[num3 * 4 + 1] = new Vector3(num + p1.x * _scale.x * _charScale.x + num9, yAnchorForHeight + num2 + p0.y * _scale.y * _charScale.y + num10, 0f);
			vertices[num3 * 4 + 2] = new Vector3(num + p0.x * _scale.x * _charScale.x + num9, yAnchorForHeight + num2 + p1.y * _scale.y * _charScale.y + num10, 0f);
			vertices[num3 * 4 + 3] = new Vector3(num + p1.x * _scale.x * _charScale.x + num9, yAnchorForHeight + num2 + p1.y * _scale.y * _charScale.y + num10, 0f);
			if (vectorCharacter)
			{
				uvs[num3 * 4] = vectorGlyph.topLeft;
				uvs[num3 * 4 + 1] = vectorGlyph.topRight;
				uvs[num3 * 4 + 2] = vectorGlyph.bottomLeft;
				uvs[num3 * 4 + 3] = vectorGlyph.bottomRight;
			}
			else if (tk2dFontChar2.flipped)
			{
				uvs[num3 * 4] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv1.y);
				uvs[num3 * 4 + 1] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv0.y);
				uvs[num3 * 4 + 2] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv1.y);
				uvs[num3 * 4 + 3] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv0.y);
			}
			else
			{
				uvs[num3 * 4] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv0.y);
				uvs[num3 * 4 + 1] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv0.y);
				uvs[num3 * 4 + 2] = new Vector2(tk2dFontChar2.uv0.x, tk2dFontChar2.uv1.y);
				uvs[num3 * 4 + 3] = new Vector2(tk2dFontChar2.uv1.x, tk2dFontChar2.uv1.y);
			}
			if (_font.textureGradients)
			{
				uv2[num3 * 4] = vector + tk2dFontChar2.gradientUv[0];
				uv2[num3 * 4 + 1] = vector + tk2dFontChar2.gradientUv[1];
				uv2[num3 * 4 + 2] = vector + tk2dFontChar2.gradientUv[2];
				uv2[num3 * 4 + 3] = vector + tk2dFontChar2.gradientUv[3];
			}
			num += CharacterAdvance(num5, i + 1 < _text.Length ? _text[i + 1] : -1, tk2dFontChar2) * _scale.x;
			num3++;
		}
		if (num4 != num3)
		{
			float lineWidth2 = num;
			int targetEnd2 = num3;
			float xAnchorForWidth2 = GetXAnchorForWidth(lineWidth2);
			PostAlignTextData(num4, targetEnd2, xAnchorForWidth2);
		}
		return num3;
	}

	public void Init(bool force)
	{
		if (force)
		{
			updateFlags |= UpdateFlags.UpdateBuffers;
		}
		Init();
	}

	public void Init()
	{
        if (Application.isPlaying) CjkTextRenderer.Restore(this);
		if (_font == null) return;
		PrepareVectorFont();
		if (!_font || ((updateFlags & UpdateFlags.UpdateBuffers) == 0 && !(mesh == null)))
		{
            if (Application.isPlaying) CjkTextRenderer.Render(this);
			return;
		}
		_font.InitDictionary();
		Color color = FontColor(_color);
		Color color2 = FontColor((!_useGradient) ? _color : _color2);
		vertices = new Vector3[_maxChars * 4];
		uvs = new Vector2[_maxChars * 4];
		colors = new Color[_maxChars * 4];
		if (_font.textureGradients)
		{
			uv2 = new Vector2[_maxChars * 4];
		}
		int[] array = new int[_maxChars * 6];
		int num = FillTextData();
		for (int i = 0; i < num; i++)
		{
			colors[i * 4] = (colors[i * 4 + 1] = color);
			colors[i * 4 + 2] = (colors[i * 4 + 3] = color2);
			array[i * 6] = i * 4;
			array[i * 6 + 1] = i * 4 + 1;
			array[i * 6 + 2] = i * 4 + 3;
			array[i * 6 + 3] = i * 4 + 2;
			array[i * 6 + 4] = i * 4;
			array[i * 6 + 5] = i * 4 + 3;
		}
		for (int j = num; j < _maxChars; j++)
		{
			vertices[j * 4] = (vertices[j * 4 + 1] = (vertices[j * 4 + 2] = (vertices[j * 4 + 3] = Vector3.zero)));
			uvs[j * 4] = (uvs[j * 4 + 1] = (uvs[j * 4 + 2] = (uvs[j * 4 + 3] = Vector2.zero)));
			if (_font.textureGradients)
			{
				uv2[j * 4] = (uv2[j * 4 + 1] = (uv2[j * 4 + 2] = (uv2[j * 4 + 3] = Vector2.zero)));
			}
			colors[j * 4] = (colors[j * 4 + 1] = color);
			colors[j * 4 + 2] = (colors[j * 4 + 3] = color2);
			array[j * 6] = j * 4;
			array[j * 6 + 1] = j * 4 + 1;
			array[j * 6 + 2] = j * 4 + 3;
			array[j * 6 + 3] = j * 4 + 2;
			array[j * 6 + 4] = j * 4;
			array[j * 6 + 5] = j * 4 + 3;
		}
		if (mesh == null)
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			mesh = new Mesh();
			meshFilter.mesh = mesh;
		}
		mesh.vertices = vertices;
		mesh.uv = uvs;
		if (font.textureGradients)
		{
			mesh.uv2 = uv2;
		}
		mesh.triangles = array;
		mesh.colors = colors;
		mesh.RecalculateBounds();
		updateFlags = UpdateFlags.UpdateNone;
        if (Application.isPlaying) CjkTextRenderer.Render(this);
	}

	public void Commit()
	{
        if (Application.isPlaying) CjkTextRenderer.Restore(this);
		if (_font == null) return;
		PrepareVectorFont();
		_font.InitDictionary();
		if ((updateFlags & UpdateFlags.UpdateBuffers) != UpdateFlags.UpdateNone || mesh == null)
		{
			Init();
		}
		else
		{
			if ((updateFlags & UpdateFlags.UpdateText) != UpdateFlags.UpdateNone)
			{
				int num = FillTextData();
				for (int i = num; i < _maxChars; i++)
				{
					vertices[i * 4] = (vertices[i * 4 + 1] = (vertices[i * 4 + 2] = (vertices[i * 4 + 3] = Vector3.zero)));
				}
				mesh.vertices = vertices;
				mesh.uv = uvs;
				if (vectorProfile != null) mesh.RecalculateBounds();
				if (font.textureGradients)
				{
					mesh.uv2 = uv2;
				}
			}
			if ((updateFlags & UpdateFlags.UpdateColors) != UpdateFlags.UpdateNone)
			{
				Color color = FontColor(_color);
				Color color2 = FontColor((!_useGradient) ? _color : _color2);
				for (int j = 0; j < colors.Length; j += 4)
				{
					colors[j] = (colors[j + 1] = color);
					colors[j + 2] = (colors[j + 3] = color2);
				}
				mesh.colors = colors;
			}
		}
		updateFlags = UpdateFlags.UpdateNone;
        if (Application.isPlaying) CjkTextRenderer.Render(this);
	}

	private Vector2 GetMeshDimensionsForString(string str)
	{
		bool flag = useInlineStyling;
		float b = 0f;
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		for (int i = 0; i < str.Length && num3 < _maxChars; i++)
		{
			int num4 = str[i];
			if (num4 == 10)
			{
				b = Mathf.Max(num, b);
				num = 0f;
				num2 -= (_font.lineHeight + lineSpacing) * _scale.y;
				continue;
			}
			if (flag && num4 == 94 && i + 1 < str.Length)
			{
				i++;
				if (str[i] != '^')
				{
					continue;
				}
			}
			tk2dFontChar tk2dFontChar2 = _font.GetSafeCharacter(num4);
			num += CharacterAdvance(num4, i + 1 < str.Length ? str[i + 1] : -1, tk2dFontChar2) * _scale.x;
			num3++;
		}
		b = Mathf.Max(num, b);
		num2 -= (_font.lineHeight + lineSpacing) * _scale.y;
		return new Vector2(b, num2);
	}

	private float GetYAnchorForHeight(float textHeight)
	{
		int num = (int)_anchor / 3;
		float num2 = (_font.lineHeight + lineSpacing) * _scale.y;
		switch (num)
		{
		case 0:
			return 0f - num2;
		case 1:
			return (0f - textHeight) / 2f - num2;
		case 2:
			return 0f - textHeight - num2;
		default:
			return 0f - num2;
		}
	}

	private float GetXAnchorForWidth(float lineWidth)
	{
		switch ((int)_anchor % 3)
		{
		case 0:
			return 0f;
		case 1:
			return (0f - lineWidth) / 2f;
		case 2:
			return 0f - lineWidth;
		default:
			return 0f;
		}
	}

	public void MakePixelPerfect()
	{
		float num = 1f;
		tk2dPixelPerfectHelper inst = tk2dPixelPerfectHelper.inst;
		if ((bool)inst)
		{
			num = ((!inst.CameraIsOrtho) ? (inst.scaleK + inst.scaleD * base.transform.position.z) : inst.scaleK);
		}
		else if (tk2dCamera.inst != null)
		{
			if (font.version < 1)
			{
				Debug.LogError("Need to rebuild font.");
			}
			num = font.invOrthoSize * font.halfTargetHeight;
		}
		else if ((bool)Camera.main)
		{
			if (Camera.main.orthographic)
			{
				num = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = base.transform.position.z - Camera.main.transform.position.z;
				num = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, zdist);
			}
		}
		scale = new Vector3(Mathf.Sign(scale.x) * num, Mathf.Sign(scale.y) * num, Mathf.Sign(scale.z) * num);
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return true;
	}

	public void ForceBuild()
	{
		Init(true);
	}
}
