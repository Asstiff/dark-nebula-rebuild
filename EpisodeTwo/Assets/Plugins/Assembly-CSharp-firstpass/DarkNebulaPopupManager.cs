using FLD;

public class DarkNebulaPopupManager
{
	public static PopupManager Instance;

	static DarkNebulaPopupManager()
	{
		Instance = new AndroidPopupManager();
	}
}
