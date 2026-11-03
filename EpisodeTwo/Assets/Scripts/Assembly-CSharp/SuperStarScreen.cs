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
		if (UserSettings.I.expert)
		{
			star.GetComponent<tk2dSprite>().spriteId = star.GetComponent<tk2dSprite>().GetSpriteIdByName("UI_SuperstarExpert");
			txt0.GetComponent<SimpleText>().SetStringID("YOU HAVE RECEIVED EXPERT");
			txt1.GetComponent<SimpleText>().SetStringID("GOLD STARS ON ALL LEVELS V2");
			txt2.GetComponent<SimpleText>().SetStringID("THE TRUE MASTERS ENVY");
			txt3.GetComponent<SimpleText>().SetStringID("YOUR SKILLS!");
		}
		else
		{
			txt0.GetComponent<SimpleText>().SetStringID("YOU HAVE RECEIVED");
			txt1.GetComponent<SimpleText>().SetStringID("GOLD STARS ON ALL LEVELS!");
			txt2.GetComponent<SimpleText>().SetStringID("YOU ARE THE TRUE MASTER");
			txt3.GetComponent<SimpleText>().SetStringID("OF DARK NEBULA!");
		}
		btn0.GetComponent<SimpleButton>().ButtonUpEvent += onPostOnFacebook;
		btn1.GetComponent<SimpleButton>().ButtonUpEvent += onContinue;
		if (!BuildSettings.Current.FacebookEnabled)
		{
			btn0.SetActive(false);
		}
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
		btn0.SetActive(on && BuildSettings.Current.FacebookEnabled);
		btn1.GetComponentInChildren<SimpleButton>().enabled = on;
	}

	private void onPostOnFacebook(SimpleButton source)
	{
		SetEnabled(false);
		Invoke("SetEnabled", 1f);
		if (UserSettings.I.expert)
		{
			DarkNebulaFacebook.PostDemonToStream(World.levelNum);
		}
		else
		{
			DarkNebulaFacebook.PostTrueMasterToStream(World.levelNum);
		}
	}
}
