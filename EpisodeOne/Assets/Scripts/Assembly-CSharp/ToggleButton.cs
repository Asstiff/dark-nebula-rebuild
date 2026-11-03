using UnityEngine;

public class ToggleButton : MonoBehaviour
{
	public delegate void ButtonHandlerDelegate(ToggleButton source);

	public Camera viewCamera;

	public string buttonOnSprite;

	public string buttonOffSprite;

	private int _buttonOnSpriteId;

	private int _buttonOffSpriteId;

	private bool _toogleValue;

	private tk2dBaseSprite _sprite;

	private bool isButtonDown;

	private bool buttonDownOnMe;

	private tk2dBaseSprite sprite
	{
		get
		{
			if (_sprite == null)
			{
				_sprite = GetComponentInChildren<tk2dBaseSprite>();
			}
			return _sprite;
		}
	}

	public event ButtonHandlerDelegate ButtonUpEvent;

	public void Start()
	{
		if (viewCamera == null)
		{
			Transform parent = base.transform;
			while ((bool)parent && parent.GetComponent<Camera>() == null)
			{
				parent = parent.parent;
			}
			if ((bool)parent && parent.GetComponent<Camera>() != null)
			{
				viewCamera = parent.GetComponent<Camera>();
			}
			if (viewCamera == null)
			{
				GameObject gameObject = GameObject.Find("Hud Camera");
				if (gameObject != null)
				{
					viewCamera = gameObject.GetComponent<Camera>();
				}
			}
			if (viewCamera == null)
			{
				viewCamera = Camera.main;
			}
		}
		if (sprite != null)
		{
			if (buttonOnSprite.Length > 0)
			{
				_buttonOnSpriteId = _sprite.GetSpriteIdByName(buttonOnSprite);
			}
			if (buttonOffSprite.Length > 0)
			{
				_buttonOffSpriteId = _sprite.GetSpriteIdByName(buttonOffSprite);
			}
		}
		if (base.GetComponent<Collider>() == null)
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			Vector3 size = boxCollider.size;
			size.z = 0.2f;
			boxCollider.size = size;
		}
		SimpleText componentInChildren = GetComponentInChildren<SimpleText>();
		if (componentInChildren != null)
		{
			componentInChildren.UseCurrentTextAsStringID();
		}
		SetToggle(_toogleValue);
	}

	public void SetToggle(bool on)
	{
		_toogleValue = on;
		sprite.spriteId = ((!_toogleValue) ? _buttonOffSpriteId : _buttonOnSpriteId);
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			if (!isButtonDown)
			{
				isButtonDown = true;
				Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				if (base.GetComponent<Collider>().Raycast(ray, out hitInfo, 100000000f) && hitInfo.collider == base.GetComponent<Collider>())
				{
					buttonDownOnMe = true;
				}
			}
			return;
		}
		if (isButtonDown && buttonDownOnMe)
		{
			Ray ray2 = viewCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo2;
			if (base.GetComponent<Collider>().Raycast(ray2, out hitInfo2, 100000000f) && hitInfo2.collider == base.GetComponent<Collider>() && ButtonUpEvent != null)
			{
				ButtonUpEvent(this);
			}
		}
		isButtonDown = false;
		buttonDownOnMe = false;
	}
}
