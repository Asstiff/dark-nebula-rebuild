using FLD;
using UnityEngine;

public class MenuHeader : MonoBehaviour
{
	[SerializeField]
	private DynamicFont _titleSingle;

	[SerializeField]
	private DynamicFont _title;

	[SerializeField]
	private DynamicFont _subtitle;

	public SimpleButton BackButton;

	private void Start()
	{
		_subtitle.gameObject.SetActive(false);
		_title.gameObject.SetActive(false);
	}

	public void SetTitleID(string pTitleStringID, bool pShowSubtitle = false)
	{
		if (pShowSubtitle)
		{
			_subtitle.gameObject.SetActive(true);
			_title.gameObject.SetActive(true);
			_titleSingle.gameObject.SetActive(false);
			_title.Text = pTitleStringID.Localize();
		}
		else
		{
			_titleSingle.Text = pTitleStringID.Localize();
		}
	}

	public void UpdateCurrentLevel()
	{
		_subtitle.Text = LoadLevelScreen.GetLevelName(LoadLevelScreen.LevelToLoad);
	}

	public void SetLevel(int pLevel)
	{
		_subtitle.Text = LoadLevelScreen.GetLevelName(pLevel);
	}

	public void SetSubTitleID(string pStringID)
	{
		_subtitle.Text = pStringID.Localize();
	}
}
