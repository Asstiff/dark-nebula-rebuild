public abstract class StateMachineState
{
	protected StateMachine machine;

	public abstract void Exit();

	public abstract StateMachineState UpdateWithDt(float dt);
}
