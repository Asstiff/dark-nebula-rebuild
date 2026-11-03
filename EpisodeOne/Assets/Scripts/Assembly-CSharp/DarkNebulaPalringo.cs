using FLD;
using Palringo;

public static class DarkNebulaPalringo
{
	private static Connect _connect;

	// Online account services are unavailable in this offline migration.
	public static bool IsAuthorized => false;

	public static Connect Connect
	{
		get
		{
			if (_connect == null)
			{
				if (CustomBuildVariables<CustomBuildVars>.Instance.PalringoTargetDevEnvironment)
				{
					_connect = new Connect(HostConfiguration.DevelopmentHost, Connect.Options.MessageAuthentication);
				}
				else
				{
					_connect = new Connect(HostConfiguration.ProductionHost, Connect.Options.MessageAuthentication);
				}
			}
			return _connect;
		}
	}
}
