using UnityEngine;

namespace FLD
{
	public class CustomBuildVariablesRedirector : ScriptableObject
	{
		public static string ResourcePath = "uFuga2/ActiveBuildVariables";

		public static string AssetPath = "Assets/Resources/" + ResourcePath + ".asset";

		public ScriptableObject RedirectedObject;
	}
}
