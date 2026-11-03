using System;
using UnityEngine;

namespace FLD
{
	public class VungleManagerAndroid : VungleManagerAbstract
	{
		private VungleAndroidManager _eventListener;

		public override event Action<double, double> AdView;

		public override event Action AdStart;

		public override event Action AdEnd;

		public override void init(string appId)
		{
			VungleAndroid.init(appId);
			InitEventListener();
		}

		public override void init(string appId, int age, VungleGender gender)
		{
			VungleAndroid.init(appId, age, gender);
			InitEventListener();
		}

		protected void InitEventListener()
		{
			if (_eventListener == null)
			{
				GameObject gameObject = new GameObject();
				_eventListener = gameObject.AddComponent<VungleAndroidManager>();
			}
			VungleAndroidManager.onVungleViewEvent += OnViewEvent;
			VungleAndroidManager.onVungleAdStartEvent += OnAdStart;
			VungleAndroidManager.onVungleAdEndEvent += OnAdEnd;
			VungleAndroidManager.onPauseChanged += OnPauseChanged;
		}

		private void OnPauseChanged(bool pause)
		{
			if (pause)
			{
				onPause();
			}
			else
			{
				onResume();
			}
		}

		public override void Dispose()
		{
			VungleAndroidManager.onVungleViewEvent -= OnViewEvent;
			VungleAndroidManager.onVungleAdStartEvent -= OnAdStart;
			VungleAndroidManager.onVungleAdEndEvent -= OnAdEnd;
			VungleAndroidManager.onPauseChanged -= OnPauseChanged;
		}

		private void OnViewEvent(double watched, double length)
		{
			if (AdView != null)
			{
				AdView(watched, length);
			}
		}

		private void OnAdStart()
		{
			if (AdStart != null)
			{
				AdStart();
			}
		}

		private void OnAdEnd()
		{
			if (AdEnd != null)
			{
				AdEnd();
			}
		}

		public override void onPause()
		{
			VungleAndroid.onPause();
		}

		public override void onResume()
		{
			VungleAndroid.onResume();
		}

		public override bool isVideoAvailable()
		{
			return VungleAndroid.isVideoAvailable();
		}

		public override void setSoundEnabled(bool isEnabled)
		{
			VungleAndroid.setSoundEnabled(isEnabled);
		}

		public override bool isSoundEnabled()
		{
			return VungleAndroid.isSoundEnabled();
		}

		public override void displayAdvert()
		{
			Screen.sleepTimeout = -1;
			VungleAndroid.displayAdvert();
		}

		public override void displayIncentivizedAdvert(bool showCloseButton)
		{
			Screen.sleepTimeout = -1;
			VungleAndroid.displayIncentivizedAdvert(showCloseButton);
		}

		public override void displayIncentivizedAdvert(bool showCloseButton, string name)
		{
			Screen.sleepTimeout = -1;
			VungleAndroid.displayIncentivizedAdvert(showCloseButton, name);
		}
	}
}
