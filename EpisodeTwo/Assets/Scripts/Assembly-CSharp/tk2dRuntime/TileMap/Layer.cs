using System;
using UnityEngine;

namespace tk2dRuntime.TileMap
{
	[Serializable]
	public class Layer
	{
		public int hash;

		public SpriteChannel spriteChannel;

		public int width;

		public int height;

		public int numColumns;

		public int numRows;

		public int divX;

		public int divY;

		public GameObject gameObject;

		public bool IsEmpty
		{
			get
			{
				return spriteChannel.chunks.Length == 0;
			}
		}

		public int NumActiveChunks
		{
			get
			{
				int num = 0;
				SpriteChunk[] chunks = spriteChannel.chunks;
				foreach (SpriteChunk spriteChunk in chunks)
				{
					if (!spriteChunk.IsEmpty)
					{
						num++;
					}
				}
				return num;
			}
		}

		public Layer(int hash, int width, int height, int divX, int divY)
		{
			spriteChannel = new SpriteChannel();
			Init(hash, width, height, divX, divY);
		}

		public void Init(int hash, int width, int height, int divX, int divY)
		{
			this.divX = divX;
			this.divY = divY;
			this.hash = hash;
			numColumns = (width + divX - 1) / divX;
			numRows = (height + divY - 1) / divY;
			this.width = width;
			this.height = height;
			spriteChannel.chunks = new SpriteChunk[numColumns * numRows];
			for (int i = 0; i < numColumns * numRows; i++)
			{
				spriteChannel.chunks[i] = new SpriteChunk();
			}
		}

		public void Create()
		{
			spriteChannel.chunks = new SpriteChunk[numColumns * numRows];
		}

		public int[] GetChunkData(int x, int y)
		{
			return GetChunk(x, y).spriteIds;
		}

		public SpriteChunk GetChunk(int x, int y)
		{
			return spriteChannel.chunks[y * numColumns + x];
		}

		private SpriteChunk FindChunkAndCoordinate(int x, int y, out int offset)
		{
			int num = x / divX;
			int num2 = y / divY;
			SpriteChunk result = spriteChannel.chunks[num2 * numColumns + num];
			int num3 = x - num * divX;
			int num4 = y - num2 * divY;
			offset = num4 * divX + num3;
			return result;
		}

		public void SetTile(int x, int y, int spriteId)
		{
			int offset;
			SpriteChunk spriteChunk = FindChunkAndCoordinate(x, y, out offset);
			CreateChunk(spriteChunk);
			spriteChunk.spriteIds[offset] = spriteId;
			spriteChunk.Dirty = true;
		}

		public int GetTile(int x, int y)
		{
			int offset;
			SpriteChunk spriteChunk = FindChunkAndCoordinate(x, y, out offset);
			if (spriteChunk.spriteIds == null || spriteChunk.spriteIds.Length == 0)
			{
				return -1;
			}
			return spriteChunk.spriteIds[offset];
		}

		private void CreateChunk(SpriteChunk chunk)
		{
			if (chunk.spriteIds == null || chunk.spriteIds.Length == 0)
			{
				chunk.spriteIds = new int[divX * divY];
				for (int i = 0; i < divX * divY; i++)
				{
					chunk.spriteIds[i] = -1;
				}
			}
		}

		private void Optimize(SpriteChunk chunk)
		{
			bool flag = true;
			int[] spriteIds = chunk.spriteIds;
			foreach (int num in spriteIds)
			{
				if (num != -1)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				chunk.spriteIds = new int[0];
			}
		}

		public void Optimize()
		{
			SpriteChunk[] chunks = spriteChannel.chunks;
			foreach (SpriteChunk chunk in chunks)
			{
				Optimize(chunk);
			}
		}

		public void OptimizeIncremental()
		{
			SpriteChunk[] chunks = spriteChannel.chunks;
			foreach (SpriteChunk spriteChunk in chunks)
			{
				if (spriteChunk.Dirty)
				{
					Optimize(spriteChunk);
				}
			}
		}

		public void ClearDirtyFlag()
		{
			SpriteChunk[] chunks = spriteChannel.chunks;
			foreach (SpriteChunk spriteChunk in chunks)
			{
				spriteChunk.Dirty = false;
			}
		}
	}
}
