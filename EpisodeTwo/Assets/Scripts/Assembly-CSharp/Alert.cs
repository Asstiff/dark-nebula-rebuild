using UnityEngine;

public class Alert : ConfirmScreen
{
	public GameObject lblBody;

	public void SetBody(string stringID)
	{
		string str = Language.Get(stringID).ToUpper();
		lblBody.GetComponent<SimpleText>().SetText(WordWrap.Wrap(str, 23));
	}

	public void SetBodyText(string str)
	{
		lblBody.GetComponent<SimpleText>().SetText(str);
	}
}
