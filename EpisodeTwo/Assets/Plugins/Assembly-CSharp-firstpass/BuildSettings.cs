using FLD;

public class BuildSettings
{
	public const bool UsePushNotifications = false;

	public static BuildSettings Current;

	public string MetricKey = string.Empty;

	public string AndroidPublicKey = string.Empty;

	public InAppPurchaseProvider InAppProvider;

	public string VungleAppId = string.Empty;

	public string ChartBoostAppId = string.Empty;

	public string ChartBoostAppSignature = string.Empty;

	public bool VungleEnabled { get { return false; } }

	public bool ChartBoostEnabled { get { return false; } }

	public string ServerSettingsURL = string.Empty;

	public string ServerSettingsSecret = string.Empty;

	public bool SpheroEnabled = true;

	public int SuperLifeOffset;

	public bool ExtendedLevelsEnabled = true;

	public bool FacebookEnabled;

	public bool GameOverAllowContinue
	{
		get
		{
			return !VungleEnabled && !ChartBoostEnabled;
		}
	}

	static BuildSettings()
	{
		Current = new BuildSettings();
		Current.VungleAppId = "com.fld.DarkNebula.EP2";
		Current.ChartBoostAppId = "514735d617ba477d4d0001c1";
		Current.ChartBoostAppSignature = "aa57ac186a7e6f5b0e6654955b5886b10555fb93";
		Current.ServerSettingsSecret = "Uyqaj5942yfKTRL8nycG";
		Current.ServerSettingsURL = "http://icytower.com/icytower2/dn2-android-callhome.php";
		Current.MetricKey = "XX82475XBX67D46SRV2F";
		Current.AndroidPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAykkJzDoX0YP3DfAUEYWza8+Pw/oLSD2v3nfLUt5GKrOoFjfa+7n66OtDJyI+Px5JiJb1sZJ/H5YgFHPSSQ+FxYMd3+Sz+xv5rMZgJard3P8zCtoV9IgXO6e1QjuPWcqyOrFZGqpordKCxogz3Pz0DFSxA1l2cmxlkfDA8dy1SLsmHbWMMFcPbyNcTDPlsq5aYynOeihxGSZ0KIfJV/Lgp5R5HqZKJUAwMroRZl5LKLGG/wpl/c3OkrD7jnTfqRQjw+QOHWXozXu+d/XdhJupHlLfodA+sX2UYpDekqCt8nqN18uEILlLlFWqSaC37GbxVKOvK3cjEHQowOI+n4YkiQIDAQAB";
		Current.InAppProvider = InAppPurchaseProvider.None; // No legacy billing SDK; preserve local purchases.
		Current.SpheroEnabled = false; // The original bridge is 32-bit only.
		if (Current.VungleEnabled || Current.ChartBoostEnabled)
		{
			Current.SuperLifeOffset = -2;
		}
	}

	public PurchaseManager CreatePurchaseManager()
	{
		PurchaseManager purchaseManager = null;
		switch (InAppProvider)
		{
		case InAppPurchaseProvider.GooglePlay:
			purchaseManager = new GooglePlayPurchaseManager();
			break;
		case InAppPurchaseProvider.Itunes:
			purchaseManager = iTunesPluginBinding.CreatePurchaseManager();
			break;
		}
		if (purchaseManager == null)
		{
			purchaseManager = new DummyPurchaseManager();
		}
		return purchaseManager;
	}
}
