public class Sequence : TaskContainer
{
	private int activeNode;

	private Task activeTask;

	public Sequence()
	{
		activeNode = -1;
		activeTask = null;
	}

	public override void Update()
	{
		if (activeTask == null)
		{
			NextTask();
			if (activeTask == null)
			{
				return;
			}
		}
		activeTask.Update();
		if (activeTask.IsDone())
		{
			activeTask.Exit();
			NextTask();
		}
	}

	public void NextTask()
	{
		if (activeNode + 1 >= Tasks.Count)
		{
			activeTask = null;
			return;
		}
		activeNode++;
		activeTask = Tasks[activeNode];
		activeTask.Enter();
	}

	public override bool IsDone()
	{
		int count = Tasks.Count;
		return activeNode == count;
	}
}
