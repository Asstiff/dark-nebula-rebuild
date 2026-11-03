using UnityEngine;

public class FullScreenGUIBackground : MonoBehaviour
{
	public Texture2D BackgroundTexture;

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), BackgroundTexture, ScaleMode.ScaleToFit);
	}
}
