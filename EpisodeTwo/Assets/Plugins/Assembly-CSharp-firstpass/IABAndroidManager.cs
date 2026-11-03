using System;
using UnityEngine;

public class IABAndroidManager : MonoBehaviour
{
	public static event Action<bool> billingSupportedEvent;

	public static event Action<string> purchaseSucceededEvent;

	public static event Action<string, string> purchaseSignatureVerifiedEvent;

	public static event Action<string> purchaseCancelledEvent;

	public static event Action<string> purchaseRefundedEvent;

	public static event Action<string> purchaseFailedEvent;

	public static event Action transactionsRestoredEvent;

	public static event Action<string> transactionRestoreFailedEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void billingSupported(string isSupported)
	{
		if (billingSupportedEvent != null)
		{
			billingSupportedEvent(isSupported == "1");
		}
	}

	public void purchaseSignatureVerified(string data)
	{
		if (purchaseSignatureVerifiedEvent != null)
		{
			string[] array = data.Split(new string[1] { "~~~" }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2)
			{
				purchaseSignatureVerifiedEvent(array[0], array[1]);
			}
		}
	}

	public void purchaseSucceeded(string productId)
	{
		if (purchaseSucceededEvent != null)
		{
			purchaseSucceededEvent(productId);
		}
	}

	public void purchaseCancelled(string productId)
	{
		if (purchaseCancelledEvent != null)
		{
			purchaseCancelledEvent(productId);
		}
	}

	public void purchaseRefunded(string productId)
	{
		if (purchaseRefundedEvent != null)
		{
			purchaseRefundedEvent(productId);
		}
	}

	public void purchaseFailed(string productId)
	{
		if (purchaseFailedEvent != null)
		{
			purchaseFailedEvent(productId);
		}
	}

	public void transactionsRestored(string empty)
	{
		if (transactionsRestoredEvent != null)
		{
			transactionsRestoredEvent();
		}
	}

	public void transactionRestoreFailed(string error)
	{
		if (transactionRestoreFailedEvent != null)
		{
			transactionRestoreFailedEvent(error);
		}
	}
}
