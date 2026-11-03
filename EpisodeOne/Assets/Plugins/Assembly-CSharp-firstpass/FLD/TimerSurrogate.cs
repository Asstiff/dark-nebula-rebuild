using System;
using UnityEngine;

namespace FLD
{
	internal class TimerSurrogate : MonoBehaviour
	{
		public event Action ApplicationPaused;

		public event Action ApplicationUnpaused;

		public event Action ApplicationQuit;

		private void OnApplicationQuit()
		{
			if (ApplicationQuit != null)
			{
				ApplicationQuit();
			}
		}

		private void OnApplicationPause(bool pOnPause)
		{
			if (pOnPause)
			{
				if (ApplicationPaused != null)
				{
					ApplicationPaused();
				}
			}
			else if (ApplicationUnpaused != null)
			{
				ApplicationUnpaused();
			}
		}
	}
}
