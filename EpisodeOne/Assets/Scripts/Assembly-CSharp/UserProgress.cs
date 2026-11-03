using FLD;
using Palringo;
using UnityEngine;

public class UserProgress
{
	private const string LegacyStorageInstanceKey = "DN1UserSettings";

	public const int StarBronze = 1;

	public const int StarSilver = 2;

	public const int StarGold = 3;

	private const string LevelStarStorageKey = "LevelStar";

	private const string BestTimeStorageKey = "BestTime";

	private const string SpawnHardCurrencyStorageKey = "HCSpawnChance";

	private static UserProgress _instance;

	public static UserProgress Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new UserProgress();
			}
			return _instance;
		}
	}

	public int HCSpawnChance
	{
		get
		{
			int? customInteger = OfflineUserData.Instance.GetCustomInteger("HCSpawnChance");
			int num = (customInteger.HasValue ? customInteger.Value : 0);
			if (num == 0)
			{
				num = CustomBuildVariables<CustomBuildVars>.Instance.HCSpawnChancePercent;
			}
			return num;
		}
		set
		{
			if (value > CustomBuildVariables<CustomBuildVars>.Instance.HCSpawnChancePercentMinimum)
			{
				Debug.Log("setting spawn chance to " + value);
				OfflineUserData.Instance.StoreCustomData("HCSpawnChance", value);
			}
			else
			{
				Debug.Log("Spawn chance at bottom");
			}
		}
	}

	public UserProgress()
	{
		MigrateLegacyData();
	}

	public void Init()
	{
	}

	private void MigrateLegacyData()
	{
		PersistentStore instance = PersistentStore.GetInstance("DN1UserSettings");
		OfflineUserData userData = OfflineUserData.Instance;
		for (int i = 1; i <= 23; i++)
		{
			string starLevelStorageKey = GetStarLevelStorageKey(i, false);
			if (instance.Get(starLevelStorageKey) != null)
			{
				Debug.Log("Migrating " + starLevelStorageKey);
				userData.StoreCustomData(starLevelStorageKey, instance.GetInt(starLevelStorageKey).ToString());
				instance.Remove(starLevelStorageKey);
			}
			starLevelStorageKey = GetStarLevelStorageKey(i, true);
			if (instance.Get(starLevelStorageKey) != null)
			{
				Debug.Log("Migrating " + starLevelStorageKey);
				userData.StoreCustomData(starLevelStorageKey, instance.GetInt(starLevelStorageKey).ToString());
				instance.Remove(starLevelStorageKey);
			}
		}
		for (int j = 1; j <= 23; j++)
		{
			string bestTimeStorageKey = GetBestTimeStorageKey(j, false);
			if (instance.Get(bestTimeStorageKey) != null)
			{
				userData.StoreCustomData(bestTimeStorageKey, instance.GetFloat(bestTimeStorageKey));
				instance.Remove(bestTimeStorageKey);
			}
			bestTimeStorageKey = GetBestTimeStorageKey(j, true);
			if (instance.Get(bestTimeStorageKey) != null)
			{
				userData.StoreCustomData(bestTimeStorageKey, instance.GetFloat(bestTimeStorageKey));
				instance.Remove(bestTimeStorageKey);
			}
		}
		if (instance.Get("HCSpawnChance") != null)
		{
			userData.StoreCustomData("HCSpawnChance", instance.GetInt("HCSpawnChance"));
		}
	}

	public bool IsTotalTimeUnlocked(bool pExpert)
	{
		return GetLevelStar(23, pExpert) > 0;
	}

	public bool HasCompletedAllLevelsInAnyDifficulty()
	{
		if (IsTotalTimeUnlocked(false) || IsTotalTimeUnlocked(true))
		{
			return true;
		}
		return false;
	}

	public void ResetProgress()
	{
		for (int i = 1; i <= 23; i++)
		{
			OfflineUserData.Instance.DeleteCustomData(GetStarLevelStorageKey(i, false));
			OfflineUserData.Instance.DeleteCustomData(GetStarLevelStorageKey(i, true));
			OfflineUserData.Instance.DeleteCustomData(GetBestTimeStorageKey(i, false));
			OfflineUserData.Instance.DeleteCustomData(GetBestTimeStorageKey(i, true));
		}
		OfflineUserData.Instance.DeleteCustomData("HCSpawnChance");
		LocalLeaderboard.ResetLeaderboards();
	}

	public int GetLevelStar(int levelNum, bool expert)
	{
		string starLevelStorageKey = GetStarLevelStorageKey(levelNum, expert);
		int result;
		if (OfflineUserData.Instance.HasCustomData(starLevelStorageKey) && int.TryParse(OfflineUserData.Instance.GetCustomData(starLevelStorageKey), out result))
		{
			return result;
		}
		return 0;
	}

	public bool IsLevelStarGold(int levelNum, bool expert)
	{
		return GetLevelStar(levelNum, expert) == 3;
	}

	public void SetLevelStar(int levelNum, int starValue, bool expert)
	{
		OfflineUserData.Instance.StoreCustomData(GetStarLevelStorageKey(levelNum, expert), starValue.ToString());
	}

	public bool GetIsAllGold(int pFirstLevel, int pLastLevel, bool pExpert)
	{
		for (int i = pFirstLevel; i <= pLastLevel; i++)
		{
			if (GetLevelStar(i, pExpert) != 3)
			{
				return false;
			}
		}
		return true;
	}

	public bool HaveCompletedLevel(int pLevel, bool pIsExpert)
	{
		return GetLevelStar(pLevel, pIsExpert) > 0;
	}

	public int GetMaxSelectableLevel(bool pIsExpert)
	{
		return GetHighestFinishedLevel(pIsExpert) + 1;
	}

	private string GetStarLevelStorageKey(int pLevel, bool pIsExpert)
	{
		return string.Format("{0}{1}-{2}", "LevelStar", pLevel, pIsExpert);
	}

	private string GetBestTimeStorageKey(int pLevel, bool pIsExpert)
	{
		return string.Format("{0}{1}-{2}", "BestTime", pLevel, pIsExpert);
	}

	public int GetHighestFinishedLevel(bool pIsExpert)
	{
		for (int i = 1; i <= 23; i++)
		{
			if (GetLevelStar(i, pIsExpert) == 0)
			{
				return i - 1;
			}
		}
		return 23;
	}

	public float GetBestTime(int levelNum, bool expert)
	{
		float? customFloat = OfflineUserData.Instance.GetCustomFloat(GetBestTimeStorageKey(levelNum, expert));
		return customFloat.HasValue && LocalLeaderboard.IsValidTime(customFloat.Value) ? customFloat.Value : 0f;
	}

	public void SetBestTime(float time, int levelNum, bool expert)
	{
		if (LocalLeaderboard.IsValidTime(time)) OfflineUserData.Instance.StoreCustomData(GetBestTimeStorageKey(levelNum, expert), time);
	}

	public void RemoveBestTime(int levelNum, bool expert)
	{
		OfflineUserData.Instance.DeleteCustomData(GetBestTimeStorageKey(levelNum, expert));
	}

	public float GetTotalTime(bool pIsExpert)
	{
		float num = 0f;
		for (int i = 1; i <= 23; i++)
		{
			num += GetBestTime(i, pIsExpert);
		}
		return num;
	}

	public void SetRawLocalLeaderboard(string pName, string value)
	{
		OfflineUserData.Instance.StoreCustomData(pName, value);
	}

	public string GetRawLocalLeaderboard(string pName, string pDefault = null)
	{
		if (OfflineUserData.Instance.HasCustomData(pName)) return OfflineUserData.Instance.GetCustomData(pName);
		PersistentStore legacy = PersistentStore.GetInstance(LegacyStorageInstanceKey);
		return legacy.Get(pName) != null ? legacy.GetString(pName) : pDefault;
	}

	public bool GetHasAnyProgress()
	{
		int num = 23;
		for (int i = 1; i <= num; i++)
		{
			if (GetLevelStar(i, false) != 0)
			{
				return true;
			}
			if (GetLevelStar(i, true) != 0)
			{
				return true;
			}
		}
		return false;
	}
}
