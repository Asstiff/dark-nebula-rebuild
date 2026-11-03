using FLD;

public class iTunesPluginBinding
{
	public static PurchaseManager CreatePurchaseManager()
	{
		return new DummyPurchaseManager();
	}
}
