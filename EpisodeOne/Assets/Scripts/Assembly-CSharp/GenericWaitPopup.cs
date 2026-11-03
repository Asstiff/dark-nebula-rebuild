using UnityEngine;

public class GenericWaitPopup : MonoBehaviour
{
	private static GenericWaitPopup _activePopup;

	private void Awake()
	{
	}

	private static GenericWaitPopup CreateInstance()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UI/GenericWaitPopup"));
		GenericWaitPopup component = gameObject.GetComponent<GenericWaitPopup>();
		component.transform.position = Vector3.zero;
		_activePopup = component;
		return component;
	}

	public static GenericWaitPopup Show()
	{
		if (_activePopup == null)
		{
			_activePopup = CreateInstance();
		}
		_activePopup.gameObject.SetActive(true);
		return _activePopup;
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
		base.gameObject.SetActive(false);
	}
}
