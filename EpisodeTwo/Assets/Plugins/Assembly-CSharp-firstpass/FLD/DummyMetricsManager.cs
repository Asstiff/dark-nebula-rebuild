using System.Collections.Generic;

namespace FLD
{
	public class DummyMetricsManager : MetricsManager
	{
		public override bool SupportsTimedEvents
		{
			get
			{
				return true;
			}
		}

		public override bool SupportsEventParameters
		{
			get
			{
				return true;
			}
		}

		public override void Initialize()
		{
		}

		public override void LogEvent(string pLogEvent)
		{
		}

		public override void LogEvent(string pLogEvent, Dictionary<string, string> pParameters)
		{
		}

		public override void BeginTimedEvent(string pLogEvent, Dictionary<string, string> pParameters)
		{
		}

		public override void BeginTimedEvent(string pLogEvent)
		{
		}

		public override void EndTimedEvent(string pLogEvent)
		{
		}
	}
}
