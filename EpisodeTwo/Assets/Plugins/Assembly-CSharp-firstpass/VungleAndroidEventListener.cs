using UnityEngine;

public class VungleAndroidEventListener : MonoBehaviour
{
	private void OnEnable()
	{
		VungleAndroidManager.onVungleAdStartEvent += onVungleAdStartEvent;
		VungleAndroidManager.onVungleAdEndEvent += onVungleAdEndEvent;
		VungleAndroidManager.onVungleViewEvent += onVungleViewEvent;
	}

	private void OnDisable()
	{
		VungleAndroidManager.onVungleAdStartEvent -= onVungleAdStartEvent;
		VungleAndroidManager.onVungleAdEndEvent -= onVungleAdEndEvent;
		VungleAndroidManager.onVungleViewEvent -= onVungleViewEvent;
	}

	private void onVungleAdStartEvent()
	{
		Debug.Log("onVungleAdStartEvent");
	}

	private void onVungleAdEndEvent()
	{
		Debug.Log("onVungleAdEndEvent");
	}

	private void onVungleViewEvent(double watched, double length)
	{
		Debug.Log("onVungleViewEvent. watched: " + watched + ", length: " + length);
	}
}
