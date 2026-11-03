using UnityEngine;

public class ScriptLoader
{
	private ScriptManager scriptManager;

	public ScriptLoader(ScriptManager pScriptManager)
	{
		scriptManager = pScriptManager;
	}

	public void Dispose()
	{
		scriptManager = null;
	}

	public ScriptLoader LoadScripts(int levelNum)
	{
		string sceneName = UserSettings.I.GetSceneName(levelNum);
		DataInputStream dataInputStream = DataStreamManager.LoadResourceStream(sceneName + "-script", 256);
		uint num = dataInputStream.ReadByte();
		for (int i = 0; i < num; i++)
		{
			string scriptName = dataInputStream.ReadString();
			LoadScriptFromStream(scriptName, dataInputStream);
		}
		dataInputStream.Close();
		return this;
	}

	private void LoadScriptFromStream(string scriptName, DataInputStream stream)
	{
		Task task = NewTaskFromStream(stream);
		Script script = new Script(scriptName, task);
		scriptManager.AddScript(script);
	}

	private Task NewTaskFromType(uint type)
	{
		Task result = null;
		switch (type)
		{
		case 1u:
			result = new MovieAction();
			break;
		case 2u:
			result = new MusicAction();
			break;
		case 3u:
			result = new Sequence();
			break;
		case 4u:
			result = new ImageAction();
			break;
		case 5u:
			result = new FlagAction();
			break;
		case 6u:
			result = new WaitAction();
			break;
		case 7u:
			result = new HelpScreenAction();
			break;
		default:
			Debug.LogError("Illegal task-type: " + type);
			break;
		}
		return result;
	}

	private Task NewTaskFromStream(DataInputStream stream)
	{
		uint type = stream.ReadByte();
		Task task = NewTaskFromType(type);
		LoadTaskParameterFromStream(task, stream);
		if (task is TaskContainer)
		{
			TaskContainer taskContainer = task as TaskContainer;
			uint num = stream.ReadByte();
			if (num == 0)
			{
				return task;
			}
			for (int i = 0; i < num; i++)
			{
				Task newTask = NewTaskFromStream(stream);
				taskContainer.AddTask(newTask);
			}
		}
		return task;
	}

	private void LoadTaskParameterFromStream(Task task, DataInputStream stream)
	{
		uint num = stream.ReadByte();
		for (int i = 0; i < num; i++)
		{
			string key = stream.ReadString();
			string value = stream.ReadString();
			task.SetParameter(key, value);
		}
	}
}
