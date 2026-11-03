using UnityEngine;

public class OptionScreen : IngameOptionScreen
{
	public GameObject FlipperDifficulty;

	public GameObject BtnFacebookConnect;

	private SimpleButton _facebookConnectButton;

	private bool _lastLoggedInStatus;

	private SimpleButton FacebookConnectButton
	{
		get
		{
			if (_facebookConnectButton == null)
			{
				_facebookConnectButton = BtnFacebookConnect.GetComponent<SimpleButton>();
			}
			return _facebookConnectButton;
		}
	}

	protected override void Start()
	{
		base.Start();
		FlipperDifficulty.GetComponent<FlipperButton>().Init(UserSettings.I.expert, "EXPERT", "NORMAL", "DIFFICULTY");
		FlipperDifficulty.GetComponent<FlipperButton>().EventValueChanged += OnDifficultyValueChanged;
		UpdateFacebookButton();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(FacebookConnectButton == null))
		{
			FacebookConnectButton.ButtonUpEvent += HandleClickFacebookConnectButton;
		}
	}

	private void OnDisable()
	{
		if (!(_facebookConnectButton == null))
		{
			_facebookConnectButton.ButtonUpEvent -= HandleClickFacebookConnectButton;
			_facebookConnectButton = null;
		}
	}

	private void HandleClickFacebookConnectButton(SimpleButton source)
	{
		if (DarkNebulaFacebook.IsLoggedIn)
		{
			DarkNebulaFacebook.Logout();
		}
		else
		{
			DarkNebulaFacebook.AssertLoggedIn();
		}
	}

	public override void Update()
	{
		base.Update();
		if (!(FacebookConnectButton == null) && Time.frameCount % 60 == 0 && _lastLoggedInStatus != DarkNebulaFacebook.IsLoggedIn)
		{
			_lastLoggedInStatus = !_lastLoggedInStatus;
			UpdateFacebookButton();
		}
	}

	private void UpdateFacebookButton()
	{
		if (BuildSettings.Current.FacebookEnabled)
		{
			if (DarkNebulaFacebook.IsLoggedIn)
			{
				BtnFacebookConnect.GetComponentInChildren<SimpleText>().SetStringID("LOGOUT FACEBOOK");
			}
			else
			{
				BtnFacebookConnect.GetComponentInChildren<SimpleText>().SetStringID("CONNECT TO FACEBOOK");
			}
		}
		else
		{
			BtnFacebookConnect.SetActive(false);
		}
	}

	private static void OnDifficultyValueChanged(bool value)
	{
		UserSettings.I.SetExpert(value);
	}
}
