using System;
using FLD;

public class UserPurchases
{
	private const string LegacyStorageInstanceKey = "DN1UserSettings";

	private const string StorageInstanceKey = "Dn1Purchases";

	private const string UnlockAllKey = "UnlockedAll";

	private const string PurchasedIAPKey = "PurchasedIAP";

	private static UserPurchases _instance;

	private PersistentStore _storage;

	public static UserPurchases Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new UserPurchases();
			}
			return _instance;
		}
	}

	public bool HasUnlockAll
	{
		get
		{
			return HasProduct("UnlockedAll");
		}
		set
		{
			if (value)
			{
				GiveProduct("UnlockedAll");
			}
			else
			{
				RemoveProduct("UnlockedAll");
			}
		}
	}

	public bool HasPurchasedIAP
	{
		get
		{
			return _storage.GetBool("PurchasedIAP");
		}
		set
		{
			_storage.Set("PurchasedIAP", value);
		}
	}

	public UserPurchases()
	{
		_storage = PersistentStore.GetInstance("DN1UserSettings");
		MigrateLegacyData();
	}

	public void Init()
	{
	}

	private void MigrateLegacyData()
	{
		PersistentStore instance = PersistentStore.GetInstance("DN1UserSettings");
		_storage.Set("PurchasedIAP", instance.GetBool("PurchasedIAP"));
		if (instance.Get("UnlockedAll") != null)
		{
			_storage.Set("UnlockedAll", true);
		}
		foreach (Avatar allAvatar in AvatarManager.Instance.AllAvatars)
		{
			if (instance.Get(allAvatar.ID) != null)
			{
				_storage.Set(allAvatar.ID, true);
			}
		}
		for (int i = 1; i <= 23; i++)
		{
			foreach (int value in Enum.GetValues(typeof(Booster.BoosterType)))
			{
				string idForBooster = Booster.GetIdForBooster((Booster.BoosterType)value, i, false);
				string idForBooster2 = Booster.GetIdForBooster((Booster.BoosterType)value, i, true);
				if (instance.Get(idForBooster) != null)
				{
					_storage.Set(idForBooster, true);
				}
				if (instance.Get(idForBooster2) != null)
				{
					_storage.Set(idForBooster2, true);
				}
			}
		}
	}

	public bool HasProduct(string pID)
	{
		return _storage.Get("UnlockedAll") != null || _storage.Get(pID) != null;
	}

	public void GiveProduct(string pID)
	{
		_storage.Set(pID, true);
	}

	public void RemoveProduct(string pID)
	{
		_storage.Remove(pID);
	}
}
