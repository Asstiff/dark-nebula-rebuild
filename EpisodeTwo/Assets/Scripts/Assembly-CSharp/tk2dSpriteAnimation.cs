using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dSpriteAnimation")]
public class tk2dSpriteAnimation : MonoBehaviour
{
	public tk2dSpriteAnimationClip[] clips;

	public int GetClipIdByName(string name)
	{
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i].name == name)
			{
				return i;
			}
		}
		return -1;
	}
}
