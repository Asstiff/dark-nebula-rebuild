using System.Collections.Generic;
using FLD;
using UnityEngine;

public class IAPmanager
{
	private static IAPmanager _instance;

	private IInAppPurchasingService _iAppService;

	private bool _firstConnect = true;

	public static IAPmanager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new IAPmanager();
			}
			return _instance;
		}
	}

	public IInAppPurchasingService AppService
	{
		get
		{
			return _iAppService;
		}
	}

	public void InitIAPService()
	{
		Debug.LogWarning("Offline test build: in-app purchasing is unavailable.");
	}

	private void HandleServicesInitialised()
	{
		_iAppService = Services.GetService<IInAppPurchasingService>();
		if (_iAppService == null)
		{
			Debug.LogWarning("Failed to initialise InAppPurchasingService");
			return;
		}
		List<string> list = new List<string>();
		list.AddRange(_iAppService.GetOwnedProducts());
		foreach (string item in list)
		{
			IAPItem iAP = GetIAP(item);
			iAP.GiveItem();
			if (iAP != null && iAP.IsConsumable)
			{
				_iAppService.Consume(item);
			}
		}
		if (_firstConnect)
		{
			RestorePurchases(true);
		}
		_firstConnect = false;
	}

	public Async<InAppPurchaseResult> Purchase(IAPItem pItem)
	{
		if (_iAppService == null)
		{
			InitIAPService();
			return Async<InAppPurchaseResult>.MakePrecompleted(new InAppPurchaseResult(InAppPurchaseState.Failed));
		}
		return _iAppService.Purchase(pItem.ID).Then((InAppPurchaseResult pInAppPurchaseResult) =>
		{
			if (pInAppPurchaseResult.State == InAppPurchaseState.Successful && pItem.IsConsumable)
			{
				_iAppService.Consume(pItem.ID);
			}
			return pInAppPurchaseResult;
		});
	}

	public string GetPrice(string pID)
	{
		if (_iAppService != null)
		{
			string readablePrice = _iAppService.GetReadablePrice(pID);
			if (!string.IsNullOrEmpty(readablePrice))
			{
				return new string(readablePrice.FilterRemove((char c) => c < ' ' || c >= '\u007f').ToArray());
			}
			return string.Empty;
		}
		return string.Empty;
	}

	public void RestorePurchases(bool pBackground = false)
	{
		if (_iAppService == null)
		{
			IAPLoadingPopup.Fail();
			return;
		}
		if (!pBackground)
		{
			IAPLoadingPopup.Show("RESTORING PURCHASES".Localize());
		}
		_iAppService.RestorePurchases().Wait(() =>
		{
			RestoreOwnedProducts();
		});
	}

	public bool OwnProduct(string pKey)
	{
		bool result = false;
		if (_iAppService != null)
		{
			result = _iAppService.GetOwnedProducts().Contains(pKey);
		}
		return result;
	}

	private void RestoreOwnedProducts()
	{
		Debug.Log("restoring products. There are " + _iAppService.GetOwnedProducts().Count + " products");
		foreach (string ownedProduct in _iAppService.GetOwnedProducts())
		{
			Debug.Log("Found owned product " + ownedProduct);
			foreach (IAPItem iAPItem in GetIAPItems())
			{
				if (iAPItem.ID == ownedProduct)
				{
					Debug.Log("Found matching item and will award this to player " + iAPItem.ID);
					iAPItem.GiveItem();
				}
			}
		}
		IAPLoadingPopup.HidePopup();
	}

	public IAPItem GetIAP(string pSKU)
	{
		return GetIAPItems().Find((IAPItem p) => p.ID == pSKU);
	}

	public List<IAPItem> GetIAPItems()
	{
		Object[] array = Resources.LoadAll("IAP");
		List<IAPItem> list = new List<IAPItem>();
		Object[] array2 = array;
		foreach (Object obj in array2)
		{
			list.Add((IAPItem)obj);
		}
		return list;
	}
}
