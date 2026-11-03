using UnityEngine;

public class StubLoader : MonoBehaviour
{
	public static string LevelToLoad;

	public static void LoadLevel(string pLevelToLoad)
	{
		LevelToLoad = pLevelToLoad;
		Application.LoadLevel(pLevelToLoad);
	}

	private void Start()
	{
		Debug.Log("StubLoader.Load " + LevelToLoad);
		Application.LoadLevel(LevelToLoad);
	}
}
