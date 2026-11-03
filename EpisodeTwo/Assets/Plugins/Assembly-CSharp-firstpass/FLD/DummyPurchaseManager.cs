namespace FLD
{
	public class DummyPurchaseManager : PurchaseManager
	{
		public override bool IsAvailable
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadyForPurchase
		{
			get
			{
				return IsAvailable;
			}
		}

		public override void RestorePurchases()
		{
		}

		public override void Purchase(string pProductId, int pQuantity)
		{
			uFuga.Instance.Delay(1.5f, () =>
			{
				SignalPurchaseFailure(false, pProductId, "Dummy manager");
			});
		}

		public override void RequestData(string[] pProductIds)
		{
		}

		public override PurchaseProductData GetProductData(string pProductId)
		{
			return null;
		}
	}
}
