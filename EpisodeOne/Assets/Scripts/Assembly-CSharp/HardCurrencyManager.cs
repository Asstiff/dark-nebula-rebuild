using System;
using FLD;
using UnityEngine;

public class HardCurrencyManager
{
	private const string HCStorageKey = "HardCurrency";

	private const string HCInstanceKey = "HCLocal";

	private static HardCurrencyManager _instance;

	public Action BalanceChangedEvent;

	private PersistentStore _storage;

	public static HardCurrencyManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new HardCurrencyManager();
			}
			return _instance;
		}
	}

	public int Amount
	{
		get
		{
			return _storage.GetInt("HardCurrency");
		}
	}

	public HardCurrencyManager()
	{
		_storage = PersistentStore.GetInstance("HCLocal");
		CheckForInitialHardCurrency();
	}

	public bool Spend(int pAmount)
	{
		if (pAmount <= 0 || pAmount > Amount)
		{
			return false;
		}
		_storage.Set("HardCurrency", Amount - pAmount);
        _storage.Save(); PlayerPrefs.Save();
		if (BalanceChangedEvent != null)
		{
			BalanceChangedEvent();
		}
		return true;
	}

	public void Earn(int pAmount)
	{
		if (pAmount <= 0) return;
		int pValue = (int)Math.Min(int.MaxValue, (long)Amount + pAmount);
		_storage.Set("HardCurrency", pValue);
        _storage.Save(); PlayerPrefs.Save();
		if (BalanceChangedEvent != null)
		{
			BalanceChangedEvent();
		}
	}

	private void CheckForInitialHardCurrency()
	{
		object obj = _storage.Get("HardCurrency");
		if (obj == null)
		{
			Earn(CustomBuildVariables<CustomBuildVars>.Instance.StartupHC);
			Debug.Log("No values found for player, awarding startup hard currency");
		}
		else
		{
			Debug.Log("HC already found for player. No more cash awarded");
		}
	}
}
