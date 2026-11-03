using System.Collections;
using FLD;
using UnityEngine;

public class FlipperButton : MonoBehaviour
{
	public delegate void HandlerDelegate(bool value);

	private const float originalSpacing = -7f;

	private const float zeroSpacing = -19f;

	public DynamicFont Title;

	public DynamicFont Value;

	public GameObject ButtonLeft;

	public GameObject ButtonRight;

	public GameObject Overlay;

	private string _on = "ON";

	private string _off = "OFF";

	private bool _value;

	private bool _enabled = true;

	public bool LastTapWasLeft;

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			ButtonLeft.GetComponent<SimpleButton>().enabled = value;
			ButtonRight.GetComponent<SimpleButton>().enabled = value;
			Overlay.GetComponent<Renderer>().enabled = !value;
		}
	}

	public event HandlerDelegate EventValueChanged;

	private void Start()
	{
		ButtonLeft.GetComponent<SimpleButton>().ButtonUpEvent += OnToggleLeft;
		ButtonRight.GetComponent<SimpleButton>().ButtonUpEvent += OnToggleRight;
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
		foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
		{
			Align(tk2dSprite2.gameObject);
		}
		Enabled = _enabled;
	}

	private void Align(GameObject go)
	{
		Vector3 position = go.transform.position;
		position.x = Mathf.Round(position.x * 2f) * 0.5f;
		position.y = Mathf.Round(position.y * 2f) * 0.5f;
		go.transform.position = position;
	}

	public void UseTitleOnly()
	{
		ButtonLeft.SetActive(false);
		ButtonRight.SetActive(false);
		tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
		foreach (tk2dSprite tk2dSprite2 in componentsInChildren)
		{
			tk2dSprite2.gameObject.GetComponent<Renderer>().enabled = false;
		}
		Value.gameObject.SetActive(false);
		Title.GetComponent<Renderer>().enabled = true;
		Title.transform.Translate(Vector3.down * 20f);
	}

	public void Init(bool value, string onStringID, string offStringID, string titleStringID)
	{
		_value = value;
		_on = onStringID;
		_off = offStringID;
		Title.Text = titleStringID.Localize();
		if (_value)
		{
			Value.Text = _on.Localize();
		}
		else
		{
			Value.Text = _off.Localize();
		}
	}

	private void OnToggleLeft(SimpleButton source)
	{
		OnToggle(true);
	}

	private void OnToggleRight(SimpleButton source)
	{
		OnToggle(false);
	}

	private void OnToggle(bool isLeft)
	{
		_value = !_value;
		LastTapWasLeft = isLeft;
		if (_value)
		{
			Value.Text = _on.Localize();
		}
		else
		{
			Value.Text = _off.Localize();
		}
		if (EventValueChanged != null)
		{
			EventValueChanged(_value);
		}
	}

	private IEnumerator AnimateSpacing()
	{
		float begin = Time.realtimeSinceStartup;
		float duration = 0.2f;
		while (begin + duration > Time.realtimeSinceStartup)
		{
			float elapsed = Time.realtimeSinceStartup - begin;
			float percent = elapsed / duration;
			Value.GetComponent<SimpleText>().Spacing = -19f + 12f * percent;
			yield return null;
		}
		Value.GetComponent<SimpleText>().Spacing = -7f;
	}
}
