using FLD;
using UnityEngine;

public class Alert : ConfirmScreen
{
	public GameObject lblBody;

	[SerializeField]
	private GameObject _titleEnhancer;

	private void Awake()
	{
		base.transform.position = new Vector3(0f, 0f, base.transform.position.z);
	}

	public void SetBody(string stringID)
	{
		lblBody.GetComponent<DynamicFont>().Text = stringID.Localize().ToUpper();
	}

	public void Reset()
	{
		_titleEnhancer.gameObject.SetActive(false);
		btn0.gameObject.SetActive(true);
		btn1.gameObject.SetActive(true);
		btn0.GetComponentInChildren<DynamicFont>().Text = "YES".Localize();
		btn1.GetComponentInChildren<DynamicFont>().Text = "NO".Localize();
	}

	public void ShowTitleEnhancer()
	{
		_titleEnhancer.gameObject.SetActive(true);
	}

	public void HideYes()
	{
		btn0.gameObject.SetActive(false);
	}

	public void HideNo()
	{
		btn1.gameObject.SetActive(false);
	}

	public void SetBodyText(string str)
	{
		lblBody.GetComponent<DynamicFont>().Text = str;
	}
}
