using System;

namespace FLD
{
	public abstract class VungleManagerAbstract
	{
		public abstract event Action<double, double> AdView;

		public abstract event Action AdStart;

		public abstract event Action AdEnd;

		public abstract void init(string appId);

		public abstract void init(string appId, int age, VungleGender gender);

		public abstract void onPause();

		public abstract void onResume();

		public abstract bool isVideoAvailable();

		public abstract void setSoundEnabled(bool isEnabled);

		public abstract bool isSoundEnabled();

		public abstract void displayAdvert();

		public abstract void displayIncentivizedAdvert(bool showCloseButton);

		public abstract void displayIncentivizedAdvert(bool showCloseButton, string name);

		public abstract void Dispose();
	}
}
