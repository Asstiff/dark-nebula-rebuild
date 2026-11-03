public class IAPItemHardCurrency : IAPItem
{
	public int HardCurrencyAmount;

	public override void GiveItem()
	{
		HardCurrencyManager.Instance.Earn(HardCurrencyAmount);
	}
}
