using System.Collections.Generic;
using UnityEngine;

public class AvatarManager
{
	private static AvatarManager _instance;

	private Avatar _activeAvatar;

	private List<Avatar> _allAvatars = new List<Avatar>();

	public static AvatarManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AvatarManager();
			}
			return _instance;
		}
	}

	public Avatar ActiveAvatar
	{
		get
		{
			if (_activeAvatar == null)
			{
				_activeAvatar = GetActiveAvatarFromPlayerPrefs();
				UserSettings.Instance.ActiveAvatarId = _activeAvatar.ID;
			}
			return _activeAvatar;
		}
		set
		{
			if (value == null || !value.Owned) return;
			_activeAvatar = value;
			UserSettings.Instance.ActiveAvatarId = _activeAvatar.ID;
            FLD.PersistentStore.GetInstance("DN1UserSettings").Save(); PlayerPrefs.Save();
		}
	}

	public List<Avatar> AllAvatars
	{
		get
		{
			if (_allAvatars.Count == 0)
			{
				Object[] array = Resources.LoadAll("avatars");
				Object[] array2 = array;
				foreach (Object obj in array2)
				{
					_allAvatars.Add((Avatar)obj);
				}
			}
			return _allAvatars;
		}
	}

	public static void UnlockAvatar(Avatar pAvatar)
	{
		UserPurchases.Instance.GiveProduct(pAvatar.ID);
	}

	public void UnlockAllAvatars()
	{
		foreach (Avatar allAvatar in AllAvatars)
		{
			UnlockAvatar(allAvatar);
		}
	}

	public static bool Have(string pAvatarID)
	{
		return UserPurchases.Instance.HasProduct(pAvatarID);
	}

	private Avatar GetActiveAvatarFromPlayerPrefs()
	{
		foreach (Avatar allAvatar in AllAvatars)
		{
			if (UserSettings.Instance.ActiveAvatarId == allAvatar.ID && allAvatar.Owned)
			{
				return allAvatar;
			}
		}
		return GetDefaultAvatar();
	}

	private Avatar GetDefaultAvatar()
	{
		foreach (Avatar allAvatar in AllAvatars)
		{
			if (allAvatar.DefaultAvatar)
			{
				return allAvatar;
			}
		}
		return null;
	}
}
