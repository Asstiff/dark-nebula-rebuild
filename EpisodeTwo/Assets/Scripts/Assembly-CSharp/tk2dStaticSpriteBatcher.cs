using System.Collections.Generic;
using UnityEngine;
using tk2dRuntime;

[AddComponentMenu("2D Toolkit/Sprite/tk2dStaticSpriteBatcher")]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class tk2dStaticSpriteBatcher : MonoBehaviour, ISpriteCollectionForceBuild
{
	public static int CURRENT_VERSION = 2;

	public int version;

	public tk2dBatchedSprite[] batchedSprites;

	public tk2dSpriteCollectionData spriteCollection;

	private Mesh mesh;

	private Mesh colliderMesh;

	[SerializeField]
	private Vector3 _scale = new Vector3(1f, 1f, 1f);

	private void Awake()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		Build();
	}

	private bool UpgradeData()
	{
		if (version == CURRENT_VERSION)
		{
			return false;
		}
		if (_scale == Vector3.zero)
		{
			_scale = Vector3.one;
		}
		if (version < 2 && batchedSprites != null)
		{
			tk2dBatchedSprite[] array = batchedSprites;
			foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
			{
				tk2dBatchedSprite2.parentId = -1;
			}
		}
		version = CURRENT_VERSION;
		return true;
	}

	public void Build()
	{
		UpgradeData();
		if ((bool)mesh)
		{
			mesh.Clear();
		}
		if ((bool)colliderMesh)
		{
			Object.Destroy(colliderMesh);
			colliderMesh = null;
		}
		if ((bool)spriteCollection && batchedSprites != null && batchedSprites.Length != 0)
		{
			SortBatchedSprites();
			BuildRenderMesh();
			BuildPhysicsMesh();
		}
	}

	private void SortBatchedSprites()
	{
		List<tk2dBatchedSprite> list = new List<tk2dBatchedSprite>();
		List<tk2dBatchedSprite> list2 = new List<tk2dBatchedSprite>();
		List<tk2dBatchedSprite> list3 = new List<tk2dBatchedSprite>();
		tk2dBatchedSprite[] array = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				list3.Add(tk2dBatchedSprite2);
				continue;
			}
			tk2dSpriteDefinition tk2dSpriteDefinition2 = spriteCollection.spriteDefinitions[tk2dBatchedSprite2.spriteId];
			if (tk2dSpriteDefinition2.material.renderQueue == 2000)
			{
				list.Add(tk2dBatchedSprite2);
			}
			else
			{
				list2.Add(tk2dBatchedSprite2);
			}
		}
		List<tk2dBatchedSprite> list4 = new List<tk2dBatchedSprite>(list.Count + list2.Count + list3.Count);
		list4.AddRange(list);
		list4.AddRange(list2);
		list4.AddRange(list3);
		Dictionary<tk2dBatchedSprite, int> dictionary = new Dictionary<tk2dBatchedSprite, int>();
		int num = 0;
		foreach (tk2dBatchedSprite item in list4)
		{
			dictionary[item] = num++;
		}
		foreach (tk2dBatchedSprite item2 in list4)
		{
			if (item2.parentId != -1)
			{
				item2.parentId = dictionary[batchedSprites[item2.parentId]];
			}
		}
		batchedSprites = list4.ToArray();
	}

	private void BuildRenderMesh()
	{
		List<Material> list = new List<Material>();
		List<List<int>> list2 = new List<List<int>>();
		bool flag = false;
		bool flag2 = false;
		if (batchedSprites.Length > 0)
		{
			tk2dSpriteDefinition firstValidDefinition = spriteCollection.FirstValidDefinition;
			flag = firstValidDefinition.normals != null && firstValidDefinition.normals.Length > 0;
			flag2 = firstValidDefinition.tangents != null && firstValidDefinition.tangents.Length > 0;
		}
		int num = 0;
		tk2dBatchedSprite[] array = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition tk2dSpriteDefinition2 = spriteCollection.spriteDefinitions[tk2dBatchedSprite2.spriteId];
			num += tk2dSpriteDefinition2.positions.Length;
		}
		Vector3[] array2 = ((!flag) ? null : new Vector3[num]);
		Vector4[] array3 = ((!flag2) ? null : new Vector4[num]);
		Vector3[] array4 = new Vector3[num];
		Color[] array5 = new Color[num];
		Vector2[] array6 = new Vector2[num];
		int num2 = 0;
		int num3 = 0;
		Material material = null;
		List<int> list3 = null;
		tk2dBatchedSprite[] array7 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite3 in array7)
		{
			if (!tk2dBatchedSprite3.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition tk2dSpriteDefinition3 = spriteCollection.spriteDefinitions[tk2dBatchedSprite3.spriteId];
			if (tk2dSpriteDefinition3.material != material)
			{
				if (material != null)
				{
					list.Add(material);
					list2.Add(list3);
				}
				material = tk2dSpriteDefinition3.material;
				list3 = new List<int>();
			}
			Color color = tk2dBatchedSprite3.color;
			if (spriteCollection.premultipliedAlpha)
			{
				color.r *= color.a;
				color.g *= color.a;
				color.b *= color.a;
			}
			for (int k = 0; k < tk2dSpriteDefinition3.indices.Length; k++)
			{
				list3.Add(num2 + tk2dSpriteDefinition3.indices[k]);
			}
			for (int l = 0; l < tk2dSpriteDefinition3.positions.Length; l++)
			{
				Vector3 vector = new Vector3(tk2dSpriteDefinition3.positions[l].x * tk2dBatchedSprite3.localScale.x, tk2dSpriteDefinition3.positions[l].y * tk2dBatchedSprite3.localScale.y, tk2dSpriteDefinition3.positions[l].z * tk2dBatchedSprite3.localScale.z);
				vector = tk2dBatchedSprite3.rotation * vector;
				vector += tk2dBatchedSprite3.position;
				vector = new Vector3(vector.x * _scale.x, vector.y * _scale.y, vector.z * _scale.z);
				array4[num2 + l] = vector;
				if (flag)
				{
					array2[num2 + l] = tk2dBatchedSprite3.rotation * tk2dSpriteDefinition3.normals[l];
				}
				if (flag2)
				{
					Vector4 vector2 = tk2dSpriteDefinition3.tangents[l];
					Vector3 vector3 = new Vector3(vector2.x, vector2.y, vector2.z);
					Vector3 vector4 = tk2dBatchedSprite3.rotation * vector3;
					array3[num2 + l] = new Vector4(vector4.x, vector4.y, vector4.z, vector2.w);
				}
				array6[num2 + l] = tk2dSpriteDefinition3.uvs[l];
				array5[num2 + l] = color;
			}
			num3 += tk2dSpriteDefinition3.indices.Length;
			num2 += tk2dSpriteDefinition3.positions.Length;
		}
		if (list3 != null)
		{
			list.Add(material);
			list2.Add(list3);
		}
		if ((bool)mesh)
		{
			mesh.vertices = array4;
			mesh.uv = array6;
			mesh.colors = array5;
			if (flag)
			{
				mesh.normals = array2;
			}
			if (flag2)
			{
				mesh.tangents = array3;
			}
			mesh.subMeshCount = list2.Count;
			for (int m = 0; m < list2.Count; m++)
			{
				mesh.SetTriangles(list2[m].ToArray(), m);
			}
			mesh.RecalculateBounds();
		}
		base.GetComponent<Renderer>().sharedMaterials = list.ToArray();
	}

	private void BuildPhysicsMesh()
	{
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		if (meshCollider != null && base.GetComponent<Collider>() != meshCollider)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		tk2dBatchedSprite[] array = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite2 in array)
		{
			if (!tk2dBatchedSprite2.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition tk2dSpriteDefinition2 = spriteCollection.spriteDefinitions[tk2dBatchedSprite2.spriteId];
			if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Box)
			{
				num += 24;
				num2 += 8;
			}
			else if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
			{
				num += tk2dSpriteDefinition2.colliderIndicesFwd.Length;
				num2 += tk2dSpriteDefinition2.colliderVertices.Length;
			}
		}
		if (num == 0)
		{
			if ((bool)colliderMesh)
			{
				Object.Destroy(colliderMesh);
			}
			return;
		}
		if (meshCollider == null)
		{
			meshCollider = base.gameObject.AddComponent<MeshCollider>();
		}
		if (colliderMesh == null)
		{
			colliderMesh = new Mesh();
		}
		colliderMesh.Clear();
		int num3 = 0;
		Vector3[] array2 = new Vector3[num2];
		int num4 = 0;
		int[] array3 = new int[num];
		tk2dBatchedSprite[] array4 = batchedSprites;
		foreach (tk2dBatchedSprite tk2dBatchedSprite3 in array4)
		{
			if (!tk2dBatchedSprite3.IsDrawn)
			{
				break;
			}
			tk2dSpriteDefinition tk2dSpriteDefinition3 = spriteCollection.spriteDefinitions[tk2dBatchedSprite3.spriteId];
			if (tk2dSpriteDefinition3.colliderType == tk2dSpriteDefinition.ColliderType.Box)
			{
				Vector3 vector = new Vector3(tk2dSpriteDefinition3.colliderVertices[0].x * tk2dBatchedSprite3.localScale.x, tk2dSpriteDefinition3.colliderVertices[0].y * tk2dBatchedSprite3.localScale.y, tk2dSpriteDefinition3.colliderVertices[0].z * tk2dBatchedSprite3.localScale.z);
				Vector3 vector2 = new Vector3(tk2dSpriteDefinition3.colliderVertices[1].x * tk2dBatchedSprite3.localScale.x, tk2dSpriteDefinition3.colliderVertices[1].y * tk2dBatchedSprite3.localScale.y, tk2dSpriteDefinition3.colliderVertices[1].z * tk2dBatchedSprite3.localScale.z);
				Vector3 vector3 = vector - vector2;
				Vector3 vector4 = vector + vector2;
				array2[num3] = tk2dBatchedSprite3.rotation * new Vector3(vector3.x, vector3.y, vector3.z) + tk2dBatchedSprite3.position;
				array2[num3 + 1] = tk2dBatchedSprite3.rotation * new Vector3(vector3.x, vector3.y, vector4.z) + tk2dBatchedSprite3.position;
				array2[num3 + 2] = tk2dBatchedSprite3.rotation * new Vector3(vector4.x, vector3.y, vector3.z) + tk2dBatchedSprite3.position;
				array2[num3 + 3] = tk2dBatchedSprite3.rotation * new Vector3(vector4.x, vector3.y, vector4.z) + tk2dBatchedSprite3.position;
				array2[num3 + 4] = tk2dBatchedSprite3.rotation * new Vector3(vector3.x, vector4.y, vector3.z) + tk2dBatchedSprite3.position;
				array2[num3 + 5] = tk2dBatchedSprite3.rotation * new Vector3(vector3.x, vector4.y, vector4.z) + tk2dBatchedSprite3.position;
				array2[num3 + 6] = tk2dBatchedSprite3.rotation * new Vector3(vector4.x, vector4.y, vector3.z) + tk2dBatchedSprite3.position;
				array2[num3 + 7] = tk2dBatchedSprite3.rotation * new Vector3(vector4.x, vector4.y, vector4.z) + tk2dBatchedSprite3.position;
				for (int k = 0; k < 8; k++)
				{
					Vector3 vector5 = array2[num3 + k];
					vector5 = new Vector3(vector5.x * _scale.x, vector5.y * _scale.y, vector5.z * _scale.z);
					array2[num3 + k] = vector5;
				}
				int[] array5 = new int[24]
				{
					0, 1, 2, 2, 1, 3, 6, 5, 4, 7,
					5, 6, 3, 7, 6, 2, 3, 6, 4, 5,
					1, 4, 1, 0
				};
				int[] array6 = new int[24]
				{
					2, 1, 0, 3, 1, 2, 4, 5, 6, 6,
					5, 7, 6, 7, 3, 6, 3, 2, 1, 5,
					4, 0, 1, 4
				};
				float num5 = tk2dBatchedSprite3.localScale.x * tk2dBatchedSprite3.localScale.y * tk2dBatchedSprite3.localScale.z;
				int[] array7 = ((!(num5 >= 0f)) ? array5 : array6);
				for (int l = 0; l < array7.Length; l++)
				{
					array3[num4 + l] = num3 + array7[l];
				}
				num4 += 24;
				num3 += 8;
			}
			else if (tk2dSpriteDefinition3.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
			{
				for (int m = 0; m < tk2dSpriteDefinition3.colliderVertices.Length; m++)
				{
					Vector3 vector6 = new Vector3(tk2dSpriteDefinition3.colliderVertices[m].x * tk2dBatchedSprite3.localScale.x, tk2dSpriteDefinition3.colliderVertices[m].y * tk2dBatchedSprite3.localScale.y, tk2dSpriteDefinition3.colliderVertices[m].z * tk2dBatchedSprite3.localScale.z);
					vector6 = tk2dBatchedSprite3.rotation * vector6;
					vector6 += tk2dBatchedSprite3.position;
					vector6 = new Vector3(vector6.x * _scale.x, vector6.y * _scale.y, vector6.z * _scale.z);
					array2[num3 + m] = vector6;
				}
				float num6 = tk2dBatchedSprite3.localScale.x * tk2dBatchedSprite3.localScale.y * tk2dBatchedSprite3.localScale.z;
				int[] array8 = ((!(num6 >= 0f)) ? tk2dSpriteDefinition3.colliderIndicesBack : tk2dSpriteDefinition3.colliderIndicesFwd);
				for (int n = 0; n < array8.Length; n++)
				{
					array3[num4 + n] = num3 + array8[n];
				}
				num4 += tk2dSpriteDefinition3.colliderIndicesFwd.Length;
				num3 += tk2dSpriteDefinition3.colliderVertices.Length;
			}
		}
		colliderMesh.vertices = array2;
		colliderMesh.triangles = array3;
		meshCollider.sharedMesh = colliderMesh;
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return this.spriteCollection == spriteCollection;
	}

	public void ForceBuild()
	{
		Build();
	}
}
