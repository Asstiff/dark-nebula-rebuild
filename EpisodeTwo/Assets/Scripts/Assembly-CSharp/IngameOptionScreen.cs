using FLD;
using UnityEngine;

public class IngameOptionScreen : MonoBehaviour
{
	public delegate void HandlerDelegate();

	public GameObject BtnCalibrateControls;

	public GameObject BtnGoBack;

	public GameObject FlipperMusic;

	public GameObject FlipperSound;

	public GameObject FlipperPostResults;

	public GameObject Title;

	public GameObject ButtonSphero;

	public SpheroConnectScreen SpheroConnectScreen;

	private bool _isEnabled = true;

	private bool isFirstTime = true;

	public bool IsInMainMenu
	{
		get
		{
			return this is OptionScreen;
		}
	}

	public event HandlerDelegate EventGoBack;

	public event HandlerDelegate EventLaunchCalibrateControls;

	private void Awake()
	{
		SpheroConnectScreen.gameObject.SetActive(false);
	}

	protected virtual void Start()
	{
		Title.GetComponent<SimpleText>().SetStringID("GAME OPTIONS");
		BtnGoBack.GetComponentInChildren<SimpleText>().SetStringID("GO BACK");
		BtnGoBack.GetComponent<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventGoBack != null)
			{
				EventGoBack();
			}
		};
		BtnCalibrateControls.GetComponentInChildren<SimpleText>().SetStringID("CALIBRATE CONTROLS");
		BtnCalibrateControls.GetComponent<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventLaunchCalibrateControls != null)
			{
				EventLaunchCalibrateControls();
			}
		};
		FlipperMusic.GetComponent<FlipperButton>().Init(!Audio.IsMusicMuted, "ENABLED", "DISABLED", "MUSIC");
		FlipperMusic.GetComponent<FlipperButton>().EventValueChanged += OnMusicValueChanged;
		FlipperSound.GetComponent<FlipperButton>().Init(!Audio.IsSoundEffectsMuted, "ENABLED", "DISABLED", "SOUND EFFECTS");
		FlipperSound.GetComponent<FlipperButton>().EventValueChanged += OnSoundValueChanged;
		FlipperPostResults.GetComponent<FlipperButton>().Init(UserSettings.I.GetVibration(), "ENABLED", "DISABLED", "VIBRATION");
		FlipperPostResults.GetComponent<FlipperButton>().EventValueChanged += OnVibrationValueChanged;
		SpheroConnectScreen.EventGoBack += OnExitSpheroConnectScreen;
		UpdateSpheroButton();
		ButtonSphero.GetComponent<SimpleButton>().ButtonUpEvent += OnClickSphero;
	}

	private void UpdateSpheroButton()
	{
		if (BuildSettings.Current.SpheroEnabled)
		{
			if (DarkNebulaInput.UseSphero)
			{
				ButtonSphero.GetComponentInChildren<SimpleText>().SetStringID("DISCONNECT SPHERO");
			}
			else
			{
				ButtonSphero.GetComponentInChildren<SimpleText>().SetStringID("CONNECT SPHERO");
			}
		}
		else
		{
			BtnCalibrateControls.transform.position = ButtonSphero.transform.position;
			ButtonSphero.SetActive(false);
		}
	}

	private void OnClickSphero(SimpleButton source)
	{
		if (DarkNebulaInput.UseSphero)
		{
			SpheroAccelerometer.Instance.Disconnect();
			UpdateSpheroButton();
			return;
		}
		if (IsInMainMenu)
		{
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.MainMenu_Sphero);
		}
		else
		{
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.Pause_Sphero);
		}
		base.gameObject.SetActive(false);
		SpheroConnectScreen.gameObject.SetActive(true);
		SpheroConnectScreen.Search();
	}

	private void OnExitSpheroConnectScreen()
	{
		SpheroConnectScreen.gameObject.SetActive(false);
		base.gameObject.SetActive(true);
		UpdateSpheroButton();
	}

	protected virtual void OnEnable()
	{
		if (isFirstTime)
		{
			isFirstTime = false;
			return;
		}
		UpdateSpheroButton();
		if (IsInMainMenu)
		{
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.MainMenu_Options);
		}
		else
		{
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.Pause_Options);
		}
	}

	public void SetEnabled(bool on)
	{
		_isEnabled = on;
		SimpleButton[] componentsInChildren = GetComponentsInChildren<SimpleButton>();
		foreach (SimpleButton simpleButton in componentsInChildren)
		{
			simpleButton.enabled = on;
		}
		if (IsInMainMenu)
		{
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.MainMenu_Options);
		}
		else
		{
			DarkNebulaMetrics.StartScreen(DarkNebulaMetrics.Screens.Pause_Options);
		}
		UpdateSpheroButton();
	}

	public virtual void Update()
	{
		if (_isEnabled && (Input.GetKeyUp(KeyCode.Menu) || Input.GetKeyUp(KeyCode.Escape)) && EventGoBack != null)
		{
			EventGoBack();
		}
	}

	private void OnMusicValueChanged(bool value)
	{
		Audio.IsMusicMuted = !value;
		DarkNebulaMetrics.MuteMusic(Audio.IsMusicMuted);
	}

	private void OnSoundValueChanged(bool value)
	{
		Audio.IsSoundEffectsMuted = !value;
		DarkNebulaMetrics.MuteSound(Audio.IsSoundEffectsMuted);
	}

	private void OnPostResultValueChanged(bool value)
	{
		UserSettings.I.SetPostResults(value);
	}

	private void OnVibrationValueChanged(bool value)
	{
		UserSettings.I.SetVibration(value);
	}
}
