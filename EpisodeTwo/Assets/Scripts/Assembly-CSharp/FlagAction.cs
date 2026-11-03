public class FlagAction : Task
{
	public int number;

	public override void Enter()
	{
		World.TriggerFlag(number);
	}

	public override bool IsDone()
	{
		return true;
	}

	public override void SetParameter(string key, string value)
	{
		switch (key)
		{
		case "id":
			number = int.Parse(value);
			break;
		default:
			base.SetParameter(key, value);
			break;
		}
	}

	public override string ToString()
	{
		return string.Format("[FlagAction].number = " + number);
	}
}
