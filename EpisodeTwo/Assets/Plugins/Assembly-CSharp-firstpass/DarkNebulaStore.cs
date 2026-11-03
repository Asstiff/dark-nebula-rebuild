using System;
using System.Collections.Generic;
using FLD;
using UnityEngine;

public class DarkNebulaStore
{
	public delegate void PurchaseFinishedHandler(string pId);

	public delegate void PurchaseFailedHandler(bool pCanceled, string pId, string pMessage);

	public const string ProductIdRemoveAds = "darknebula.ep2.removeads";

	private static DarkNebulaStore _instance;

	private static PurchaseManager purchaseManager;

	private static bool _hasPurchasedRemoveAds = false;

	public static bool Enabled = BuildSettings.Current.InAppProvider != InAppPurchaseProvider.None;

	public static bool IsReadyForPurchase
	{
		get
		{
			return purchaseManager != null && purchaseManager.IsReadyForPurchase;
		}
	}

	public static string[] Skus
	{
		get
		{
			return new string[1] { "darknebula.ep2.removeads" };
		}
	}

	public static DarkNebulaStore Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DarkNebulaStore();
				purchaseManager = BuildSettings.Current.CreatePurchaseManager();
			}
			return _instance;
		}
	}

	public event PurchaseFinishedHandler onPurchaseFinished;

	public event PurchaseFailedHandler onPurchaseFailed;

	public static event Action<bool> PurchaseRestored;

	private DarkNebulaStore()
	{
	}

	public static int GetPriceTier(string pProductID)
	{
		switch (pProductID)
		{
		case "darknebula.ep2.removeads":
			return 1;
		default:
			return 0;
		}
	}

	public static string GetBoughtProducts()
	{
		List<string> list = new List<string>();
		if (_hasPurchasedRemoveAds)
		{
			list.Add("darknebula.ep2.removeads");
		}
		return string.Join(",", list.ToArray());
	}

	public void Init()
	{
		if (purchaseManager.IsAvailable)
		{
			Debug.Log("RestorePurchases");
			purchaseManager.PurchasesRestored += OnPurchasesRestored;
			purchaseManager.RestorePurchases();
		}
		else
		{
			purchaseManager.BillingAvailable += OnBillingAvailableChanged;
		}
	}

	private void OnBillingAvailableChanged(bool pIsAvailable)
	{
		if (pIsAvailable)
		{
			purchaseManager.BillingAvailable -= OnBillingAvailableChanged;
			Init();
		}
	}

	private void OnPurchasesRestored(bool pSuccess)
	{
		if (PurchaseRestored != null)
		{
			PurchaseRestored(pSuccess);
		}
		if (!pSuccess)
		{
			Debug.Log("RestorePurchases Completed Failed");
			return;
		}
		if (purchaseManager.HasPurchased("darknebula.ep2.removeads"))
		{
			Debug.Log("HasPurchased darknebula.ep2.removeads");
			_hasPurchasedRemoveAds = true;
			UserSettings.I.SetIsPayingUser(true);
		}
		else
		{
			Debug.Log("Has not Purchased darknebula.ep2.removeads");
		}
		if (!_hasPurchasedRemoveAds)
		{
			UserSettings.I.SetIsPayingUser(false);
		}
	}

	public void TryPurchaseRemoveAds()
	{
		// Billing is not included in this build.
	}

	private void OnFinished(string pProductId, int pQuantity, string pTransactionId)
	{
		StopListeningToPurchase();
		Debug.Log("Finished purchasing " + pProductId);
		if (pProductId == "darknebula.ep2.removeads")
		{
			_hasPurchasedRemoveAds = true;
		}
		UserSettings.I.SetIsPayingUser(true);
		if (onPurchaseFinished != null)
		{
			onPurchaseFinished(pProductId);
		}
	}

	private void StopListeningToPurchase()
	{
		DarkNebulaAds.IsAboutToLeaveAppAndComeBack = false;
		purchaseManager.OnPostPurchase -= OnFinished;
		purchaseManager.OnPostPurchaseFailure -= OnError;
		purchaseManager.OnPostPurchaseCancelled -= OnCancel;
	}

	private void OnError(string pId, string pMessage)
	{
		StopListeningToPurchase();
		Debug.Log("Error purchasing " + pMessage);
		if (onPurchaseFailed != null)
		{
			onPurchaseFailed(false, pId, pMessage);
		}
	}

	private void OnCancel(string pId)
	{
		StopListeningToPurchase();
		Debug.Log("Canceled purchasing " + pId);
		if (onPurchaseFailed != null)
		{
			onPurchaseFailed(true, pId, string.Empty);
		}
	}

	public static bool HasRemovedAds()
	{
		return _hasPurchasedRemoveAds;
	}
}
