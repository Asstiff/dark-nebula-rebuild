using System.Collections.Generic;
using UnityEngine;

namespace tk2dRuntime.TileMap
{
	public static class BuilderUtil
	{
		public static bool InitDataStore(tk2dTileMap tileMap)
		{
			bool result = false;
			int numLayers = tileMap.data.NumLayers;
			if (tileMap.Layers == null)
			{
				tileMap.Layers = new Layer[numLayers];
				for (int i = 0; i < numLayers; i++)
				{
					tileMap.Layers[i] = new Layer(tileMap.data.Layers[i].hash, tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
				}
				result = true;
			}
			else
			{
				Layer[] array = new Layer[numLayers];
				for (int j = 0; j < numLayers; j++)
				{
					LayerInfo layerInfo = tileMap.data.Layers[j];
					bool flag = false;
					for (int k = 0; k < tileMap.Layers.Length; k++)
					{
						if (tileMap.Layers[k].hash == layerInfo.hash)
						{
							array[j] = tileMap.Layers[k];
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						array[j] = new Layer(layerInfo.hash, tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
					}
				}
				int num = 0;
				Layer[] array2 = array;
				foreach (Layer layer in array2)
				{
					if (!layer.IsEmpty)
					{
						num++;
					}
				}
				int num2 = 0;
				Layer[] layers = tileMap.Layers;
				foreach (Layer layer2 in layers)
				{
					if (!layer2.IsEmpty)
					{
						num2++;
					}
				}
				if (num != num2)
				{
					result = true;
				}
				tileMap.Layers = array;
			}
			if (tileMap.ColorChannel == null)
			{
				tileMap.ColorChannel = new ColorChannel(tileMap.width, tileMap.height, tileMap.partitionSizeX, tileMap.partitionSizeY);
			}
			return result;
		}

		public static void CleanRenderData(tk2dTileMap tileMap)
		{
			if (tileMap.renderData == null)
			{
				return;
			}
			int num = 0;
			List<Transform> list = new List<Transform>();
			list.Add(tileMap.renderData.transform);
			while (num < list.Count)
			{
				Transform transform = list[num++];
				int childCount = transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					list.Add(transform.GetChild(i));
				}
			}
			num = list.Count - 1;
			while (num > 0)
			{
				GameObject gameObject = list[num--].gameObject;
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				if (component != null)
				{
					Mesh sharedMesh = component.sharedMesh;
					component.sharedMesh = null;
					tileMap.DestroyMesh(sharedMesh);
				}
				MeshCollider component2 = gameObject.GetComponent<MeshCollider>();
				if ((bool)component2)
				{
					Mesh sharedMesh2 = component2.sharedMesh;
					component2.sharedMesh = null;
					tileMap.DestroyMesh(sharedMesh2);
				}
				Object.DestroyImmediate(gameObject);
			}
			tileMap.buildKey++;
		}

		public static void SpawnPrefabsForChunk(tk2dTileMap tileMap, SpriteChunk chunk, int baseX, int baseY)
		{
			int[] spriteIds = chunk.spriteIds;
			Object[] tilePrefabs = tileMap.data.tilePrefabs;
			Vector3 tileSize = tileMap.data.tileSize;
			int[] array = new int[tilePrefabs.Length];
			Transform transform = chunk.gameObject.transform;
			float x = 0f;
			float y = 0f;
			tileMap.data.GetTileOffset(out x, out y);
			for (int i = 0; i < tileMap.partitionSizeY; i++)
			{
				float num = (float)((baseY + i) & 1) * x;
				for (int j = 0; j < tileMap.partitionSizeX; j++)
				{
					int num2 = spriteIds[i * tileMap.partitionSizeX + j];
					if (num2 < 0 || num2 >= tilePrefabs.Length)
					{
						continue;
					}
					Vector3 localPosition = new Vector3(tileSize.x * ((float)j + num), tileSize.y * (float)i, 0f);
					Object obj = tilePrefabs[num2];
					if (obj != null)
					{
						array[num2]++;
						GameObject gameObject = Object.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
						if ((bool)gameObject)
						{
							gameObject.name = obj.name + " " + array[num2];
							gameObject.transform.parent = transform;
							gameObject.transform.localPosition = localPosition;
							gameObject.transform.localRotation = Quaternion.identity;
							gameObject.transform.localScale = Vector3.one;
						}
					}
				}
			}
		}

		public static void SpawnPrefabs(tk2dTileMap tileMap)
		{
			int num = tileMap.Layers.Length;
			for (int i = 0; i < num; i++)
			{
				Layer layer = tileMap.Layers[i];
				if (layer.IsEmpty || tileMap.data.Layers[i].skipMeshGeneration)
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
							SpawnPrefabsForChunk(tileMap, chunk, baseX, baseY);
						}
					}
				}
			}
		}

