using FLD;
using UnityEngine;

public class ConfirmScreen : SimpleScreen
{
	protected const string YesID = "YES";

	protected const string NoID = "NO";

	public event HandlerDelegate EventYES;

	public event HandlerDelegate EventNO;

	private void Start()
	{
		btn0.GetComponentInChildren<DynamicFont>().Text = "YES".Localize();
		btn0.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			EventYES();
		};
		btn1.GetComponentInChildren<DynamicFont>().Text = "NO".Localize();
		btn1.GetComponentInChildren<SimpleButton>().ButtonUpEvent += delegate
		{
			EventNO();
		};
	}

	public void SetTitle(string stringID)
	{
		lblTitle.GetComponentInChildren<DynamicFont>().Text = stringID.Localize();
	}

	public void SetTitleText(string str)
	{
		lblTitle.GetComponentInChildren<DynamicFont>().Text = str;
	}

	public void SetYes(string stringID)
	{
		btn0.GetComponentInChildren<DynamicFont>().Text = stringID.Localize();
	}

	public void SetNo(string stringID)
	{
		btn1.GetComponentInChildren<DynamicFont>().Text = stringID.Localize();
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			EventNO();
		}
	}
}
