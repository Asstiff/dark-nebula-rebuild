namespace FLD
{
	public class InAppPurchaseResult
	{
		public InAppPurchaseState State;

		public string ProductSKU;

		public string OrderId;

		public InAppPurchaseResult(InAppPurchaseState pState)
		{
			State = pState;
		}
	}
}
