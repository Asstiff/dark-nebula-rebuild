using System;
using System.Collections.Generic;
using Palringo;
using UnityEngine;

public class DarkNebulaMetrics
{
	private enum PalringoEvents
	{
		palringo_connect_try = 0,
		palringo_connect_success = 1,
		palringo_connect_fail = 2,
		palringo_connect_cancel = 3,
		level_progression_normal = 4,
		level_progression_expert = 5,
		gate_ = 6,
		iap_purchase_try = 7,
		iap_purchase_success = 8,
		iap_purchase_fail = 9,
		iap_purchase_cancel = 10,
		currency_purchase = 11,
		ad_view = 12,
		ad_click = 13,
		level_upgrade = 14,
		level_upgrade_speed = 15,
		level_upgrade_shield = 16,
		level_upgrade_magnet = 17,
		level_completed_ = 18,
		hacked_score = 19
	}

	private enum PalringoParameters
	{
		level_progression = 0,
		palringo_connected = 1,
		palringo_rep = 2,
		have_purchased_iap = 3,
		currency_balance = 4,
		level_number = 5,
		product = 6,
		source = 7
	}

	public enum PassGateReason
	{
		IAP = 0,
		Palringo = 1
	}

	static DarkNebulaMetrics()
	{
	}

	private static void PrintEvent(string pEvent, Dictionary<string, string> data)
	{
		Debug.Log("Logging metrics event " + pEvent + " with parameters ");
		foreach (KeyValuePair<string, string> datum in data)
		{
			Debug.Log("key: " + datum.Key + " value: " + datum.Value + "\n");
		}
	}

	public static void LogEvent(string pEvent, Dictionary<string, string> data)
	{
	}

