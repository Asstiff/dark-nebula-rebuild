using System;

namespace FLD
{
	public class VungleManagerDummy : VungleManagerAbstract
	{
		public override event Action<double, double> AdView;

		public override event Action AdStart;

		public override event Action AdEnd;

		public override void init(string appId)
		{
		}

		public override void init(string appId, int age, VungleGender gender)
		{
		}

		public override void Dispose()
		{
		}

		public override void onPause()
		{
		}

		public override void onResume()
		{
		}

		public override bool isVideoAvailable()
		{
			return false;
		}

		public override void setSoundEnabled(bool isEnabled)
		{
		}

		public override bool isSoundEnabled()
		{
			return false;
		}

		public override void displayAdvert()
		{
		}

		public override void displayIncentivizedAdvert(bool showCloseButton)
		{
		}

		public override void displayIncentivizedAdvert(bool showCloseButton, string name)
		{
		}
	}
}
