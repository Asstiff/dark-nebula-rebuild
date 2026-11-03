public class BattlePuckAggressive : BattlePuckState
{
	public BattlePuckAggressive(BattlePuck p)
		: base(p)
	{
	}

	public override StateMachineState UpdateWithDt(float dt)
	{
		base.UpdateWithDt(dt);
		if (battlePuck.hasLightEntity)
		{
			battlePuck.SetChildedLight(true);
		}
		return this;
	}
}
