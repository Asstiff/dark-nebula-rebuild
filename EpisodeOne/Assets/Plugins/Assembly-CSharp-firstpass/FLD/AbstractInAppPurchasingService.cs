using System.Collections.Generic;
using UnityEngine;

namespace FLD
{
	public abstract class AbstractInAppPurchasingService
	{
		protected class ProductInfo
		{
			public string SKU;

			public string CurrencyCode;

			public string ReadablePrice;
		}

		private List<ProductInfo> _knownProducts = new List<ProductInfo>();

		[ServiceConfigurable]
		public string ProductListResourcePath { get; set; }

		protected string[] ProductSKUs
		{
			get
			{
				return _knownProducts.Map((ProductInfo p) => p.SKU).ToArray();
			}
		}

		protected bool HasProductWithSKU(string pSKU)
		{
			return _knownProducts.Any((ProductInfo p) => p.SKU == pSKU);
		}

		protected ProductInfo GetProductWithSKU(string pSKU)
		{
			return _knownProducts.Find((ProductInfo p) => p.SKU == pSKU);
		}

		public string GetReadablePrice(string pSKU)
		{
			string result = string.Empty;
			ProductInfo productInfo = _knownProducts.Find((ProductInfo p) => p.SKU == pSKU);
			if (productInfo != null)
			{
				result = productInfo.ReadablePrice;
			}
			else
			{
				Debug.Log("Found no product with SKU " + pSKU);
			}
			return result;
		}

		protected void LoadProductList()
		{
			_knownProducts.Clear();
			TextAsset textAsset = (TextAsset)Resources.Load(ProductListResourcePath, typeof(TextAsset));
			if (textAsset != null)
			{
				foreach (string item in textAsset.text.Split('\n').Map((string s) => s.Trim()).FilterRemove(string.IsNullOrEmpty))
				{
					_knownProducts.Add(new ProductInfo
					{
						SKU = item
					});
				}
				return;
			}
			Debug.LogWarning("No product list could be loaded; making in-app purchases will not be possible");
		}
	}
}
