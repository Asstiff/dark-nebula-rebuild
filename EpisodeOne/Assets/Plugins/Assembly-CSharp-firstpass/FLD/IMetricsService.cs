using System.Collections.Generic;

namespace FLD
{
	[Service("Metrics", Description = "")]
	public interface IMetricsService : IService
	{
		bool SupportsTimedEvents { get; }

		bool SupportsEventParameters { get; }

		void LogEvent(string pLogEvent);

		void LogEventWithParameters(string pLogEvent, Dictionary<string, string> pParameters);

		void BeginTimedEvent(string pLogEvent);

		void BeginTimedEventWithParameters(string pLogEvent, Dictionary<string, string> pParameters);

		void EndTimedEvent(string pLogEvent);
	}
}
