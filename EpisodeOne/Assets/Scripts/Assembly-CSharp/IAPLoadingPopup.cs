using FLD;
using UnityEngine;

public class IAPLoadingPopup : MonoBehaviour
{
	[SerializeField]
	private DynamicFont _loadingText;

	[SerializeField]
	private DynamicFont _failText;

	[SerializeField]
	private GameObject _container;

	public bool IsVisible;

	private static IAPLoadingPopup _activePopup;

	private void Awake()
	{
		_container.SetActive(false);
	}

	private static IAPLoadingPopup CreateInstance()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UI/IAPLoadingPopup"));
		IAPLoadingPopup component = gameObject.GetComponent<IAPLoadingPopup>();
		component.transform.position = Vector3.zero;
		_activePopup = component;
		return component;
	}

	public static IAPLoadingPopup Show(string pText = "")
	{
		if (_activePopup == null)
		{
			_activePopup = CreateInstance();
		}
		_activePopup.Setup(pText);
		return _activePopup;
	}

	public static IAPLoadingPopup Fail()
	{
		return Fail(string.Empty);
	}

	public static IAPLoadingPopup Fail(string pText)
	{
		IAPLoadingPopup iAPLoadingPopup = Show(string.Empty);
		iAPLoadingPopup.ShowFailText(pText);
		Timer.Delay(2f, HidePopup);
		return iAPLoadingPopup;
	}

	private void Setup(string pText)
	{
		IsVisible = true;
		_container.SetActive(true);
		if (!string.IsNullOrEmpty(pText))
		{
			_loadingText.Text = pText;
		}
		_loadingText.gameObject.SetActive(true);
		_failText.gameObject.SetActive(false);
	}

	public static void HidePopup()
	{
		if (!(_activePopup == null))
		{
			_activePopup.Hide();
		}
	}

	private void Hide()
	{
		IsVisible = false;
		_container.SetActive(false);
	}

	public void ShowFailText(string pText = "")
	{
		if (!string.IsNullOrEmpty(pText))
		{
			_failText.Text = pText;
		}
		_loadingText.gameObject.SetActive(false);
		_failText.gameObject.SetActive(true);
	}
}
