using System.Text;

public class PurchaseItem
{
	public string productId;

	public int quantity;

	public string title;

	public string description;

	public string price = string.Empty;

	private char _delim = '|';

	public override string ToString()
	{
		return new StringBuilder().Append(productId).Append(_delim).Append(quantity)
			.Append(_delim)
			.Append(title)
			.Append(_delim)
			.Append(description)
			.Append(_delim)
			.Append(price)
			.ToString();
	}

	public void Unpack(string packedString)
	{
		string[] array = packedString.Split(_delim);
		productId = array[0];
		quantity = int.Parse(array[1]);
		title = array[2];
		description = array[3];
		price = array[4];
	}
}
