using System.Collections.Generic;

public class Script
{
	private string name;

	private Task root;

	public Script(string _name, Task task)
	{
		name = _name;
		root = task;
	}

	public Task GetTaskRoot()
	{
		return root;
	}

	public void Update()
	{
		root.Update();
	}

	public void SkipSequence()
	{
		Sequence sequence = (Sequence)root;
		sequence.NextTask();
		sequence.NextTask();
	}

	public bool IsDone()
	{
		return root.IsDone();
	}

	public string GetName()
	{
		return name;
	}

	public List<string> GetMusic()
	{
		if (root is TaskContainer)
		{
			return (root as TaskContainer).GetMusic();
		}
		if (root is MusicAction)
		{
			List<string> list = new List<string>();
			list.Add((root as MusicAction).name);
			return list;
		}
		return new List<string>();
	}

	public List<T> GetTasks<T>() where T : class
	{
		if (root is TaskContainer)
		{
			return (root as TaskContainer).GetTasks<T>();
		}
		if (root is T)
		{
			List<T> list = new List<T>();
			list.Add(root as T);
			return list;
		}
		return new List<T>();
	}

	public override string ToString()
	{
		string text = "[Script " + name + "] { ";
		return text + root.ToString() + " }";
	}
}
