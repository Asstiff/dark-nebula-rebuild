using System;
using System.Collections.Generic;
using UnityEngine;

namespace tk2dRuntime.TileMap
{
	public static class ColliderBuilder
	{
		public static void Build(tk2dTileMap tileMap)
		{
			int num = tileMap.Layers.Length;
			for (int i = 0; i < num; i++)
			{
				Layer layer = tileMap.Layers[i];
				if (layer.IsEmpty || !tileMap.data.Layers[i].generateCollider)
				{
					continue;
				}
				for (int j = 0; j < layer.numRows; j++)
				{
					int baseY = j * layer.divY;
					for (int k = 0; k < layer.numColumns; k++)
					{
						int baseX = k * layer.divX;
						SpriteChunk chunk = layer.GetChunk(k, j);
						if (!chunk.IsEmpty)
						{
							BuildForChunk(tileMap, chunk, baseX, baseY);
						}
					}
				}
			}
		}

		public static void BuildForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY)
		{
			Vector3[] vertices = new Vector3[0];
			int[] indices = new int[0];
			BuildLocalMeshForChunk(tileMap, chunk, baseX, baseY, ref vertices, ref indices);
			if (indices.Length > 6)
			{
				vertices = WeldVertices(vertices, ref indices);
				indices = RemoveDuplicateFaces(indices);
			}
			if (vertices.Length > 0)
			{
				if (chunk.colliderMesh != null)
				{
					UnityEngine.Object.DestroyImmediate(chunk.colliderMesh);
					chunk.colliderMesh = null;
				}
				if (chunk.meshCollider == null)
				{
					chunk.meshCollider = chunk.gameObject.GetComponent<MeshCollider>();
					if (chunk.meshCollider == null)
					{
						chunk.meshCollider = chunk.gameObject.AddComponent<MeshCollider>();
					}
				}
				chunk.colliderMesh = tileMap.GetOrCreateMesh();
				chunk.colliderMesh.vertices = vertices;
				chunk.colliderMesh.triangles = indices;
				chunk.colliderMesh.RecalculateBounds();
				if (tileMap.serializeRenderData)
				{
					chunk.mesh.RecalculateNormals();
				}
				chunk.meshCollider.sharedMesh = chunk.colliderMesh;
			}
			else
			{
				chunk.DestroyColliderData(tileMap);
			}
		}

