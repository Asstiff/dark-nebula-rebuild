using FLD;
using UnityEngine;

public class SimpleButton : MonoBehaviour
{
	public delegate void ButtonHandlerDelegate(SimpleButton source);

	public Camera viewCamera;

	public string buttonDownSprite = "button_down";

	public string buttonUpSprite = "button_up";

	public string buttonPressedSprite = "button_up";

	private int buttonDownSpriteId = -1;

	private int buttonUpSpriteId = -1;

	private int buttonPressedSpriteId = -1;

	public AudioClip buttonDownSound;

	public AudioClip buttonUpSound;

	public AudioClip buttonPressedSound;

	public GameObject targetObject;

	public string messageName = string.Empty;

	private tk2dBaseSprite sprite;

	private bool isButtonDown;

	private static int audioIndex;

	private bool _buttonHasFocus;

	public event ButtonHandlerDelegate ButtonPressedEvent;

	public event ButtonHandlerDelegate ButtonAutoFireEvent;

	public event ButtonHandlerDelegate ButtonDownEvent;

	public event ButtonHandlerDelegate ButtonUpEvent;

	public virtual void Start()
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
		sprite = GetComponent<tk2dBaseSprite>();
		if ((bool)sprite)
		{
			if (buttonDownSprite.Length > 0)
			{
				buttonDownSpriteId = sprite.GetSpriteIdByName(buttonDownSprite);
			}
			if (buttonUpSprite.Length > 0)
			{
				buttonUpSpriteId = sprite.GetSpriteIdByName(buttonUpSprite);
			}
			if (buttonPressedSprite.Length > 0)
			{
				buttonPressedSpriteId = sprite.GetSpriteIdByName(buttonPressedSprite);
			}
		}
		if (base.GetComponent<Collider>() == null)
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			Vector3 extents = (boxCollider.size * 0.5f);
			extents.z = 0.2f;
			boxCollider.size = (extents) * 2f;
		}
	}

	private void PlaySound(AudioClip source)
	{
		if ((bool)source)
		{
			Audio.Play("button" + audioIndex, source);
			audioIndex = (audioIndex + 1) % 5;
		}
	}

	private void Update()
	{
#if UNITY_EDITOR
        if(UnityEditor.SessionState.GetBool("WideReview",false))return;
#endif
		if (Input.GetMouseButton(0))
		{
			Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (base.GetComponent<Collider>().Raycast(ray, out hitInfo, 100000000f))
			{
				if (!(hitInfo.collider == base.GetComponent<Collider>()))
				{
					return;
				}
				if (!isButtonDown)
				{
					if (Input.GetMouseButtonDown(0))
					{
						_buttonHasFocus = true;
						isButtonDown = true;
						OnButtonDown();
					}
				}
				else if (_buttonHasFocus)
				{
					OnButtonPressed();
				}
			}
			else
			{
				SetNormalSprite();
			}
			return;
		}
		if (isButtonDown && _buttonHasFocus)
		{
			Ray ray2 = viewCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo2;
			if (base.GetComponent<Collider>().Raycast(ray2, out hitInfo2, 100000000f))
			{
				if (hitInfo2.collider == base.GetComponent<Collider>())
				{
					OnButtonUp();
				}
			}
			else
			{
				SetNormalSprite();
			}
			_buttonHasFocus = false;
		}
		isButtonDown = false;
	}

	public virtual void OnButtonDown()
	{
		PlaySound(buttonDownSound);
		if (buttonDownSpriteId != -1)
		{
			sprite.spriteId = buttonDownSpriteId;
		}
		if (ButtonDownEvent != null)
		{
			ButtonDownEvent(this);
		}
	}

	private void OnButtonPressed()
	{
		if (ButtonAutoFireEvent != null)
		{
			ButtonAutoFireEvent(this);
		}
		PlaySound(buttonPressedSound);
		if (buttonPressedSpriteId != -1)
		{
			sprite.spriteId = buttonPressedSpriteId;
		}
		if ((bool)targetObject)
		{
			targetObject.SendMessage(messageName);
		}
		if (ButtonPressedEvent != null)
		{
			ButtonPressedEvent(this);
		}
	}

	public void OnButtonUp()
	{
		PlaySound(buttonUpSound);
		if (buttonUpSpriteId != -1)
		{
			sprite.spriteId = buttonUpSpriteId;
		}
		if (ButtonUpEvent != null)
		{
			ButtonUpEvent(this);
		}
	}

	private void SetNormalSprite()
	{
		if (buttonUpSpriteId != -1)
		{
			sprite.spriteId = buttonUpSpriteId;
		}
	}
}
