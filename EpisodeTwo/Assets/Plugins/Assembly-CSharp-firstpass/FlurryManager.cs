using System;
using Prime31;

public class FlurryManager : AbstractManager
{
	public static event Action<string> spaceDidDismissEvent;

	public static event Action<string> spaceWillLeaveApplicationEvent;

	public static event Action<string> spaceDidFailToRenderEvent;

	public static event Action<string> spaceDidFailToReceiveAdEvent;

	public static event Action<string> spaceDidReceiveAdEvent;

	static FlurryManager()
	{
		AbstractManager.initialize(typeof(FlurryManager));
	}

	public void spaceDidDismiss(string space)
	{
		if (spaceDidDismissEvent != null)
		{
			spaceDidDismissEvent(space);
		}
	}

	public void spaceWillLeaveApplication(string space)
	{
		if (spaceWillLeaveApplicationEvent != null)
		{
			spaceWillLeaveApplicationEvent(space);
		}
	}

	public void spaceDidFailToRender(string space)
	{
		if (spaceDidFailToRenderEvent != null)
		{
			spaceDidFailToRenderEvent(space);
		}
	}

	public void spaceDidFailToReceiveAd(string space)
	{
		if (spaceDidFailToReceiveAdEvent != null)
		{
			spaceDidFailToReceiveAdEvent(space);
		}
	}

	public void spaceDidReceiveAd(string space)
	{
		if (spaceDidReceiveAdEvent != null)
		{
			spaceDidReceiveAdEvent(space);
		}
	}
}
