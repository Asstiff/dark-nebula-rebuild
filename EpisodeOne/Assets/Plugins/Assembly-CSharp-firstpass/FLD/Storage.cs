namespace FLD
{
	public class Storage
	{
		public static StorageUnit settings = new StorageUnit("settings", true);

		public static StorageUnit statistics = new StorageUnit("statistics", true);

		public static StorageUnit persistent = new StorageUnit("persistent", true);

		public static StorageUnit temporary = new StorageUnit("temporary", false);
	}
}
