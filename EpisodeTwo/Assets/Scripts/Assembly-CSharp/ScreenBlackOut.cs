using UnityEngine;

public class ScreenBlackOut : ScreenWhiteOut
{
	private void Awake()
	{
		color = Color.black;
	}
}
