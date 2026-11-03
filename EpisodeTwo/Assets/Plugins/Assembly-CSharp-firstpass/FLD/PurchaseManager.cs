using System;
using System.Collections.Generic;

namespace FLD
{
	public abstract class PurchaseManager
	{
		public delegate void PostPurchaseSuccessHandler(string pProductId, int pQuantity, string pTransactionId);

		public delegate void PostPurchaseFailureHandler(string pProductId, string pReason);

		public delegate void PostPurchaseCancelledHandler(string pProductId);

		protected string _pendingProductId = string.Empty;

		protected int _pendingQuantity;

		protected Dictionary<string, PurchaseItem> _purchaseHistory;

		private PersistentStorage _historyStorage = new PersistentStorage("ufugapurchases");

		public abstract bool IsAvailable { get; }

		public abstract bool IsReadyForPurchase { get; }

		public event Action<string, string> PurchaseSucceeded;

		public event Action<bool, string, string> PurchaseFailed;

		public event Action<bool> PurchasesRestored;

		public event Action<bool> BillingAvailable;

		public event PostPurchaseSuccessHandler OnPostPurchase;

		public event PostPurchaseFailureHandler OnPostPurchaseFailure;

		public event PostPurchaseCancelledHandler OnPostPurchaseCancelled;

		protected PurchaseManager()
		{
			LoadPurchaseHistory();
		}

		public abstract void Purchase(string pProductId, int pQuantity);

		public abstract void RestorePurchases();

		public abstract void RequestData(string[] pProductIds);

		public abstract PurchaseProductData GetProductData(string pProductId);

		protected void SavePurchaseHistory()
		{
			_historyStorage.Delete();
			foreach (KeyValuePair<string, PurchaseItem> item in _purchaseHistory)
			{
				_historyStorage.Set(item.Key, item.Value.ToString(), true);
			}
		}

		protected void LoadPurchaseHistory()
		{
			_purchaseHistory = new Dictionary<string, PurchaseItem>();
			foreach (KeyValuePair<string, string> item in _historyStorage.GetAll())
			{
				PurchaseItem purchaseItem = new PurchaseItem();
				purchaseItem.Unpack(item.Value);
				_purchaseHistory.Add(item.Key, purchaseItem);
			}
		}

		protected void AddToPurchaseHistoryQuantity(string pProductId, int pQuantity)
		{
			uDebug.Info("AddToPurchaseHistoryQuantity: " + pProductId + " / " + pQuantity);
			GetPurchaseItem(pProductId).quantity += pQuantity;
			SavePurchaseHistory();
		}

		protected void SetPurchaseHistoryQuantity(string pProductId, int pQuantity)
		{
			GetPurchaseItem(pProductId).quantity = pQuantity;
			SavePurchaseHistory();
		}

		protected void SetPurchaseHistoryData(string pProductId, string pTitle, string pDescription, string pPrice)
		{
			PurchaseItem purchaseItem = GetPurchaseItem(pProductId);
			purchaseItem.title = pTitle;
			purchaseItem.description = pDescription;
			purchaseItem.price = pPrice;
		}

		public bool HasPurchased(string pProductId)
		{
			return _purchaseHistory.ContainsKey(pProductId) && _purchaseHistory[pProductId].quantity > 0;
		}

		public bool HasPurchasedAnything()
		{
			foreach (KeyValuePair<string, PurchaseItem> item in _purchaseHistory)
			{
				if (HasPurchased(item.Key))
				{
					return true;
				}
			}
			return false;
		}

		public PurchaseItem GetPurchaseItem(string pProductId)
		{
			if (!_purchaseHistory.ContainsKey(pProductId))
			{
				PurchaseItem purchaseItem = new PurchaseItem();
				purchaseItem.productId = pProductId;
				_purchaseHistory.Add(pProductId, purchaseItem);
			}
			return _purchaseHistory[pProductId];
		}

		public void ClearHistory()
		{
			_purchaseHistory = new Dictionary<string, PurchaseItem>();
			SavePurchaseHistory();
		}

		protected void ResetPendingRequest()
		{
			_pendingProductId = string.Empty;
		}

		protected void SignalPurchaseSuccess(string pProductId, string pTransactionId)
		{
			if (_pendingQuantity == 0)
			{
				_pendingQuantity = 1;
			}
			AddToPurchaseHistoryQuantity(pProductId, _pendingQuantity);
			if (PurchaseSucceeded != null)
			{
				PurchaseSucceeded(pProductId, pTransactionId);
			}
			else
			{
				uDebug.Log(uDebug.Level.ERROR, "Purchase success notification was ignored");
			}
			if (OnPostPurchase != null)
			{
				OnPostPurchase(pProductId, _pendingQuantity, pTransactionId);
			}
			ResetPendingRequest();
		}

		protected void SignalPurchaseFailure(bool pWasCancelled, string pProductId, string pReason)
		{
			if (PurchaseFailed != null)
			{
				PurchaseFailed(pWasCancelled, pProductId, pReason);
			}
			else
			{
				uDebug.Log(uDebug.Level.WARNING, "Purchase failure notification was ignored");
			}
			if (pWasCancelled)
			{
				if (OnPostPurchaseCancelled != null)
				{
					OnPostPurchaseCancelled(pProductId);
				}
			}
			else if (OnPostPurchaseFailure != null)
			{
				OnPostPurchaseFailure(pProductId, pReason);
			}
			ResetPendingRequest();
		}

		protected void SignalPurchasesRestored(bool pSuccess)
		{
			if (PurchasesRestored != null)
			{
				PurchasesRestored(pSuccess);
			}
		}

		protected void SignalBillingAvailable(bool pIsAvailable)
		{
			if (BillingAvailable != null)
			{
				BillingAvailable(pIsAvailable);
			}
		}
	}
}
