using UnityEngine;

public class ImageAction : Task
{
	public string name;

	public override void Enter()
	{
		Hud.Instance.ImageScreen.Show(name);
		Time.timeScale = 0f;
	}

	public override void Exit()
	{
		Time.timeScale = 1f;
	}

	public override bool IsDone()
	{
		return Hud.Instance.ImageScreen.IsDone();
	}

	public override string ToString()
	{
		return string.Format("[ImageAction].name = " + name);
	}

	public override void SetParameter(string key, string value)
	{
		switch (key)
		{
		case "name":
			name = value;
			SetImage();
			break;
		default:
			base.SetParameter(key, value);
			break;
		}
	}

	private void SetImage()
	{
	}
}