		private static void BuildLocalMeshForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY, ref Vector3[] vertices, ref int[] indices)
		{
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			int num = tileMap.spriteCollection.spriteDefinitions.Length;
			Vector3 tileSize = tileMap.data.tileSize;
			UnityEngine.Object[] tilePrefabs = tileMap.data.tilePrefabs;
			float x = 0f;
			float y = 0f;
			tileMap.data.GetTileOffset(out x, out y);
			int[] spriteIds = chunk.spriteIds;
			for (int i = 0; i < tileMap.partitionSizeY; i++)
			{
				float num2 = (float)((baseY + i) & 1) * x;
				for (int j = 0; j < tileMap.partitionSizeX; j++)
				{
					int num3 = spriteIds[i * tileMap.partitionSizeX + j];
					Vector3 vector = new Vector3(tileSize.x * ((float)j + num2), tileSize.y * (float)i, 0f);
					if (num3 < 0 || num3 >= num || (bool)tilePrefabs[num3])
					{
						continue;
					}
					tk2dSpriteDefinition tk2dSpriteDefinition2 = tileMap.spriteCollection.spriteDefinitions[num3];
					int count = list.Count;
					if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Box)
					{
						Vector3 vector2 = tk2dSpriteDefinition2.colliderVertices[0] + vector;
						Vector3 vector3 = tk2dSpriteDefinition2.colliderVertices[1];
						Vector3 vector4 = vector2 - vector3;
						Vector3 vector5 = vector2 + vector3;
						list.Add(new Vector3(vector4.x, vector4.y, vector4.z));
						list.Add(new Vector3(vector4.x, vector4.y, vector5.z));
						list.Add(new Vector3(vector5.x, vector4.y, vector4.z));
						list.Add(new Vector3(vector5.x, vector4.y, vector5.z));
						list.Add(new Vector3(vector4.x, vector5.y, vector4.z));
						list.Add(new Vector3(vector4.x, vector5.y, vector5.z));
						list.Add(new Vector3(vector5.x, vector5.y, vector4.z));
						list.Add(new Vector3(vector5.x, vector5.y, vector5.z));
						int[] array = new int[24]
						{
							2, 1, 0, 3, 1, 2, 4, 5, 6, 6,
							5, 7, 6, 7, 3, 6, 3, 2, 1, 5,
							4, 0, 1, 4
						};
						int[] array2 = array;
						for (int k = 0; k < array2.Length; k++)
						{
							list2.Add(count + array2[k]);
						}
					}
					else if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
					{
						for (int l = 0; l < tk2dSpriteDefinition2.colliderVertices.Length; l++)
						{
							Vector3 item = tk2dSpriteDefinition2.colliderVertices[l] + vector;
							list.Add(item);
						}
						int[] colliderIndicesFwd = tk2dSpriteDefinition2.colliderIndicesFwd;
						for (int m = 0; m < colliderIndicesFwd.Length; m++)
						{
							list2.Add(count + colliderIndicesFwd[m]);
						}
					}
				}
			}
			vertices = list.ToArray();
			indices = list2.ToArray();
		}

		private static int CompareWeldVertices(Vector3 a, Vector3 b)
		{
			float num = 0.01f;
			float f = a.x - b.x;
			if (Mathf.Abs(f) > num)
			{
				return (int)Mathf.Sign(f);
			}
			float f2 = a.y - b.y;
			if (Mathf.Abs(f2) > num)
			{
				return (int)Mathf.Sign(f2);
			}
			float f3 = a.z - b.z;
			if (Mathf.Abs(f3) > num)
			{
				return (int)Mathf.Sign(f3);
			}
			return 0;
		}

		private static Vector3[] WeldVertices(Vector3[] vertices, ref int[] indices)
		{
			int[] array = new int[vertices.Length];
			for (int i = 0; i < vertices.Length; i++)
			{
				array[i] = i;
			}
			Array.Sort(array, (int a, int b) => CompareWeldVertices(vertices[a], vertices[b]));
			List<Vector3> list = new List<Vector3>();
			int[] array2 = new int[vertices.Length];
			Vector3 vector = vertices[array[0]];
			list.Add(vector);
			array2[array[0]] = list.Count - 1;
			for (int num = 1; num < array.Length; num++)
			{
				Vector3 vector2 = vertices[array[num]];
				if (CompareWeldVertices(vector2, vector) != 0)
				{
					vector = vector2;
					list.Add(vector);
					array2[array[num]] = list.Count - 1;
				}
				array2[array[num]] = list.Count - 1;
			}
			for (int num2 = 0; num2 < indices.Length; num2++)
			{
				indices[num2] = array2[indices[num2]];
			}
			return list.ToArray();
		}

		private static int CompareDuplicateFaces(int[] indices, int face0index, int face1index)
		{
			for (int i = 0; i < 3; i++)
			{
				int num = indices[face0index + i] - indices[face1index + i];
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		private static int[] RemoveDuplicateFaces(int[] indices)
		{
			int[] sortedFaceIndices = new int[indices.Length];
			for (int i = 0; i < indices.Length; i += 3)
			{
				int[] array = new int[3]
				{
					indices[i],
					indices[i + 1],
					indices[i + 2]
				};
				Array.Sort(array);
				sortedFaceIndices[i] = array[0];
				sortedFaceIndices[i + 1] = array[1];
				sortedFaceIndices[i + 2] = array[2];
			}
			int[] array2 = new int[indices.Length / 3];
			for (int j = 0; j < indices.Length; j += 3)
			{
				array2[j / 3] = j;
			}
			Array.Sort(array2, (int a, int b) => CompareDuplicateFaces(sortedFaceIndices, a, b));
			List<int> list = new List<int>();
			for (int num = 0; num < array2.Length; num++)
			{
				if (num != array2.Length - 1 && CompareDuplicateFaces(sortedFaceIndices, array2[num], array2[num + 1]) == 0)
				{
					num++;
					continue;
				}
				for (int num2 = 0; num2 < 3; num2++)
				{
					list.Add(indices[array2[num] + num2]);
				}
			}
			return list.ToArray();
		}
	}
}
