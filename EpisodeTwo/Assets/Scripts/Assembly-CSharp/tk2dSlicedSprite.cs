using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[AddComponentMenu("2D Toolkit/Sprite/tk2d9SliceSprite")]
[ExecuteInEditMode]
public class tk2dSlicedSprite : tk2dBaseSprite
{
	public enum Anchor
	{
		LowerLeft = 0,
		LowerCenter = 1,
		LowerRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		UpperLeft = 6,
		UpperCenter = 7,
		UpperRight = 8
	}

	private Mesh mesh;

	private Vector2[] meshUvs;

	private Vector3[] meshVertices;

	private Color[] meshColors;

	private int[] meshIndices;

	[SerializeField]
	private Vector2 _dimensions = new Vector2(50f, 50f);

	[SerializeField]
	private Anchor _anchor;

	public bool legacyMode = true;

	public float borderTop = 0.2f;

	public float borderBottom = 0.2f;

	public float borderLeft = 0.2f;

	public float borderRight = 0.2f;

	private Vector3 boundsCenter = Vector3.zero;

	private Vector3 boundsExtents = Vector3.zero;

	public Vector2 dimensions
	{
		get
		{
			return _dimensions;
		}
		set
		{
			if (value != _dimensions)
			{
				_dimensions = value;
				UpdateVertices();
				UpdateCollider();
			}
		}
	}

	public Anchor anchor
	{
		get
		{
			return _anchor;
		}
		set
		{
			if (value != _anchor)
			{
				_anchor = value;
				UpdateVertices();
				UpdateCollider();
			}
		}
	}

	private void Awake()
	{
		if ((bool)collection)
		{
			if (_spriteId < 0 || _spriteId >= collection.Count)
			{
				_spriteId = 0;
			}
			Build();
			if (boxCollider == null)
			{
				boxCollider = GetComponent<BoxCollider>();
			}
		}
	}

	protected void OnDestroy()
	{
		if ((bool)mesh)
		{
			if (Application.isPlaying) Object.Destroy(mesh);
			else Object.DestroyImmediate(mesh);
		}
	}

	protected new void SetColors(Color[] dest)
	{
		Color color = _color;
		if (collection.premultipliedAlpha)
		{
			color.r *= color.a;
			color.g *= color.a;
			color.b *= color.a;
		}
		for (int i = 0; i < dest.Length; i++)
		{
			dest[i] = color;
		}
	}

