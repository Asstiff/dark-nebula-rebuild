public class HUDMessageEntity : Entity
{
	private FlagConditionChanged condition;

	private void Start()
	{
		condition = new FlagConditionChanged(GetParameterInt("listenFlag"));
		condition.Callback += onCondition;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (condition != null)
		{
			condition.Callback -= onCondition;
			condition.Dispose();
			condition = null;
		}
	}

	private void onCondition()
	{
		string text = string.Empty;
		switch (GetParameterInt("Message"))
		{
		case 0:
			text = Language.Get("Excellent!");
			break;
		case 1:
			text = Language.Get("Perfect!");
			break;
		case 2:
			text = Language.Get("Superb!");
			break;
		case 3:
			text = Language.Get("Outstanding!");
			break;
		}
		if (text != string.Empty)
		{
			World.hud.ShowMessage(text);
		}
	}
}
