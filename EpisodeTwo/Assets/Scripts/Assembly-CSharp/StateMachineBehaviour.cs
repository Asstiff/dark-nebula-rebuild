using UnityEngine;

public class StateMachineBehaviour : MonoBehaviour
{
	public Enemy Host;

	private void Update()
	{
		if (!Host.IsPooled && !Host.IsFrozen)
		{
			Host.state.UpdateWithDt(Time.deltaTime);
		}
	}
}
