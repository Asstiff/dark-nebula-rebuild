using System;
using System.Collections.Generic;
using Prime31;
using UnityEngine;

namespace FLD
{
	public class GooglePlayPurchaseManager : PurchaseManager
	{
		private bool _billingAvailable;

		private List<GoogleSkuInfo> ProductInfo;

		private string CurrentPurchaseProductId;

		public override bool IsReadyForPurchase
		{
			get
			{
				return ProductInfo != null;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				return _billingAvailable;
			}
		}

		public GooglePlayPurchaseManager()
		{
			Listen();
			GoogleIAB.init(BuildSettings.Current.AndroidPublicKey);
			GoogleIAB.enableLogging(Debug.isDebugBuild);
		}

		private void Listen()
		{
			GoogleIABManager.billingSupportedEvent += billingSupportedEvent;
			GoogleIABManager.billingNotSupportedEvent += billingNotSupportedEvent;
			GoogleIABManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
			GoogleIABManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
			GoogleIABManager.purchaseCompleteAwaitingVerificationEvent += purchaseCompleteAwaitingVerificationEvent;
			GoogleIABManager.purchaseSucceededEvent += purchaseSucceededEvent;
			GoogleIABManager.purchaseFailedEvent += purchaseFailedEvent;
			GoogleIABManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
			GoogleIABManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
		}

		private void billingSupportedEvent()
		{
			Debug.Log("billingSupportedEvent");
			_billingAvailable = true;
			SignalBillingAvailable(_billingAvailable);
		}

		private void billingNotSupportedEvent(string error)
		{
			Debug.Log("billingNotSupportedEvent: " + error);
			_billingAvailable = false;
			SignalBillingAvailable(_billingAvailable);
		}

		private void queryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
		{
			Debug.Log("queryInventorySucceededEvent");
			Utils.logObject(purchases);
			Utils.logObject(skus);
			ClearHistory();
			AddProductInfos(skus);
			foreach (GooglePurchase purchase in purchases)
			{
				if (purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
				{
					AddToPurchaseHistoryQuantity(purchase.productId, 1);
				}
			}
			SignalPurchasesRestored(true);
		}

		private void AddProductInfos(List<GoogleSkuInfo> skus)
		{
			if (ProductInfo == null)
			{
				ProductInfo = skus;
				return;
			}
			foreach (GoogleSkuInfo sku in skus)
			{
				if (GetProductData(sku.productId) == null)
				{
					ProductInfo.Add(sku);
				}
			}
		}

		private void queryInventoryFailedEvent(string error)
		{
			Debug.Log("queryInventoryFailedEvent: " + error);
			SignalPurchasesRestored(false);
		}

		private void purchaseCompleteAwaitingVerificationEvent(string purchaseData, string signature)
		{
			Debug.Log("purchaseCompleteAwaitingVerificationEvent. purchaseData: " + purchaseData + ", signature: " + signature);
		}

		private void purchaseSucceededEvent(GooglePurchase purchase)
		{
			Debug.Log("purchaseSucceededEvent: " + purchase);
			CurrentPurchaseProductId = null;
			AddToPurchaseHistoryQuantity(purchase.productId, 1);
			SignalPurchaseSuccess(purchase.productId, purchase.orderId);
		}

		private void purchaseFailedEvent(string error)
		{
			Debug.Log("purchaseFailedEvent: " + error);
			string currentPurchaseProductId = CurrentPurchaseProductId;
			CurrentPurchaseProductId = null;
			SignalPurchaseFailure(false, currentPurchaseProductId, error);
		}

		private void consumePurchaseSucceededEvent(GooglePurchase purchase)
		{
			Debug.Log("consumePurchaseSucceededEvent: " + purchase);
			PurchaseItem purchaseItem = GetPurchaseItem(purchase.productId);
			if (purchaseItem != null)
			{
				SetPurchaseHistoryQuantity(purchase.productId, purchaseItem.quantity - 1);
			}
		}

		private void consumePurchaseFailedEvent(string error)
		{
			Debug.Log("consumePurchaseFailedEvent: " + error);
		}

		public override void Purchase(string pProductId, int pQuantity)
		{
			if (Application.isEditor)
			{
				SignalPurchaseFailure(false, pProductId, "Can't purchase in editor");
				return;
			}
			if (pQuantity != 1)
			{
				SignalPurchaseFailure(false, pProductId, "Cannot purchase quantities other than 1");
				return;
			}
			if (!string.IsNullOrEmpty(CurrentPurchaseProductId))
			{
				SignalPurchaseFailure(false, pProductId, "Cannot purchase while another purchase is in progress " + CurrentPurchaseProductId);
				return;
			}
			CurrentPurchaseProductId = pProductId;
			GoogleIAB.purchaseProduct(pProductId);
		}

		public override void RestorePurchases()
		{
			GoogleIAB.queryInventory(DarkNebulaStore.Skus);
		}

		public override void RequestData(string[] pProductIds)
		{
			GoogleIAB.queryInventory(pProductIds);
		}

		public override PurchaseProductData GetProductData(string pProductId)
		{
			if (ProductInfo == null)
			{
				throw new InvalidOperationException("Need to successully RestorePurchases or RequestData before GetProductData can be called");
			}
			foreach (GoogleSkuInfo item in ProductInfo)
			{
				if (item.productId == pProductId)
				{
					PurchaseProductData purchaseProductData = new PurchaseProductData();
					purchaseProductData.Price = item.price;
					purchaseProductData.CurrencySymbol = string.Empty;
					return purchaseProductData;
				}
			}
			return null;
		}
	}
}
