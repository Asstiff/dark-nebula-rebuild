using System;
using FLD;
using UnityEngine;

public class IAPShopItem : MonoBehaviour
{
	public Action<IAPItem> BuyClicked;

	public IAPItem IAP;

	[SerializeField]
	private SimpleButton _buyButton;

	[SerializeField]
	private DynamicFont _itemLabel;

	[SerializeField]
	private DynamicFont _itemDescription;

	[SerializeField]
	private Transform _container;

	[SerializeField]
	private DynamicFont _priceLabel;

	private void Start()
	{
		_priceLabel.gameObject.SetActive(false);
		_buyButton.ButtonUpEvent += HandleButtonUpEvent;
		if (IAP is IAPItemHardCurrency)
		{
			IAPItemHardCurrency iAPItemHardCurrency = (IAPItemHardCurrency)IAP;
			_itemLabel.Text = IAP.Name.Localize(iAPItemHardCurrency.HardCurrencyAmount);
		}
		else
		{
			_itemLabel.Text = IAP.LocalisedName;
		}
		if (!string.IsNullOrEmpty(IAP.LocalisedDescription))
		{
			_itemDescription.gameObject.SetActive(true);
			_itemDescription.Text = IAP.LocalisedDescription;
		}
		else
		{
			_itemDescription.gameObject.SetActive(false);
		}
		string price = IAPmanager.Instance.GetPrice(IAP.ID);
		if (!string.IsNullOrEmpty(price))
		{
			price = price.ToUpper();
			_priceLabel.gameObject.SetActive(true);
			_priceLabel.Text = price;
		}
	}

	private void HandleButtonUpEvent(SimpleButton pButton)
	{
		if (BuyClicked != null)
		{
			BuyClicked(IAP);
		}
	}
}
