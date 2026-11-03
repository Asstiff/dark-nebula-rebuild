public class StateMachine
{
	private StateMachineState currentState;

	private StateMachineState nextState;

	public StateMachine(StateMachineState state)
	{
		currentState = state;
		nextState = null;
	}

	public void UpdateWithDt(float dt)
	{
		if (nextState != null)
		{
			currentState.Exit();
			currentState = nextState;
			nextState = null;
		}
		StateMachineState stateMachineState = currentState.UpdateWithDt(dt);
		if (stateMachineState != currentState)
		{
			nextState = stateMachineState;
		}
	}

	public StateMachineState CurrentState()
	{
		return currentState;
	}

	public string CurrentStateAsString()
	{
		return currentState.ToString();
	}
}
