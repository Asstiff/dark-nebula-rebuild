namespace FLD
{
	internal class SystemConfiguration : BaseSystemConfiguration
	{
		private static SystemConfiguration _instance;

		public static SystemConfiguration Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SystemConfiguration();
				}
				return _instance;
			}
		}

		private SystemConfiguration()
		{
		}

		public override void RegisterServices()
		{
			GooglePlayInAppPurchasing googlePlayInAppPurchasing = new GooglePlayInAppPurchasing();
			googlePlayInAppPurchasing.APIKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAoI1I2NlfSs5pgNVyHQsdmVFGKJj+YvhLI3zP9okQ+dPG1d87FjgcriYUnASkMPoP51IJ2kHrTDKx43RpYPjVd4WpjRUggc6iGveD2//HGv2ONbkM4aIT7lcibZIxQF1r3X332NtkIlCwiKK6KuYjV+6kpIWoXgjzpGMvSdfchVi712wcdI1m9z0whYCkGkZ8swQvvQLFzshh2jLG5bn/TvntH4uT1c7/sT5hoSmK9/It/NFBPdIcI1h/F6xJeYmfBX+qzjtG0WA4KupIj1xAxPR22Bm65wSrx3WE96XNGMQ1srKcwtKSNBRFpNlUD4WkCPs4mGpaXEdGacxRSNTMNQIDAQAB";
			googlePlayInAppPurchasing.ProductListResourcePath = "IAPGooglePlay";
			Services.RegisterServiceProvider((IInAppPurchasingService)googlePlayInAppPurchasing);
		}
	}
}
