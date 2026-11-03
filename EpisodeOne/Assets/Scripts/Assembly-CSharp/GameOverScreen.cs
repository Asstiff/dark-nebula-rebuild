using FLD;

public class GameOverScreen : SimpleScreen
{
	public event HandlerDelegate EventResume;

	public event HandlerDelegate EventMainMenu;

	public event HandlerDelegate EventContinue;

	private void Awake()
	{
		btn1.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventMainMenu != null)
			{
				EventMainMenu();
			}
		};
		btn3.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			if (EventContinue != null)
			{
				EventContinue();
			}
		};
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		btn0.SetActive(false);
		btn1.SetActive(true);
		btn2.SetActive(false);
		btn3.SetActive(true);
		btn3.GetComponentInChildren<SimpleButton>().Text = "CONTINUE".Localize();
		btn1.GetComponentInChildren<SimpleButton>().Text = "MAIN MENU".Localize();
		SetTitle("GAME OVER");
	}

	private void ShowMainMenuButton()
	{
		ShowMainMenuButton(true);
	}

	private void ShowMainMenuButton(bool show)
	{
		btn1.SetActive(show);
	}

	public void SetTitle(string stringID)
	{
		lblTitle.GetComponentInChildren<DynamicFont>().Text = stringID.Localize();
	}
}
