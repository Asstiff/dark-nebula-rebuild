using FLD;
using UnityEngine;

public class IngameOptionScreen : MonoBehaviour
{
	public delegate void HandlerDelegate();

	public GameObject BtnCalibrateControls;

	public GameObject BtnGoBack;

	public GameObject FlipperMusic;

	public GameObject FlipperSound;

	public GameObject FlipperVibration;

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

	protected virtual void Start()
	{
		BtnGoBack.GetComponent<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventGoBack != null)
			{
				EventGoBack();
			}
		};
		BtnCalibrateControls.GetComponentInChildren<SimpleButton>().Text = "CALIBRATE CONTROLS".Localize();
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
		FlipperVibration.GetComponent<FlipperButton>().Init(UserSettings.Instance.GetVibration(), "ENABLED", "DISABLED", "VIBRATION");
		FlipperVibration.GetComponent<FlipperButton>().EventValueChanged += OnVibrationValueChanged;
		if ((float)(Screen.width / Screen.height) > 0.8f)
		{
			Vector3 position = BtnGoBack.transform.position;
			position.y = -140f;
			BtnGoBack.transform.position = position;
		}
	}

	protected virtual void UpdateSpheroButton()
	{
	}

	private void OnClickSphero(SimpleButton source)
	{
		if (!DarkNebulaInput.UseSphero)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnExitSpheroConnectScreen()
	{
		base.gameObject.SetActive(true);
		UpdateSpheroButton();
	}

	protected virtual void OnEnable()
	{
		if (isFirstTime)
		{
			isFirstTime = false;
		}
		else
		{
			UpdateSpheroButton();
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
	}

	public virtual void Update()
	{
		if (_isEnabled && (Input.GetKeyUp(KeyCode.Menu) || Input.GetKeyUp(KeyCode.Escape)) && EventGoBack != null)
		{
			EventGoBack();
		}
	}

	private void OnMusicValueChanged(bool pMusicEnabled)
	{
		Audio.IsMusicMuted = !pMusicEnabled;
		if (pMusicEnabled && IsInMainMenu)
		{
			MainMenuScreen componentInParent = GetComponentInParent<MainMenuScreen>();
			if (componentInParent != null)
			{
				componentInParent.PlayMenuMusic();
			}
		}
	}

	private void OnSoundValueChanged(bool value)
	{
		Audio.IsSoundEffectsMuted = !value;
	}

	private void OnPostResultValueChanged(bool value)
	{
	}

	private void OnVibrationValueChanged(bool value)
	{
		UserSettings.Instance.SetVibration(value);
	}
}
