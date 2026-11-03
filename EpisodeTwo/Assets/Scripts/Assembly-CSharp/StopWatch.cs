using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Text;
using UnityEngine;

public class StopWatch
{
	private class TimeEntry
	{
		public List<TimeEntry> Children;

		public string Name;

		public float Start;

		public float End;

		public bool HasParent;

		public float Duration
		{
			get
			{
				if (IsRunning)
				{
					Debug.LogWarning("Checking duration of something that is not stopped!");
					return Time.realtimeSinceStartup - Start;
				}
				return End - Start;
			}
		}

		public bool IsRunning
		{
			get
			{
				return Start > End;
			}
		}

		public void AddChild(TimeEntry child)
		{
			if (!child.HasParent)
			{
				child.HasParent = true;
				if (Children == null)
				{
					Children = new List<TimeEntry>();
				}
				Children.Add(child);
			}
		}

		public string ToString(string pPrefix, float pTotal)
		{
			StringBuilder stringBuilder = new StringBuilder();
			float percent = Duration / pTotal;
			stringBuilder.Append(pPrefix + Name + " " + percent.ToString("P"));
			bool flag = false;
			if (Children != null)
			{
				foreach (TimeEntry child in Children)
				{
					if (!flag)
					{
						flag = true;
						float percent2 = (Duration - GetChildDuration()) / pTotal;
						stringBuilder.AppendLine(" (" + percent2.ToString("P") + ")" + ToStars(percent2));
					}
					stringBuilder.Append(child.ToString(pPrefix + " ", pTotal));
				}
			}
			if (!flag)
			{
				stringBuilder.AppendLine(ToStars(percent));
			}
			return stringBuilder.ToString();
		}

		private string ToStars(float percent)
		{
			return "\t\t\t" + new string('*', (int)(percent * 10f));
		}

		private float GetChildDuration()
		{
			if (Children == null)
			{
				return 0f;
			}
			float num = 0f;
			foreach (TimeEntry child in Children)
			{
				num += child.Duration;
			}
			return num;
		}
	}

	private static StopWatch _current;

	private Dictionary<string, TimeEntry> Entries;

	private List<TimeEntry> CallStack;

	public static StopWatch Current
	{
		get
		{
			if (_current == null)
			{
				_current = new StopWatch();
			}
			return _current;
		}
		set
		{
			_current = value;
		}
	}

	[Conditional("ENABLE_PROFILER")]
	public void Reset()
	{
		Entries = new Dictionary<string, TimeEntry>();
		CallStack = new List<TimeEntry>();
	}

	[Conditional("ENABLE_PROFILER")]
	public void Start(string pName)
	{
		TimeEntry timeEntry = null;
		if (Entries.ContainsKey(pName))
		{
			timeEntry = Entries[pName];
			if (timeEntry.IsRunning)
			{
				Debug.LogWarning("StopWatch already running " + pName);
				return;
			}
			timeEntry.Start = Time.realtimeSinceStartup - timeEntry.Duration;
			timeEntry.End = -1f;
		}
		else
		{
			TimeEntry timeEntry2 = new TimeEntry();
			timeEntry2.Name = pName;
			timeEntry2.Start = Time.realtimeSinceStartup;
			timeEntry2.End = -1f;
			timeEntry = timeEntry2;
			Entries.Add(pName, timeEntry);
		}
		if (CallStack.Count > 0)
		{
			CallStack[CallStack.Count - 1].AddChild(timeEntry);
		}
		CallStack.Add(timeEntry);
	}

	[Conditional("ENABLE_PROFILER")]
	public void Stop()
	{
		if (CallStack.Count == 0)
		{
			Debug.LogWarning("StopWatch.Stop() there is nothing in the RunningStack");
		}
	}

	[Conditional("ENABLE_PROFILER")]
	public void Stop(string pName)
	{
		if (!Entries.ContainsKey(pName))
		{
			Debug.LogWarning("StopWatch.Stop() can't find " + pName);
		}
	}

	[Conditional("ENABLE_PROFILER")]
	private void Stop(TimeEntry entry)
	{
		entry.End = Time.realtimeSinceStartup;
		if (CallStack[CallStack.Count - 1] == entry)
		{
			CallStack.RemoveAt(CallStack.Count - 1);
		}
		else
		{
			Debug.LogWarning("Removing an entry not at the top of the stack! " + entry.Name);
		}
	}

	[Conditional("ENABLE_PROFILER")]
	public void StopAll()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		foreach (KeyValuePair<string, TimeEntry> entry in Entries)
		{
			if (entry.Value.IsRunning)
			{
				entry.Value.End = realtimeSinceStartup;
			}
		}
		CallStack = new List<TimeEntry>();
	}

	public string GetReport()
	{
		float num = 0f;
		foreach (KeyValuePair<string, TimeEntry> entry in Entries)
		{
			if (!entry.Value.HasParent)
			{
				num += entry.Value.Duration;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("=== StopWatch " + num + " sec ===");
		foreach (KeyValuePair<string, TimeEntry> entry2 in Entries)
		{
			TimeEntry value = entry2.Value;
			if (!value.HasParent)
			{
				stringBuilder.AppendLine(value.ToString(string.Empty, num));
			}
		}
		stringBuilder.AppendLine("=============");
		return stringBuilder.ToString();
	}
}
