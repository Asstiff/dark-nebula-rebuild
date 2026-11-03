using FLD;
using Palringo;
using UnityEngine;

public class ContentGatingManager
{
	private const string PersistentStoreId = "dnep1Gates";

	public const int KeyReputationLevel = 5;

	public const int HardCurrencyUnlockReward = 25;

	private static readonly int[] ContentGates = new int[2] { 9, 16 };

	private float _lastKnownReputation;

	private int _unlockedGateCount;

	private static ContentGatingManager _instance;

	public int GateKeyCount
	{
		get
		{
			int num = 0;
			num += GetPurchasedKeysCount();
			if (DarkNebulaPalringo.IsAuthorized)
			{
				num++;
				if (ProgressToReputationGateKey >= 1f)
				{
					num++;
				}
			}
			return num;
		}
	}

	public float ProgressToReputationGateKey
	{
		get
		{
			return Mathf.Clamp01(_lastKnownReputation / 5f);
		}
	}

	public static ContentGatingManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ContentGatingManager();
			}
			return _instance;
		}
	}

	public ContentGatingManager()
	{
		RestoreGateLocks();
	}

	private void RestoreGateLocks()
	{
		PersistentStore instance = PersistentStore.GetInstance("dnep1Gates");
		_unlockedGateCount = instance.GetInt("unlockedGates");
	}

	private void SaveGateLocks()
	{
		PersistentStore instance = PersistentStore.GetInstance("dnep1Gates");
		instance.Set("unlockedGates", _unlockedGateCount);
		instance.Save();
	}

	public Palringo.Async UpdateLocalInformation()
	{
		if (!DarkNebulaPalringo.IsAuthorized)
		{
			_lastKnownReputation = 0f;
			return Palringo.Async.MakePrecompleted();
		}
		return DarkNebulaPalringo.Connect.GetLocalUserInfo().Then((UserInfo userInfo) =>
		{
			if (userInfo != null)
			{
				_lastKnownReputation = (float)userInfo.ReputationLevel + userInfo.ProgressToNextLevel;
			}
			else
			{
				_lastKnownReputation = 0f;
			}
		});
	}

	public void ResetUnlocks()
	{
		_unlockedGateCount = 0;
		SaveGateLocks();
	}

	public bool IsLevelGated(int pLevelIdx)
	{
		return false; // Offline edition: no purchase or account gates.
	}

	public bool IsGateUnlocked(int pLevelIdx)
	{
		int gateIndex = GetGateIndex(pLevelIdx);
		return gateIndex >= 0 && _unlockedGateCount > gateIndex;
	}

	public void GetGateLevelSpan(int pLevelIdx, out int pMinLevel, out int pMaxLevel)
	{
		int gateIndex = GetGateIndex(pLevelIdx);
		if (gateIndex >= 0)
		{
			pMinLevel = ContentGates[gateIndex];
			if (gateIndex < ContentGates.Length - 1)
			{
				pMaxLevel = ContentGates[gateIndex + 1] - 1;
			}
			else
			{
				pMaxLevel = 23;
			}
		}
		else
		{
			pMinLevel = (pMaxLevel = 0);
		}
	}

	public bool CanUnlockGate(int pLevelIdx)
	{
		int gateIndex = GetGateIndex(pLevelIdx);
		return gateIndex >= 0 && !IsGateUnlocked(pLevelIdx) && GateKeyCount > gateIndex;
	}

	public void UnlockGate(int pLevelIdx)
	{
		int gateIndex = GetGateIndex(pLevelIdx);
		if (gateIndex >= 0)
		{
			_unlockedGateCount = Mathf.Max(_unlockedGateCount, gateIndex + 1);
			SaveGateLocks();
		}
	}

	private int GetPurchasedKeysCount()
	{
		int num = 1;
		while (true)
		{
			string keyPurchaseSKU = GetKeyPurchaseSKU(num);
			if (!IAPmanager.Instance.OwnProduct(keyPurchaseSKU))
			{
				break;
			}
			num++;
		}
		return num - 1;
	}

	public FLD.Async<bool> PurchaseKey()
	{
		IAPItem product = GetNextKeyItem();
		if (product == null)
		{
			return FLD.Async<bool>.MakePrecompleted(false);
		}
		DarkNebulaMetrics.PurchaseIAPTry(product.ID);
		return IAPmanager.Instance.Purchase(product).Then((InAppPurchaseResult pResult) =>
		{
			if (pResult.State == InAppPurchaseState.Aborted)
			{
				DarkNebulaMetrics.PurchaseIAPCancel(product.ID);
			}
			else if (pResult.State == InAppPurchaseState.Failed)
			{
				DarkNebulaMetrics.PurchaseIAPFail(product.ID);
			}
			else if (pResult.State == InAppPurchaseState.Successful)
			{
				DarkNebulaMetrics.PurchaseIAPSuccess(product.ID);
			}
			return pResult.State == InAppPurchaseState.Successful;
		});
	}

	public string GetKeyPrice()
	{
		return IAPmanager.Instance.GetPrice(GetNextKeyPurchaseSKU());
	}

	private string GetNextKeyPurchaseSKU()
	{
		return GetKeyPurchaseSKU(GetPurchasedKeysCount() + 1);
	}

	private string GetKeyPurchaseSKU(int pKeyIndex)
	{
		return string.Format("com.palringo.darknebula1.gatekey.{0}", pKeyIndex);
	}

	private IAPItem GetNextKeyItem()
	{
		string nextKeyPurchaseSKU = GetNextKeyPurchaseSKU();
		if (string.IsNullOrEmpty(nextKeyPurchaseSKU))
		{
			return null;
		}
		return IAPmanager.Instance.GetIAP(nextKeyPurchaseSKU);
	}

	public int GetGateIndex(int pLevelIdx)
	{
		for (int i = 0; i < ContentGates.Length; i++)
		{
			if (ContentGates[i] == pLevelIdx)
			{
				return i;
			}
		}
		return -1;
	}
}
