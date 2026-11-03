using UnityEngine;

public class ChartBoostUIManager : MonoBehaviour
{
	private void OnGUI()
	{
		GUI.matrix = Matrix4x4.Scale(new Vector3(2f, 2f, 2f));
		GUI.skin.button.margin = new RectOffset(0, 0, 10, 10);
		if (GUILayout.Button("Init"))
		{
			ChartBoostAndroid.init(BuildSettings.Current.ChartBoostAppId, BuildSettings.Current.ChartBoostAppSignature);
			ChartBoostAndroid.onStart();
		}
		if (GUILayout.Button("Cache Interstitial"))
		{
			ChartBoostAndroid.cacheInterstitial(null);
		}
		if (GUILayout.Button("Check for Cached Interstitial"))
		{
			Debug.Log("has cached interstitial: " + ChartBoostAndroid.hasCachedInterstitial(null));
		}
		if (GUILayout.Button("Show Interstitial"))
		{
			ChartBoostAndroid.showInterstitial(null);
		}
		if (GUILayout.Button("Cache More Apps"))
		{
			ChartBoostAndroid.cacheMoreApps();
		}
		if (GUILayout.Button("Has Cached More Apps"))
		{
			Debug.Log("has cached more apps: " + ChartBoostAndroid.hasCachedMoreApps());
		}
		if (GUILayout.Button("Show More Apps"))
		{
			ChartBoostAndroid.showMoreApps();
		}
	}
}
