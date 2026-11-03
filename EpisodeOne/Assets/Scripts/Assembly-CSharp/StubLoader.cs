using UnityEngine;

public class StubLoader : MonoBehaviour
{
	public static string LevelToLoad;

	private void Start()
	{
		Application.LoadLevel(LevelToLoad);
	}
}
