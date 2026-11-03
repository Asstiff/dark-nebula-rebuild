using System.Collections.Generic;

public class TaskContainer : Task
{
	public List<Task> Tasks = new List<Task>();

	public void AddTask(Task newTask)
	{
		Tasks.Add(newTask);
	}

	public List<string> GetMusic()
	{
		List<string> list = new List<string>();
		foreach (Task task in Tasks)
		{
			if (task is TaskContainer)
			{
				list.AddRange((task as TaskContainer).GetMusic());
			}
			else if (task is MusicAction)
			{
				list.Add((task as MusicAction).name);
			}
		}
		return list;
	}

	public override string ToString()
	{
		string text = "[TaskContainer]";
		foreach (Task task in Tasks)
		{
			text = text + "\n" + task.ToString();
		}
		return text;
	}

	public List<T> GetTasks<T>() where T : class
	{
		foreach (Task task in Tasks)
		{
			if (task is TaskContainer)
			{
				return (task as TaskContainer).GetTasks<T>();
			}
			if (task is T)
			{
				List<T> list = new List<T>();
				list.Add(task as T);
				return list;
			}
		}
		return new List<T>();
	}
}
