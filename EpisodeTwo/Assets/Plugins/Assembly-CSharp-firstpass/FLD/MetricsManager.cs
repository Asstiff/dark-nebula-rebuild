using System.Collections.Generic;

namespace FLD
{
	public abstract class MetricsManager
	{
		public abstract bool SupportsTimedEvents { get; }

		public abstract bool SupportsEventParameters { get; }

		public abstract void Initialize();

		public abstract void LogEvent(string pLogEvent);

		public abstract void LogEvent(string pLogEvent, Dictionary<string, string> pParameters);

		public abstract void BeginTimedEvent(string pLogEvent, Dictionary<string, string> pParameters);

		public abstract void BeginTimedEvent(string pLogEvent);

		public abstract void EndTimedEvent(string pLogEvent);
	}
}