	protected void SetGeometry(Vector3[] vertices, Vector2[] uvs)
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collection.spriteDefinitions[base.spriteId];
		if (tk2dSpriteDefinition2.positions.Length == 4)
		{
			float z = ((tk2dSpriteDefinition2.colliderType != tk2dSpriteDefinition.ColliderType.Box) ? 0.1f : (tk2dSpriteDefinition2.colliderVertices[1].z * _scale.z));
			if (legacyMode)
			{
				float x = _scale.x;
				float y = _scale.y;
				Vector3[] positions = tk2dSpriteDefinition2.positions;
				Vector3 vector = positions[1] - positions[0];
				Vector3 vector2 = positions[2] - positions[0];
				Vector2[] uvs2 = tk2dSpriteDefinition2.uvs;
				Vector2 vector3 = tk2dSpriteDefinition2.uvs[1] - tk2dSpriteDefinition2.uvs[0];
				Vector2 vector4 = tk2dSpriteDefinition2.uvs[2] - tk2dSpriteDefinition2.uvs[0];
				Vector3 vector5 = new Vector3(positions[0].x * x, positions[0].y * y, positions[0].z * _scale.z);
				Vector3[] array = new Vector3[4]
				{
					vector5,
					vector5 + vector2 * borderBottom,
					vector5 + vector2 * (y - borderTop),
					vector5 + vector2 * y
				};
				Vector2[] array2 = new Vector2[4]
				{
					uvs2[0],
					uvs2[0] + vector4 * borderBottom,
					uvs2[0] + vector4 * (1f - borderTop),
					uvs2[0] + vector4
				};
				for (int i = 0; i < 4; i++)
				{
					meshVertices[i * 4] = array[i];
					meshVertices[i * 4 + 1] = array[i] + vector * borderLeft;
					meshVertices[i * 4 + 2] = array[i] + vector * (x - borderRight);
					meshVertices[i * 4 + 3] = array[i] + vector * x;
					meshUvs[i * 4] = array2[i];
					meshUvs[i * 4 + 1] = array2[i] + vector3 * borderLeft;
					meshUvs[i * 4 + 2] = array2[i] + vector3 * (1f - borderRight);
					meshUvs[i * 4 + 3] = array2[i] + vector3;
				}
				return;
			}
			float x2 = tk2dSpriteDefinition2.texelSize.x;
			float y2 = tk2dSpriteDefinition2.texelSize.y;
			Vector3[] positions2 = tk2dSpriteDefinition2.positions;
			float num = positions2[1].x - positions2[0].x;
			float num2 = positions2[2].y - positions2[0].y;
			float num3 = borderTop * num2;
			float y3 = borderBottom * num2;
			float num4 = borderRight * num;
			float x3 = borderLeft * num;
			float num5 = dimensions.x * x2;
			float num6 = dimensions.y * y2;
			float num7 = 0f;
			float num8 = 0f;
			switch (anchor)
			{
			case Anchor.LowerCenter:
			case Anchor.MiddleCenter:
			case Anchor.UpperCenter:
				num7 = -(int)(dimensions.x / 2f);
				break;
			case Anchor.LowerRight:
			case Anchor.MiddleRight:
			case Anchor.UpperRight:
				num7 = -(int)dimensions.x;
				break;
			}
			switch (anchor)
			{
			case Anchor.MiddleLeft:
			case Anchor.MiddleCenter:
			case Anchor.MiddleRight:
				num8 = -(int)(dimensions.y / 2f);
				break;
			case Anchor.UpperLeft:
			case Anchor.UpperCenter:
			case Anchor.UpperRight:
				num8 = -(int)dimensions.y;
				break;
			}
			num7 *= x2;
			num8 *= y2;
			boundsCenter = new Vector3(num5 / 2f + num7, num6 / 2f + num8, 0f);
			boundsExtents = new Vector3(num5 / 2f, num6 / 2f, z);
			Vector2[] uvs3 = tk2dSpriteDefinition2.uvs;
			Vector2 vector6 = tk2dSpriteDefinition2.uvs[1] - tk2dSpriteDefinition2.uvs[0];
			Vector2 vector7 = tk2dSpriteDefinition2.uvs[2] - tk2dSpriteDefinition2.uvs[0];
			Vector3 vector8 = new Vector3(positions2[0].x, positions2[0].y, positions2[0].z);
			vector8 = new Vector3(num7, num8, 0f);
			Vector3[] array3 = new Vector3[4]
			{
				vector8,
				vector8 + new Vector3(0f, y3, 0f),
				vector8 + new Vector3(0f, num6 - num3, 0f),
				vector8 + new Vector3(0f, num6, 0f)
			};
			Vector2[] array4 = new Vector2[4]
			{
				uvs3[0],
				uvs3[0] + vector7 * borderBottom,
				uvs3[0] + vector7 * (1f - borderTop),
				uvs3[0] + vector7
			};
			for (int j = 0; j < 4; j++)
			{
				meshVertices[j * 4] = array3[j];
				meshVertices[j * 4 + 1] = array3[j] + new Vector3(x3, 0f, 0f);
				meshVertices[j * 4 + 2] = array3[j] + new Vector3(num5 - num4, 0f, 0f);
				meshVertices[j * 4 + 3] = array3[j] + new Vector3(num5, 0f, 0f);
				for (int k = 0; k < 4; k++)
				{
					Vector3 vector9 = meshVertices[j * 4 + k];
					vector9.x *= _scale.x;
					vector9.y *= _scale.y;
					vector9.z *= _scale.z;
					meshVertices[j * 4 + k] = vector9;
				}
				meshUvs[j * 4] = array4[j];
				meshUvs[j * 4 + 1] = array4[j] + vector6 * borderLeft;
				meshUvs[j * 4 + 2] = array4[j] + vector6 * (1f - borderRight);
				meshUvs[j * 4 + 3] = array4[j] + vector6;
			}
		}
		else
		{
			for (int l = 0; l < vertices.Length; l++)
			{
				vertices[l] = Vector3.zero;
			}
		}
	}

	private void SetIndices()
	{
		meshIndices = new int[54]
		{
			0, 4, 1, 1, 4, 5, 1, 5, 2, 2,
			5, 6, 2, 6, 3, 3, 6, 7, 4, 8,
			5, 5, 8, 9, 5, 9, 6, 6, 9, 10,
			6, 10, 7, 7, 10, 11, 8, 12, 9, 9,
			12, 13, 9, 13, 10, 10, 13, 14, 10, 14,
			11, 11, 14, 15
		};
	}

	public override void Build()
	{
		meshUvs = new Vector2[16];
		meshVertices = new Vector3[16];
		meshColors = new Color[16];
		SetIndices();
		SetGeometry(meshVertices, meshUvs);
		SetColors(meshColors);
		if (mesh == null)
		{
			mesh = new Mesh();
		}
		else
		{
			mesh.Clear();
		}
		mesh.vertices = meshVertices;
		mesh.colors = meshColors;
		mesh.uv = meshUvs;
		mesh.triangles = meshIndices;
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;
		UpdateCollider();
		UpdateMaterial();
	}

	protected override void UpdateGeometry()
	{
		UpdateGeometryImpl();
	}

	protected override void UpdateColors()
	{
		UpdateColorsImpl();
	}

	protected override void UpdateVertices()
	{
		UpdateGeometryImpl();
	}

	protected void UpdateColorsImpl()
	{
		SetColors(meshColors);
		mesh.colors = meshColors;
	}

	protected void UpdateGeometryImpl()
	{
		SetGeometry(meshVertices, meshUvs);
		mesh.vertices = meshVertices;
		mesh.uv = meshUvs;
		mesh.RecalculateBounds();
		UpdateCollider();
	}

	private new void UpdateCollider()
	{
		if ((bool)boxCollider)
		{
			boxCollider.center = boundsCenter;
			boxCollider.size = (boundsExtents) * 2f;
		}
	}

	protected override void UpdateMaterial()
	{
		if (base.GetComponent<Renderer>().sharedMaterial != collection.spriteDefinitions[base.spriteId].material)
		{
			base.GetComponent<Renderer>().material = collection.spriteDefinitions[base.spriteId].material;
		}
	}

	protected override int GetCurrentVertexCount()
	{
		return 16;
	}
}
