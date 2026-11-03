using System;

public class OpenPalringoSimpleButton : SimpleButton
{
	public Action ButtonUpdated;

	public override void Start()
	{
		base.Start();
		UpdateStatus();
	}

	public void UpdateStatus()
	{
		base.gameObject.SetActive(false); // Palringo SDK is unavailable in this offline test build.
		if (ButtonUpdated != null)
		{
			ButtonUpdated();
		}
	}

	public override void OnButtonUp()
	{
		if (DarkNebulaPalringo.Connect.IsClientInstalled)
		{
			DarkNebulaPalringo.Connect.OpenPalringo();
		}
		else
		{
			DarkNebulaPalringo.Connect.OpenAppStore();
		}
		base.OnButtonUp();
	}
}
