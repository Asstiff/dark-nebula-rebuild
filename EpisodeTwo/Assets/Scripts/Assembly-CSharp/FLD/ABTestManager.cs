using System;
using System.Collections.Generic;

namespace FLD
{
	public sealed class ABTestManager
	{
		private static ABTestManager _instance;

		public string DeviceIdentifier
		{
			get
			{
				return DeviceIdentity.UniqueIdentifier;
			}
		}

		public string CohortKey
		{
			get
			{
				return LiveSettingsManager.Instance.ABCohort;
			}
		}

		public static ABTestManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ABTestManager();
				}
				return _instance;
			}
		}

		private ABTestManager()
		{
			LiveSettingsManager instance = LiveSettingsManager.Instance;
			instance.SettingsUpdated = (Action)Delegate.Combine(instance.SettingsUpdated, new Action(OnSettingsUpdated));
			uFuga.ApplicationPausedOrResumed += OnApplicationPaused;
			LogCohort();
		}

		private void OnApplicationPaused(bool pPaused)
		{
			if (!pPaused)
			{
				LogCohort();
			}
		}

		private void OnSettingsUpdated()
		{
			LogCohort();
		}

		private void LogCohort()
		{
			if (!string.IsNullOrEmpty(CohortKey))
			{
				DarkNebulaMetrics.LogEvent("ABTest", new Dictionary<string, string> { { "cohort", CohortKey } }, DarkNebulaMetrics.StandardParams.None);
			}
		}
	}
}
