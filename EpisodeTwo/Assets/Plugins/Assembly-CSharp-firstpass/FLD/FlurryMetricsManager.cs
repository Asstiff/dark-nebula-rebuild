using System;
using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public class FlurryMetricsManager : MetricsManager
	{
		private List<Action> _deferredCalls = new List<Action>();

		private bool _isInitialized;

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

		private void DispatchDeferredCalls()
		{
			foreach (Action deferredCall in _deferredCalls)
			{
				deferredCall();
			}
			_deferredCalls.Clear();
		}

		public override void Initialize()
		{
			FlurryAdapter.Instance.Equals(null);
			uFuga.Instance.Delay(1f, () =>
			{
				_isInitialized = true;
				DispatchDeferredCalls();
			});
		}

		public override void LogEvent(string pLogEvent)
		{
			if (!_isInitialized)
			{
				_deferredCalls.Add(() =>
				{
					LogEvent(pLogEvent);
				});
			}
			else
			{
				FlurryAdapter.Instance.LogEvent(pLogEvent);
			}
		}

		public override void LogEvent(string pLogEvent, Dictionary<string, string> pParameters)
		{
			if (!_isInitialized)
			{
				_deferredCalls.Add(() =>
				{
					LogEvent(pLogEvent, pParameters);
				});
				return;
			}
			if (pParameters.Count > 10)
			{
				Debug.LogError("More than 10 parameters sent with a metric event - unsupported by Flurry!");
			}
			FlurryAdapter.Instance.LogEvent(pLogEvent, pParameters);
		}

		public override void BeginTimedEvent(string pLogEvent, Dictionary<string, string> pParameters)
		{
			if (!_isInitialized)
			{
				_deferredCalls.Add(() =>
				{
					BeginTimedEvent(pLogEvent, pParameters);
				});
				return;
			}
			if (pParameters.Count > 10)
			{
				Debug.LogError("More than 10 parameters sent with a metric event - unsupported by Flurry!");
			}
			FlurryAdapter.Instance.LogEvent(pLogEvent, pParameters, true);
		}

		public override void BeginTimedEvent(string pLogEvent)
		{
			if (!_isInitialized)
			{
				Debug.LogWarning("Tried to call BeginTimedEvent before initializing Flurry - deferring call");
				_deferredCalls.Add(() =>
				{
					BeginTimedEvent(pLogEvent);
				});
			}
			else
			{
				FlurryAdapter.Instance.LogEvent(pLogEvent, true);
			}
		}

		public override void EndTimedEvent(string pLogEvent)
		{
			FlurryAdapter.Instance.EndTimedEvent(pLogEvent);
		}
	}
}
