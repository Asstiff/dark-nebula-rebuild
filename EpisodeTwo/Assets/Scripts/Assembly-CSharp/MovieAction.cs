public class MovieAction : Task
{
	private string name;

	private bool allowedToSkip;

	public override void Enter()
	{
	}

	public override bool IsDone()
	{
		return true;
	}

	public override string ToString()
	{
		return string.Format("[MovieAction].name = " + name + " allowerdToSkip = " + allowedToSkip);
	}

	public override void SetParameter(string key, string value)
	{
		switch (key)
		{
		case "name":
			name = value;
			break;
		case "allowedToSkip":
			allowedToSkip = bool.Parse(value);
			break;
		default:
			base.SetParameter(key, value);
			break;
		}
	}
}
