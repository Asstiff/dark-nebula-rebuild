using UnityEngine;

namespace FLD
{
	public class BrandingManifest<T> : ScriptableObject where T : BrandingManifest<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					string path = "Db/Branding";
					BrandingReference brandingReference = (BrandingReference)Resources.Load(path, typeof(BrandingReference));
					if (brandingReference == null)
					{
						Debug.LogError("Can't load branding data");
					}
					else
					{
						_instance = (T)brandingReference.ActiveManifest;
					}
				}
				return _instance;
			}
		}
	}
}
