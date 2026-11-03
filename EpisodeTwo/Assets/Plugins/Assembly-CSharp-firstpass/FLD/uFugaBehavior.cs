using System.Collections;
using UnityEngine;

namespace FLD
{
	public class uFugaBehavior : MonoBehaviour
	{
		public delegate void ApplicationPauseHandler(bool pOnPause);

		public delegate void ApplicationQuitHandler();

		public delegate void PopupCanceledHandler();

		public delegate void PopupConfirmedHandler();

		public delegate void DelayableFunction();

		public delegate void RunCallbackHandler();

		private RunCallbackHandler callback;

		public event ApplicationPauseHandler onApplicationPause;

		public event ApplicationQuitHandler onApplicationQuit;

		public void InitializePlatformServices()
		{
		}

		private void Awake()
		{
		}

		private void OnApplicationPause(bool pOnPause)
		{
			if (onApplicationPause != null)
			{
				onApplicationPause(pOnPause);
			}
			if (pOnPause)
			{
				PlayerPrefs.Save();
			}
		}

		private void OnApplicationQuit()
		{
			if (onApplicationQuit != null)
			{
				onApplicationQuit();
			}
			PlayerPrefs.Save();
		}

		public void Delay(float pDelay, DelayableFunction pFunction)
		{
			StartCoroutine(DelayCoroutine(pDelay, pFunction));
		}

		private IEnumerator DelayCoroutine(float pDelay, DelayableFunction pFunction)
		{
			yield return new WaitForSeconds(pDelay);
			pFunction();
		}

		public void RunCallbackIn(float time, RunCallbackHandler pCallBack)
		{
			callback = pCallBack;
			iTween.MoveTo(base.gameObject, iTween.Hash("position", Vector3.zero, "oncomplete", "OnCallbackIn", "time", time, "ignoretimescale", true));
		}

		private void OnCallbackIn()
		{
			callback();
		}
	}
}