	public static void LogHack(string pHackedVariableName)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["variable"] = pHackedVariableName;
		LogEvent(PalringoEvents.hacked_score.ToString(), dictionary);
	}

	public static void LogEvent(string pEvent)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		LogEvent(pEvent, data);
	}

	public static void LevelCompleted(int levelNum, float pTime, bool pIsExpert, bool pIsFirstTime, string pCurrentVessel)
	{
		string pEvent = PalringoEvents.level_completed_.ToString() + levelNum;
		Dictionary<string, string> pData = new Dictionary<string, string>();
		if (pTime >= 0f)
		{
			pData["time"] = UserSettings.SpecialSegmentString((int)pTime);
		}
		pData["is_first_time"] = ((!pIsFirstTime) ? "no" : "yes");
		pData["difficulty"] = ((!pIsExpert) ? "normal" : "expert");
		foreach (int value2 in Enum.GetValues(typeof(Booster.BoosterType)))
		{
			string value = ((!BoosterManager.Have(Booster.GetIdForBooster((Booster.BoosterType)value2, levelNum, pIsExpert))) ? "no" : "yes");
			switch (value2)
			{
			case 0:
				pData["have_speed"] = value;
				break;
			case 1:
				pData["have_shield"] = value;
				break;
			case 2:
				pData["have_magnet"] = value;
				break;
			}
		}
		pData["vessel"] = pCurrentVessel;
		AppendPalringoConnected(ref pData);
		AppendReputationAndLogEvent(pEvent, pData);
	}

	public static void LevelUpgradeSpeed(int pLevel)
	{
		LevelUpgradeX(PalringoEvents.level_upgrade_speed, pLevel);
	}

	public static void LevelUpgradeShield(int pLevel)
	{
		LevelUpgradeX(PalringoEvents.level_upgrade_shield, pLevel);
	}

	public static void LevelUpgradeMagnet(int pLevel)
	{
		LevelUpgradeX(PalringoEvents.level_upgrade_magnet, pLevel);
	}

	private static void LevelUpgradeX(PalringoEvents pEvent, int pLevel)
	{
		Dictionary<string, string> pData = new Dictionary<string, string>();
		pData[PalringoParameters.level_number.ToString()] = pLevel.ToString();
		AppendPalringoConnected(ref pData);
		AppendCurrencyBalance(ref pData);
		AppendHavePurchasedIAP(ref pData);
		AppendReputationAndLogEvent(pEvent.ToString(), pData);
	}

	public static void LevelUpgrade(string pUpgradeName, int pLevel)
	{
		Dictionary<string, string> pData = new Dictionary<string, string>();
		pData[PalringoParameters.product.ToString()] = pUpgradeName;
		pData[PalringoParameters.level_number.ToString()] = pLevel.ToString();
		AppendCurrencyBalance(ref pData);
		AppendHavePurchasedIAP(ref pData);
		AppendReputationAndLogEvent(PalringoEvents.level_upgrade.ToString(), pData);
	}

	public static void CurrencyPurchase(string pProductName)
	{
		Dictionary<string, string> pData = new Dictionary<string, string>();
		pData[PalringoParameters.product.ToString()] = pProductName;
		AppendPalringoConnected(ref pData);
		AppendHavePurchasedIAP(ref pData);
		AppendCurrencyBalance(ref pData);
		AppendLevelProgression(ref pData);
		AppendReputationAndLogEvent(PalringoEvents.currency_purchase.ToString(), pData);
	}

	public static void UnlockGate(PassGateReason pReason, int pGate)
	{
		string pEvent = PalringoEvents.gate_.ToString() + pGate;
		Dictionary<string, string> pData = new Dictionary<string, string>();
		AppendPalringoConnected(ref pData);
		AppendCurrencyBalance(ref pData);
		AppendHavePurchasedIAP(ref pData);
		pData["key"] = pReason.ToString();
		AppendReputationAndLogEvent(pEvent, pData);
	}

	public static void PurchaseIAPTry(string pIAPID)
	{
		PurchaseIAP(PalringoEvents.iap_purchase_try, pIAPID);
	}

	public static void PurchaseIAPSuccess(string pIAPID)
	{
		PurchaseIAP(PalringoEvents.iap_purchase_success, pIAPID);
	}

	public static void PurchaseIAPFail(string pIAPID)
	{
		PurchaseIAP(PalringoEvents.iap_purchase_fail, pIAPID);
	}

	public static void PurchaseIAPCancel(string pIAPID)
	{
		PurchaseIAP(PalringoEvents.iap_purchase_cancel, pIAPID);
	}

	private static void PurchaseIAP(PalringoEvents pEvent, string pProductID)
	{
		Dictionary<string, string> pData = new Dictionary<string, string>();
		pData[PalringoParameters.product.ToString()] = pProductID;
		AppendPalringoConnected(ref pData);
		AppendHavePurchasedIAP(ref pData);
		AppendCurrencyBalance(ref pData);
		AppendLevelProgression(ref pData);
		AppendReputationAndLogEvent(pEvent.ToString(), pData);
	}

	private static void AppendReputationAndLogEvent(string pEvent, Dictionary<string, string> pData)
	{
		pData[PalringoParameters.palringo_rep.ToString()] = "0";
		if (DarkNebulaPalringo.IsAuthorized)
		{
			DarkNebulaPalringo.Connect.GetLocalUserInfo().Wait((UserInfo pUserInfo) =>
			{
				if (pUserInfo != null)
				{
					pData[PalringoParameters.palringo_rep.ToString()] = pUserInfo.ReputationLevel.ToString();
				}
				LogEvent(pEvent, pData);
			});
		}
		else
		{
			LogEvent(pEvent, pData);
		}
	}

	public static void LevelProgression(int pUnlockedLevel, bool pIsExpert)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary[PalringoParameters.level_progression.ToString()] = pUnlockedLevel.ToString();
		if (pIsExpert)
		{
			LogEvent(PalringoEvents.level_progression_expert.ToString(), dictionary);
		}
		else
		{
			LogEvent(PalringoEvents.level_progression_normal.ToString(), dictionary);
		}
	}

	public static void PalringoConnectTry()
	{
		Dictionary<string, string> pData = new Dictionary<string, string>();
		PalringoConnect(PalringoEvents.palringo_connect_try, pData);
	}

	public static void PalringoConnectSuccess()
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		DarkNebulaPalringo.Connect.GetLocalUserInfo().Wait((UserInfo pUserInfo) =>
		{
			if (pUserInfo == null)
			{
				Debug.LogWarning("No userinfo found");
			}
			else
			{
				data[PalringoParameters.palringo_rep.ToString()] = pUserInfo.ReputationLevel.ToString();
			}
			PalringoConnect(PalringoEvents.palringo_connect_success, data);
		});
	}

	public static void PalringoConnectCancel()
	{
		Dictionary<string, string> pData = new Dictionary<string, string>();
		PalringoConnect(PalringoEvents.palringo_connect_cancel, pData);
	}

	public static void PalringoConnectFail(string pReason)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["reason"] = pReason;
		PalringoConnect(PalringoEvents.palringo_connect_fail, dictionary);
	}

	private static void PalringoConnect(PalringoEvents pEvent, Dictionary<string, string> pData)
	{
		AppendSource(ref pData);
		AppendLevelProgression(ref pData);
		LogEvent(pEvent.ToString(), pData);
	}

	private static void AppendSource(ref Dictionary<string, string> pData)
	{
		pData[PalringoParameters.source.ToString()] = Application.loadedLevelName;
	}

	private static void AppendLevelProgression(ref Dictionary<string, string> pData)
	{
		pData[PalringoParameters.level_progression.ToString()] = UserProgress.Instance.GetMaxSelectableLevel(UserSettings.Instance.IsExpertMode).ToString();
	}

	private static void AppendPalringoConnected(ref Dictionary<string, string> pData)
	{
		pData[PalringoParameters.palringo_connected.ToString()] = ((!DarkNebulaPalringo.IsAuthorized) ? "no" : "yes");
	}

	private static void AppendHavePurchasedIAP(ref Dictionary<string, string> pData)
	{
		pData[PalringoParameters.have_purchased_iap.ToString()] = ((!UserPurchases.Instance.HasPurchasedIAP) ? "no" : "yes");
	}

	private static void AppendCurrencyBalance(ref Dictionary<string, string> pData)
	{
		pData[PalringoParameters.currency_balance.ToString()] = GetSegmentedBalance(HardCurrencyManager.Instance.Amount);
	}

	private static string GetSegmentedBalance(int pBalance)
	{
		int[] array = new int[13]
		{
			0, 5, 10, 20, 50, 100, 300, 500, 1000, 1500,
			2000, 3000, 10000
		};
		string result = ">" + array[array.Length - 1];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] >= pBalance)
			{
				result = ((i != 0) ? (array[i - 1] + "-" + array[i]) : (array[0] + "-" + array[1]));
				break;
			}
		}
		return result;
	}
}
