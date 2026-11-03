using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public sealed class FacebookScoresManager
	{
		public class Entry
		{
			public string UserId;

			public string FullName;

			public int HighestFloor;

			private Texture2D _profilePicture;

			public Texture2D ProfilePicture
			{
				get
				{
					return _profilePicture;
				}
				set
				{
					_profilePicture = value;
					if (PortraitFetched != null)
					{
						PortraitFetched();
					}
				}
			}

			public event Action PortraitFetched;
		}

		private Entry _myself;

		private List<Entry> _allEntries = new List<Entry>();

		private Dictionary<string, Texture2D> _cachedProfilePictures = new Dictionary<string, Texture2D>();

		private DateTime _lastRefreshedScores = new DateTime(1970, 0, 0);

		private bool _downloadingProfilePictures;

		private static FacebookScoresManager _instance;

		public bool HasEntries
		{
			get
			{
				return _allEntries.Count > 0;
			}
		}

		public IList<Entry> AllEntries
		{
			get
			{
				return _allEntries;
			}
		}

		public IList<Entry> EntriesByScore
		{
			get
			{
				List<Entry> list = new List<Entry>(AllEntries);
				list.Sort((Entry a, Entry b) => b.HighestFloor.CompareTo(a.HighestFloor));
				return list;
			}
		}

		public IList<Entry> EntriesByScoreAscending
		{
			get
			{
				List<Entry> list = new List<Entry>(AllEntries);
				list.Sort((Entry a, Entry b) => a.HighestFloor.CompareTo(b.HighestFloor));
				return list;
			}
		}

		public Entry MyEntry
		{
			get
			{
				return _myself;
			}
		}

		public static FacebookScoresManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new FacebookScoresManager();
				}
				return _instance;
			}
		}

		public event Action ScoresRefreshed;

		public event Action PortraitsFetched;

		private FacebookScoresManager()
		{
			if (!Application.isEditor)
			{
			}
		}

		public void Initialize(bool pWasCancelled)
		{
		}

		public void RefreshScores()
		{
			if (!FacebookManager.Instance.HasBasicInformation)
			{
				Debug.LogWarning("Facebook: Don't have basic information - can't fetch scores");
				return;
			}
			if (_allEntries.Count > 0 && (DateTime.UtcNow - _lastRefreshedScores).TotalMinutes < 10.0)
			{
				if (ScoresRefreshed != null)
				{
					ScoresRefreshed();
				}
				return;
			}
			_lastRefreshedScores = DateTime.UtcNow;
			FacebookManager.Instance.GraphGet(string.Format("{0}/scores", FacebookManager.Instance.ApplicationId), (object pResult) =>
			{
				Hashtable hashtable;
				if ((hashtable = pResult as Hashtable) != null)
				{
					ArrayList arrayList;
					if ((arrayList = hashtable["data"] as ArrayList) != null)
					{
						foreach (object item in arrayList)
						{
							LoadScore(item as Hashtable);
						}
					}
					if (ScoresRefreshed != null)
					{
						ScoresRefreshed();
					}
				}
				else
				{
					Debug.LogWarning("Facebook: Could not retrieve scores");
				}
			});
		}

		private void LoadScore(Hashtable pData)
		{
			if (pData == null)
			{
				Debug.LogError("Facebook: Invalid data");
				return;
			}
			Hashtable hashtable = pData["user"] as Hashtable;
			if (hashtable == null)
			{
				Debug.LogError("Facebook: Missing user data");
			}
			Entry entry = new Entry();
			entry.UserId = (string)hashtable["id"];
			entry.FullName = (string)hashtable["name"];
			entry.HighestFloor = (int)(double)pData["score"];
			Entry entry2 = entry;
			bool flag = false;
			foreach (Entry allEntry in _allEntries)
			{
				if (allEntry.UserId == entry2.UserId)
				{
					allEntry.HighestFloor = entry2.HighestFloor;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				_allEntries.Add(entry2);
			}
			if (_myself == null && entry2.UserId == FacebookManager.Instance.UserId)
			{
				Debug.Log("Facebook: Identified myself: I am " + entry2.FullName);
				_myself = entry2;
			}
		}

		public void SubmitScore(int pHighestFloor)
		{
			if (FacebookManager.Instance.HasPermission("publish_actions"))
			{
				if (_myself == null)
				{
					Debug.LogWarning("Can't submit score now; users haven't been loaded");
					return;
				}
				if (pHighestFloor < _myself.HighestFloor)
				{
					Debug.Log(string.Format("Score of {0} is not enough to break record of {1}", pHighestFloor, _myself.HighestFloor));
					return;
				}
				Debug.Log("Facebook: Submitting new score of " + pHighestFloor);
				_myself.HighestFloor = pHighestFloor;
				FacebookManager.Instance.GraphPost(_myself.UserId + "/scores", new Dictionary<string, object> { 
				{
					"score",
					pHighestFloor.ToString()
				} });
			}
		}

		public void ForceSubmitScore(int pHighestFloor)
		{
			if (_myself != null)
			{
				_myself.HighestFloor = -1;
			}
			SubmitScore(pHighestFloor);
		}

		private IEnumerator DownloadProfilePictures()
		{
			if (_downloadingProfilePictures)
			{
				yield break;
			}
			_downloadingProfilePictures = true;
			List<string> uidsToFetch = new List<string>();
			foreach (Entry entry in _allEntries)
			{
				if (entry.ProfilePicture == null)
				{
					uidsToFetch.Add(entry.UserId);
				}
			}
			foreach (string uid in uidsToFetch)
			{
				Texture2D texture = null;
				if (!_cachedProfilePictures.TryGetValue(uid, out texture))
				{
					string url = string.Format("http://graph.facebook.com/{0}/picture", uid);
					Debug.Log("Fetching profile picture from " + url);
					WWW www = new WWW(url);
					yield return www;
					if (www.textureNonReadable != null)
					{
						texture = www.textureNonReadable;
						_cachedProfilePictures.Add(uid, texture);
					}
					else
					{
						Debug.LogWarning("Couldn't download profile picture for " + uid);
					}
				}
				foreach (Entry entry2 in _allEntries)
				{
					if (entry2.UserId == uid)
					{
						entry2.ProfilePicture = texture;
						Debug.Log("Loaded profile picture for " + uid + "/" + entry2.FullName);
						break;
					}
				}
			}
			_downloadingProfilePictures = false;
			if (PortraitsFetched != null)
			{
				PortraitsFetched();
			}
		}
	}
}
