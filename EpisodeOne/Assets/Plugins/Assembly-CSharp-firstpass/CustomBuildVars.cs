using FLD;

public class CustomBuildVars : CustomBuildVariables<CustomBuildVars>
{
	public string MetricKey = string.Empty;

	public string FlurryAPIKey;

	public bool PalringoTargetDevEnvironment;

	public int HCSpawnChancePercent = 40;

	public int HCSpawnChancePercentMinimum = 10;

	public int Lives;

	public int InGameHCValue = 1;

	public int StartupHC;
}
