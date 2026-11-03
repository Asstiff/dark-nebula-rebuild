using UnityEngine;

public class TestIAPScreen : MonoBehaviour
{
	[SerializeField]
	private SimpleButton _backbutton;

	[SerializeField]
	private SimpleButton _shopButton;

	private void Start()
	{
		_backbutton.ButtonUpEvent += (SimpleButton source) =>
		{
			World.LoadLevel("MainMenu", 0.5f);
		};
		_shopButton.ButtonUpEvent += (SimpleButton source) =>
		{
			World.LoadLevel("IAPShop", 0.5f);
		};
	}
}
