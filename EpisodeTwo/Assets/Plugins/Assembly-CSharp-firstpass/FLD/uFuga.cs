using System;
using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public static class uFuga
	{
		public enum BuildProfileId
		{
			WebPlayerGeneric = 0,
			iOSDebug = 1
		}

		public static class BuildVariables
		{
		}

		public class StorageCollection
		{
			public PersistentStorage platform = new PersistentStorage("uFugaPlatform", false);

			public PersistentStorage settings = new PersistentStorage("uFugaSettings", true);

			public PersistentStorage statistics = new PersistentStorage("uFugaStatistics", true);

			public PersistentStorage achievements = new PersistentStorage("uFugaAchievements", true);

			public PersistentStorage leaderboards = new PersistentStorage("uFugaLeaderboards", true);

			public PersistentStorage persistent = new PersistentStorage("uFugaPersistent", true);

			public PersistentStorage temporary = new PersistentStorage("uFugaTemporary", false);

			public PersistentStorage purchases = new PersistentStorage("uFugaPurchases", true);

			public Dictionary<string, PersistentStorage> GetAll()
			{
				Dictionary<string, PersistentStorage> dictionary = new Dictionary<string, PersistentStorage>();
				dictionary.Add("platform", platform);
				dictionary.Add("settings", settings);
				dictionary.Add("statistics", statistics);
				dictionary.Add("achievements", achievements);
				dictionary.Add("leaderboards", leaderboards);
				dictionary.Add("persistent", persistent);
				dictionary.Add("temporary", temporary);
				dictionary.Add("purchases", purchases);
				return dictionary;
			}
		}

		public const BuildProfileId ActiveProfile = BuildProfileId.WebPlayerGeneric;

		public static readonly VersionNumber Version = new VersionNumber
		{
			Minor = 2,
			Revision = 2
		};

		private static uFugaBehavior _instance = null;

		private static StorageCollection _storage;

		public static uFugaBehavior Instance
		{
			get
			{
				if (object.ReferenceEquals(_instance, null))
				{
					GameObject gameObject = new GameObject("uFuga");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					_instance = gameObject.AddComponent<uFugaBehavior>();
					Debug.Log(string.Format("uFuga {0} initialized", Version.ToString()));
				}
				return _instance;
			}
		}

		public static StorageCollection Storage
		{
			get
			{
				if (_storage == null)
				{
					_storage = new StorageCollection();
				}
				return _storage;
			}
		}

		public static event Action<bool> ApplicationPausedOrResumed;

		public static event Action ApplicationPaused;

		public static event Action ApplicationResumed;

		public static event Action ApplicationQuit;

		public static void SignalApplicationPause(bool pPaused)
		{
			if (ApplicationPausedOrResumed != null)
			{
				ApplicationPausedOrResumed(pPaused);
			}
			if (pPaused && ApplicationPaused != null)
			{
				ApplicationPaused();
			}
			if (pPaused && ApplicationResumed != null)
			{
				ApplicationResumed();
			}
		}

		public static void SignalApplicationQuit()
		{
			if (ApplicationQuit != null)
			{
				ApplicationQuit();
			}
			_instance = null;
		}
	}
}
