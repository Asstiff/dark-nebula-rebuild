using FLD;
using UnityEngine;

public class OptionScreen : IngameOptionScreen
{
	[SerializeField]
	private SimpleButton _calibrateButton;

	[SerializeField]
	private SimpleButton _restorePurchaseButton;

	[SerializeField]
	private SimpleButton _backButton;

	public GameObject FlipperDifficulty;

	[SerializeField]
	private Alert _alertScreen;

	protected override void Start()
	{
		base.Start();
		base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
		FlipperDifficulty.GetComponent<FlipperButton>().Init(UserSettings.Instance.IsExpertMode, "EXPERT", "NORMAL", "DIFFICULTY");
		FlipperDifficulty.GetComponent<FlipperButton>().EventValueChanged += OnDifficultyValueChanged;
		_calibrateButton.Text = "CALIBRATE CONTROLS".Localize();
		_restorePurchaseButton.Text = "SELECT VESSEL";
		_restorePurchaseButton.ButtonUpEvent += HandleRestorPurchases;
		_backButton.Text = "GO BACK".Localize();
		_restorePurchaseButton.gameObject.SetActive(true);
	}

	protected override void UpdateSpheroButton()
	{
		base.UpdateSpheroButton();
	}

	private void HandleRestorPurchases(SimpleButton pButton)
	{
		AvatarShopScreen.PreviousScene="MainMenu";
		UnityEngine.SceneManagement.SceneManager.LoadScene("AvatarShop");
	}

	private void OnDisable()
	{
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public override void Update()
	{
		base.Update();
	}

	private static void OnDifficultyValueChanged(bool value)
	{
		UserSettings.Instance.IsExpertMode = value;
	}
}