		private static Vector3 GetTilePosition(tk2dTileMap tileMap, int x, int y)
		{
			return new Vector3(tileMap.data.tileSize.x * (float)x, tileMap.data.tileSize.y * (float)y, 0f);
		}

		public static void CreateRenderData(tk2dTileMap tileMap, bool editMode)
		{
			if (tileMap.renderData == null)
			{
				tileMap.renderData = new GameObject(tileMap.name + " Render Data");
			}
			tileMap.renderData.transform.position = tileMap.transform.position;
			float num = 0f;
			int num2 = 0;
			Layer[] layers = tileMap.Layers;
			foreach (Layer layer in layers)
			{
				if (num2 != 0)
				{
					num -= tileMap.data.Layers[num2].z;
				}
				if (layer.IsEmpty && layer.gameObject != null)
				{
					Object.DestroyImmediate(layer.gameObject);
					layer.gameObject = null;
				}
				else if (!layer.IsEmpty && layer.gameObject == null)
				{
					(layer.gameObject = new GameObject(string.Empty)).transform.parent = tileMap.renderData.transform;
				}
				int unityLayer = tileMap.data.Layers[num2].unityLayer;
				if (layer.gameObject != null)
				{
					if (!editMode && !layer.gameObject.active)
					{
						layer.gameObject.SetActiveRecursively(true);
					}
					layer.gameObject.name = tileMap.data.Layers[num2].name;
					layer.gameObject.transform.localPosition = new Vector3(0f, 0f, num);
					layer.gameObject.transform.localRotation = Quaternion.identity;
					layer.gameObject.transform.localScale = Vector3.one;
					layer.gameObject.layer = unityLayer;
				}
				int x;
				int x2;
				int dx;
				int y;
				int y2;
				int dy;
				GetLoopOrder(tileMap.data.sortMethod, layer.numColumns, layer.numRows, out x, out x2, out dx, out y, out y2, out dy);
				float num3 = 0f;
				for (int j = y; j != y2; j += dy)
				{
					for (int k = x; k != x2; k += dx)
					{
						SpriteChunk chunk = layer.GetChunk(k, j);
						bool flag = layer.IsEmpty || chunk.IsEmpty;
						if (flag && chunk.HasGameData)
						{
							chunk.DestroyGameData(tileMap);
						}
						else if (!flag && chunk.gameObject == null)
						{
							string name = "Chunk " + j + " " + k;
							GameObject gameObject = (chunk.gameObject = new GameObject(name));
							gameObject.transform.parent = layer.gameObject.transform;
							MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
							gameObject.AddComponent<MeshRenderer>();
							chunk.mesh = tileMap.GetOrCreateMesh();
							meshFilter.mesh = chunk.mesh;
							chunk.meshCollider = gameObject.AddComponent<MeshCollider>();
							chunk.meshCollider.sharedMesh = null;
							chunk.colliderMesh = null;
						}
						if (chunk.gameObject != null)
						{
							Vector3 tilePosition = GetTilePosition(tileMap, k * tileMap.partitionSizeX, j * tileMap.partitionSizeY);
							tilePosition.z += num3;
							chunk.gameObject.transform.localPosition = tilePosition;
							chunk.gameObject.transform.localRotation = Quaternion.identity;
							chunk.gameObject.transform.localScale = Vector3.one;
							chunk.gameObject.layer = unityLayer;
							if (editMode && (bool)chunk.colliderMesh)
							{
								chunk.DestroyColliderData(tileMap);
							}
						}
						num3 -= 1E-06f;
					}
				}
				num2++;
			}
		}

		public static void GetLoopOrder(tk2dTileMapData.SortMethod sortMethod, int w, int h, out int x0, out int x1, out int dx, out int y0, out int y1, out int dy)
		{
			switch (sortMethod)
			{
			case tk2dTileMapData.SortMethod.BottomLeft:
				x0 = 0;
				x1 = w;
				dx = 1;
				y0 = 0;
				y1 = h;
				dy = 1;
				break;
			case tk2dTileMapData.SortMethod.BottomRight:
				x0 = w - 1;
				x1 = -1;
				dx = -1;
				y0 = 0;
				y1 = h;
				dy = 1;
				break;
			case tk2dTileMapData.SortMethod.TopLeft:
				x0 = 0;
				x1 = w;
				dx = 1;
				y0 = h - 1;
				y1 = -1;
				dy = -1;
				break;
			case tk2dTileMapData.SortMethod.TopRight:
				x0 = w - 1;
				x1 = -1;
				dx = -1;
				y0 = h - 1;
				y1 = -1;
				dy = -1;
				break;
			default:
				Debug.LogError("Unhandled sort method");
				goto case tk2dTileMapData.SortMethod.BottomLeft;
			}
		}
	}
}
