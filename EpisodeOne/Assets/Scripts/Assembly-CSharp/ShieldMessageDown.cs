using FLD;
using UnityEngine;

public class ShieldMessageDown : Entity
{
	[SerializeField]
	private DynamicFont _text;

	public void SetStringID(string stringID)
	{
		_text.Text = stringID.Localize();
	}
}
