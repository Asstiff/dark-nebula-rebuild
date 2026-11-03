using System.Collections.Generic;

namespace FLD
{
	[Service("In-App Purchasing")]
	public interface IInAppPurchasingService : IService
	{
		Async<InAppPurchaseResult> Purchase(string pSKU);

		ICollection<string> GetOwnedProducts();

		void Consume(string pSKU);

		string GetReadablePrice(string pSKU);

		Async RestorePurchases();
	}
}
