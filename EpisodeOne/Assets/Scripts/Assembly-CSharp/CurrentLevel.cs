using UnityEngine;

public class CurrentLevel : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh _levelText;

	private void Start()
	{
		_levelText.text = LoadLevelScreen.GetLevelName(LoadLevelScreen.LevelToLoad);
	}
}
