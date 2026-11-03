using System;

namespace tk2dRuntime.TileMap
{
	[Serializable]
	public class LayerInfo
	{
		public string name;

		public int hash;

		public bool useColor;

		public bool generateCollider;

		public float z = 0.1f;

		public int unityLayer;

		public bool skipMeshGeneration;

		public LayerInfo()
		{
			unityLayer = 0;
			useColor = true;
			generateCollider = true;
			skipMeshGeneration = false;
		}
	}
}
