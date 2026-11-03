using UnityEngine;

public class SuperStarScreen : MonoBehaviour
{
	public delegate void CompleteHandler();

	public GameObject star;

	public GameObject btn0;

	public GameObject btn1;

	public GameObject txt0;

	public GameObject txt1;

	public GameObject txt2;

	public GameObject txt3;

	public event CompleteHandler ScreenClosed;

	private void Start()
	{
		Time.timeScale = 1f;
		if (UserSettings.Instance.IsExpertMode)
		{
			star.GetComponent<tk2dSprite>().spriteId = star.GetComponent<tk2dSprite>().GetSpriteIdByName("SuperstarExpert");
		}
		btn1.GetComponent<SimpleButton>().ButtonUpEvent += onContinue;
	}

	private void onContinue(SimpleButton source)
	{
		if (ScreenClosed != null)
		{
			ScreenClosed();
		}
	}

	private void SetEnabled()
	{
		SetEnabled(true);
	}

	private void SetEnabled(bool on)
	{
		btn1.GetComponentInChildren<SimpleButton>().enabled = on;
	}

	private void onPostOnFacebook(SimpleButton source)
	{
		SetEnabled(false);
		Invoke("SetEnabled", 1f);
	}
}
