using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	internal static class PrefabPoolContainer
	{
		private static Dictionary<string, GameObject> _activeContainers = new Dictionary<string, GameObject>();

		public static GameObject Get(string pName)
		{
			GameObject value = null;
			if (!_activeContainers.TryGetValue(pName, out value) || value == null)
			{
				value = new GameObject(pName);
				_activeContainers[pName] = value;
			}
			return value;
		}
	}
}
