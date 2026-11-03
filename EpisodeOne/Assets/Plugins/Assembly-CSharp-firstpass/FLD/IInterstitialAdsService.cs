namespace FLD
{
	[Service("Interstitial Ads", Description = "")]
	public interface IInterstitialAdsService : IService
	{
		Async<string> ShowInterstitial(string pLocation);

		Async<string> CacheInterstitial(string pLocation);

		bool HasCachedInterstitial(string pLocation);
	}
}
