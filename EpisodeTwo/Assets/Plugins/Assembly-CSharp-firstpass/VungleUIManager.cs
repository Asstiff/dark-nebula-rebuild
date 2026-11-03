using UnityEngine;

public class VungleUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = ((Screen.width < 800 && Screen.height < 800) ? 160 : 320);
		float num3 = ((Screen.width < 800 && Screen.height < 800) ? 40 : 80);
		float num4 = num3 + 10f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Init"))
		{
			VungleAndroid.init("com.fld.DarkNebula");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Is Video Available?"))
		{
			Debug.Log("is video available? " + VungleAndroid.isVideoAvailable());
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Is Sound Enabled?"))
		{
			Debug.Log("is sound enabled? " + VungleAndroid.isSoundEnabled());
		}
		left = (float)Screen.width - num2 - 5f;
		num = 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Display Advert"))
		{
			VungleAndroid.displayAdvert();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Display Incentivized Advert"))
		{
			VungleAndroid.displayIncentivizedAdvert(true);
		}
	}
}
