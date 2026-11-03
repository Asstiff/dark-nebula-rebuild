public class MusicAction : Task
{
	public string name;

	private int repeatcount;

	public override void Enter()
	{
		World.PlayMusic(name, repeatcount);
	}

	public override bool IsDone()
	{
		return true;
	}

	public override string ToString()
	{
		return string.Format("[MusicAction].name = " + name);
	}

	public override void SetParameter(string key, string value)
	{
		switch (key)
		{
		case "name":
			name = value;
			break;
		case "repeatcount":
			repeatcount = int.Parse(value);
			break;
		default:
			base.SetParameter(key, value);
			break;
		}
	}
}
