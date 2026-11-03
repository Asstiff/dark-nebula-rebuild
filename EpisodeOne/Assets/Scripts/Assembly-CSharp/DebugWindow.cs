using UnityEngine;

public class DebugWindow : MonoBehaviour
{
	public static string line1 = string.Empty;

	public static string line2 = string.Empty;

	public static string line3 = string.Empty;

	public bool Visible;

	private static DebugWindow _activePopup;

	private static DebugWindow CreateInstance()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UI/DebugWindow"));
		DebugWindow component = gameObject.GetComponent<DebugWindow>();
		_activePopup = component;
		return _activePopup;
	}

	public static void Show()
	{
		if (_activePopup == null)
		{
			_activePopup = CreateInstance();
		}
		_activePopup.Visible = true;
	}

	public static void Hide()
	{
		_activePopup.Visible = false;
	}

	private void OnGUI()
	{
		if (Visible)
		{
			string text = "debug info\n" + line1 + "\n" + line2 + "\n" + line3;
			GUI.Box(new Rect(10f, 400f, 100f, 90f), text);
		}
	}
}
