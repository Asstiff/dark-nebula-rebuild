using UnityEngine;

namespace FLD
{
	public abstract class CustomBuildVariables<T> : ScriptableObject where T : CustomBuildVariables<T>, new()
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (object.ReferenceEquals(_instance, null))
				{
					CustomBuildVariablesRedirector customBuildVariablesRedirector = (CustomBuildVariablesRedirector)Resources.Load(CustomBuildVariablesRedirector.ResourcePath, typeof(CustomBuildVariablesRedirector));
					if (customBuildVariablesRedirector != null)
					{
						_instance = customBuildVariablesRedirector.RedirectedObject as T;
					}
					if (_instance == null)
					{
						_instance = ScriptableObject.CreateInstance<T>();
					}
				}
				return _instance;
			}
		}
	}
}
