using System;
using UnityEngine;

namespace FLD
{
	public class VungleManager
	{
		private static VungleManagerAbstract _instance;

		public static VungleManagerAbstract Instance
		{
			get
			{
				if (_instance == null && BuildSettings.Current.VungleEnabled)
				{
					try
					{
						_instance = new VungleManagerAndroid();
					}
					catch (Exception ex)
					{
						Debug.LogWarning("Vungle could not be initialized " + ex.Message);
						_instance = new VungleManagerDummy();
					}
				}
				if (_instance == null)
				{
					_instance = new VungleManagerDummy();
				}
				return _instance;
			}
		}
	}
}
