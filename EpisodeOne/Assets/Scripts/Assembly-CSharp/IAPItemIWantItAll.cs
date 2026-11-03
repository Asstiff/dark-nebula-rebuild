public class IAPItemIWantItAll : IAPItem
{
	public override void GiveItem()
	{
		UserPurchases.Instance.HasUnlockAll = true;
		AvatarManager.Instance.UnlockAllAvatars();
		BoosterManager.UnlockAllBoosters();
		for (int i = 1; i <= 23; i++)
		{
			ContentGatingManager.Instance.UnlockGate(i);
		}
	}
}
