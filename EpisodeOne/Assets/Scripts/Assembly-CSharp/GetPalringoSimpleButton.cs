using System;
using Palringo;

public class GetPalringoSimpleButton : SimpleButton
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
		GenericWaitPopup.Show();
		DarkNebulaMetrics.PalringoConnectTry();
		DarkNebulaPalringo.Connect.Authorize().Wait((AuthorizationResult pResult) =>
		{
			switch (pResult)
			{
			case AuthorizationResult.Granted:
				DarkNebulaMetrics.PalringoConnectSuccess();
				break;
			case AuthorizationResult.Denied:
				DarkNebulaMetrics.PalringoConnectCancel();
				break;
			default:
				DarkNebulaMetrics.PalringoConnectFail(pResult.ToString());
				break;
			}
			GenericWaitPopup.HidePopup();
			UpdateStatus();
		});
		base.OnButtonUp();
	}
}
