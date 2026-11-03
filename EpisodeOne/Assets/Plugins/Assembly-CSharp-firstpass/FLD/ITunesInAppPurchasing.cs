using System.Collections.Generic;

namespace FLD
{
	[Service("iTunes App Store", Description = "iTunes App Store in-app purchasing.", AvailablePlatforms = "iPhone")]
	public class ITunesInAppPurchasing : AbstractInAppPurchasingService, IService, IInAppPurchasingService
	{
		public Async<bool> Initialize()
		{
			return Async<bool>.MakePrecompleted(false);
		}

		public Async<InAppPurchaseResult> Purchase(string pSKU)
		{
			return Async.MakePrecompleted(new InAppPurchaseResult(InAppPurchaseState.Failed));
		}

		public ICollection<string> GetOwnedProducts()
		{
			return new List<string>();
		}

		public void Consume(string pSKU)
		{
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
