using System;
using System.Collections;
using UnityEngine;

public class HardCurrencyBalance : MonoBehaviour
{
	[SerializeField]
	private bool _hideAddButton;

	[SerializeField]
	private DynamicFont _balanceText;

	[SerializeField]
	private SimpleButton _addHCButton;

	[SerializeField]
	private Transform _background;

	private bool _hidden;

	private void Awake()
	{
		if (true) // No in-app purchase entry in the offline build.
		{
			_addHCButton.gameObject.SetActive(false);
			_background.localPosition = new Vector3(_background.localPosition.x - 50f, _background.localPosition.y, _background.localPosition.z);
		}
	}

	private void Start()
	{
		if (UserPurchases.Instance.HasUnlockAll)
		{
			HideAll();
			return;
		}
		_addHCButton.ButtonUpEvent += HandleButtonUpEvent;
		HardCurrencyManager instance = HardCurrencyManager.Instance;
		instance.BalanceChangedEvent = (Action)Delegate.Combine(instance.BalanceChangedEvent, new Action(HandleBalanceChanged));
		HandleBalanceChanged();
	}

	private void HideAll()
	{
		_hidden = true;
		base.gameObject.SetActive(false);
	}

	private void HandleBalanceChanged()
	{
		_balanceText.Text = HardCurrencyManager.Instance.Amount.ToString();
	}

	private void HandleButtonUpEvent(SimpleButton source)
	{
		IAPShopScreen.PreviousScene = Application.loadedLevelName;
		StartCoroutine(LoadStore());
	}

	private void OnDestroy()
	{
		if (!_hidden)
		{
			_addHCButton.ButtonUpEvent -= HandleButtonUpEvent;
			HardCurrencyManager instance = HardCurrencyManager.Instance;
			instance.BalanceChangedEvent = (Action)Delegate.Remove(instance.BalanceChangedEvent, new Action(HandleBalanceChanged));
		}
	}

	private IEnumerator LoadStore()
	{
		CameraFade fade = UnityEngine.Object.FindObjectOfType<CameraFade>();
		if (fade != null)
		{
			fade.FadeOut();
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			Debug.LogWarning("Found no camera fade. There should be one...");
		}
		Application.LoadLevel("IAPShop");
	}
}
