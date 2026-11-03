using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	[Service("Google Play Store", Description = "Google Play in-app purchasing.", AvailablePlatforms = "Android")]
	public class GooglePlayInAppPurchasing : AbstractInAppPurchasingService, IService, IInAppPurchasingService
	{
		private Async<bool> _initAsr;

		private Async<InAppPurchaseResult> _pendingPurchaseAsr;

		private List<string> _ownedProducts = new List<string>();

		[ServiceConfigurable]
		public string APIKey { get; set; }

		private bool HasPendingPurchase
		{
			get
			{
				return _pendingPurchaseAsr != null;
			}
		}

		public Async<bool> Initialize()
		{
			_initAsr = new Async<bool>();
			LoadProductList();
			GoogleIABManager.purchaseSucceededEvent += HandlePurchaseSucceeded;
			GoogleIABManager.purchaseFailedEvent += HandlePurchaseFailed;
			GoogleIABManager.billingSupportedEvent += HandleBillingSupported;
			GoogleIABManager.billingNotSupportedEvent += delegate
			{
				_initAsr.Complete(false);
			};
			GoogleIAB.init(APIKey);
			return _initAsr;
		}

		private void HandlePurchaseSucceeded(GooglePurchase pPurchase)
		{
			if (!HasPendingPurchase)
			{
				Debug.Log("Purchase succeeded unexpectedly");
				return;
			}
			Async<InAppPurchaseResult> pendingPurchaseAsr = _pendingPurchaseAsr;
			_pendingPurchaseAsr = null;
			pendingPurchaseAsr.Complete(new InAppPurchaseResult(InAppPurchaseState.Successful)
			{
				ProductSKU = pPurchase.productId,
				OrderId = pPurchase.orderId
			});
		}

		private void HandlePurchaseFailed(string pError)
		{
			Debug.LogError("Purchase failed: " + pError);
			Async<InAppPurchaseResult> pendingPurchaseAsr = _pendingPurchaseAsr;
			_pendingPurchaseAsr = null;
			pendingPurchaseAsr.Complete(new InAppPurchaseResult(InAppPurchaseState.Failed));
		}

		private void HandleBillingSupported()
		{
			GoogleIABManager.queryInventorySucceededEvent += HandleQueryInventory;
			GoogleIABManager.queryInventoryFailedEvent += delegate
			{
				_initAsr.Complete(false);
			};
			GoogleIAB.queryInventory(base.ProductSKUs);
		}

		private void HandleQueryInventory(List<GooglePurchase> pPurchases, List<GoogleSkuInfo> pSkus)
		{
			foreach (GoogleSkuInfo pSku in pSkus)
			{
				ProductInfo productWithSKU = GetProductWithSKU(pSku.productId);
				if (productWithSKU == null)
				{
					Debug.LogWarning("Got info about nonexistent SKU " + pSku.productId);
					continue;
				}
				productWithSKU.CurrencyCode = pSku.priceCurrencyCode;
				productWithSKU.ReadablePrice = pSku.price;
			}
			foreach (GooglePurchase pPurchase in pPurchases)
			{
				if (!_ownedProducts.Contains(pPurchase.productId))
				{
					_ownedProducts.Add(pPurchase.productId);
					Debug.Log("Product we already own: " + pPurchase.productId);
				}
			}
			_initAsr.Complete(true);
		}

		public Async<InAppPurchaseResult> Purchase(string pSKU)
		{
			if (!HasProductWithSKU(pSKU))
			{
				Debug.LogError(string.Format("No such SKU \"{0}\" (did you forget to add it to the list?)", pSKU));
				return Async.MakePrecompleted(new InAppPurchaseResult(InAppPurchaseState.Failed));
			}
			if (HasPendingPurchase)
			{
				Debug.Log("A purchase is already pending completion");
				return Async.MakePrecompleted(new InAppPurchaseResult(InAppPurchaseState.Failed));
			}
			_pendingPurchaseAsr = new Async<InAppPurchaseResult>();
			GoogleIAB.purchaseProduct(pSKU);
			return _pendingPurchaseAsr;
		}

		public ICollection<string> GetOwnedProducts()
		{
			return _ownedProducts;
		}

		public void Consume(string pSKU)
		{
			GoogleIAB.consumeProduct(pSKU);
			_ownedProducts.Remove(pSKU);
		}

		public Async RestorePurchases()
		{
			return Async.MakePrecompleted();
		}

		string IInAppPurchasingService.GetReadablePrice(string pSKU)
		{
			return GetReadablePrice(pSKU);
		}
	}
}
