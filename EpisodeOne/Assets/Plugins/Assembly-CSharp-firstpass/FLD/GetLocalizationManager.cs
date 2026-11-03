using UnityEngine;

namespace FLD
{
	public class GetLocalizationManager
	{
		private static GameObject _getLocalizationGameObject;

		private static GetLocalization _getLocalization;

		public static GetLocalization GetLocalization
		{
			get
			{
				if (_getLocalizationGameObject == null)
				{
					_getLocalizationGameObject = new GameObject("GetLocalization");
					_getLocalization = _getLocalizationGameObject.AddComponent<GetLocalization>();
				}
				return _getLocalization;
			}
		}

		public static void CleanUp()
		{
			if (_getLocalizationGameObject != null)
			{
				Object.DestroyImmediate(_getLocalizationGameObject);
			}
		}
	}
}
