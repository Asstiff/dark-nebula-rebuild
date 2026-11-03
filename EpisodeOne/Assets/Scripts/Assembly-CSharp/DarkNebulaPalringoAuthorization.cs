using FLD;
using Palringo;

public class DarkNebulaPalringoAuthorization : PasswordAuthorization
{
	private Palringo.Async _dialogAsync;

	private PalringoLoginPopup _popup;

	public DarkNebulaPalringoAuthorization(string pClientId)
		: base(DarkNebulaPalringo.Connect.BackendApiHost, pClientId)
	{
	}

	private void Complete()
	{
		if (_dialogAsync != null)
		{
			_dialogAsync.Complete();
			_dialogAsync = null;
		}
	}

	private void HandlePopupCanceled()
	{
		Complete();
		_popup.Close();
		_popup = null;
	}

	private void HandlePopupAccepted(string pEmail, string pPassword)
	{
		SetCredentials(pEmail, pPassword);
		base.Authorize().Wait(() =>
		{
			if (AccessToken.IsValid)
			{
				Complete();
				_popup.LoginSucceeded();
				_popup = null;
			}
			else
			{
				DarkNebulaMetrics.PalringoConnectFail("No reason given");
				_popup.TryAgain("PALRINGO FAILED AUTHORIZE".Localize());
			}
		});
	}

	public override Palringo.Async Authorize()
	{
		_token = AccessToken.EmptyToken;
		if (_dialogAsync == null)
		{
			_dialogAsync = new Palringo.Async();
			_popup = PalringoLoginPopup.Open();
			_popup.Canceled += HandlePopupCanceled;
			_popup.Accepted += HandlePopupAccepted;
		}
		return _dialogAsync;
	}

	public override void OpenAppStore()
	{
		DarkNebulaPalringo.Connect.GetNativeAuthorizationService().OpenAppStore();
	}
}
