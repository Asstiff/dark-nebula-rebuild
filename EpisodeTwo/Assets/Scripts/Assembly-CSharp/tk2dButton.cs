using System.Collections;
using UnityEngine;

[AddComponentMenu("2D Toolkit/GUI/tk2dButton")]
public class tk2dButton : MonoBehaviour
{
	public delegate void ButtonHandlerDelegate(tk2dButton source);

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

	private bool buttonDown;

	public float targetScale = 1.1f;

	public float scaleTime = 0.05f;

	public float pressedWaitTime = 0.3f;

	public event ButtonHandlerDelegate ButtonPressedEvent;

	public event ButtonHandlerDelegate ButtonAutoFireEvent;

	public event ButtonHandlerDelegate ButtonDownEvent;

	public event ButtonHandlerDelegate ButtonUpEvent;

	private void OnEnable()
	{
		buttonDown = false;
	}

	private void Start()
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
			if (viewCamera == null && (bool)tk2dCamera.inst)
			{
				viewCamera = tk2dCamera.inst.mainCamera;
			}
			if (viewCamera == null)
			{
				viewCamera = Camera.main;
			}
		}
		sprite = GetComponent<tk2dBaseSprite>();
		if ((bool)sprite)
		{
			UpdateSpriteIds();
		}
		if (base.GetComponent<Collider>() == null)
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			Vector3 extents = (boxCollider.size * 0.5f);
			extents.z = 0.2f;
			boxCollider.size = (extents) * 2f;
		}
		if ((buttonDownSound != null || buttonPressedSound != null || buttonUpSound != null) && base.GetComponent<AudioSource>() == null)
		{
			AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}
	}

	public void UpdateSpriteIds()
	{
		buttonDownSpriteId = ((buttonDownSprite.Length <= 0) ? (-1) : sprite.GetSpriteIdByName(buttonDownSprite));
		buttonUpSpriteId = ((buttonUpSprite.Length <= 0) ? (-1) : sprite.GetSpriteIdByName(buttonUpSprite));
		buttonPressedSpriteId = ((buttonPressedSprite.Length <= 0) ? (-1) : sprite.GetSpriteIdByName(buttonPressedSprite));
	}

	private void PlaySound(AudioClip source)
	{
		if ((bool)base.GetComponent<AudioSource>() && (bool)source)
		{
			base.GetComponent<AudioSource>().PlayOneShot(source);
		}
	}

	private IEnumerator coScale(Vector3 defaultScale, float startScale, float endScale)
	{
		float t0 = Time.realtimeSinceStartup;
		Vector3 scale = defaultScale;
		for (float s = 0f; s < scaleTime; s = Time.realtimeSinceStartup - t0)
		{
			float t1 = Mathf.Clamp01(s / scaleTime);
			float scl = Mathf.Lerp(startScale, endScale, t1);
			scale = defaultScale * scl;
			base.transform.localScale = scale;
			yield return 0;
		}
		base.transform.localScale = defaultScale * endScale;
	}

	private IEnumerator LocalWaitForSeconds(float seconds)
	{
		float t0 = Time.realtimeSinceStartup;
		for (float s = 0f; s < seconds; s = Time.realtimeSinceStartup - t0)
		{
			yield return 0;
		}
	}

	private IEnumerator coHandleButtonPress()
	{
		buttonDown = true;
		bool buttonPressed = true;
		Vector3 defaultScale = base.transform.localScale;
		if (targetScale != 1f)
		{
			yield return StartCoroutine(coScale(defaultScale, 1f, targetScale));
		}
		PlaySound(buttonDownSound);
		if (buttonDownSpriteId != -1)
		{
			sprite.spriteId = buttonDownSpriteId;
		}
		if (ButtonDownEvent != null)
		{
			ButtonDownEvent(this);
		}
		while (Input.GetMouseButton(0))
		{
			Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			bool colliderHit = base.GetComponent<Collider>().Raycast(ray, out hitInfo, 100000000f);
			if (buttonPressed && !colliderHit)
			{
				if (targetScale != 1f)
				{
					yield return StartCoroutine(coScale(defaultScale, targetScale, 1f));
				}
				PlaySound(buttonUpSound);
				if (buttonUpSpriteId != -1)
				{
					sprite.spriteId = buttonUpSpriteId;
				}
				if (ButtonUpEvent != null)
				{
					ButtonUpEvent(this);
				}
				buttonPressed = false;
			}
			else if (!buttonPressed & colliderHit)
			{
				if (targetScale != 1f)
				{
					yield return StartCoroutine(coScale(defaultScale, 1f, targetScale));
				}
				PlaySound(buttonDownSound);
				if (buttonDownSpriteId != -1)
				{
					sprite.spriteId = buttonDownSpriteId;
				}
				if (ButtonDownEvent != null)
				{
					ButtonDownEvent(this);
				}
				buttonPressed = true;
			}
			if (buttonPressed && ButtonAutoFireEvent != null)
			{
				ButtonAutoFireEvent(this);
			}
			yield return 0;
		}
		if (buttonPressed)
		{
			if (targetScale != 1f)
			{
				yield return StartCoroutine(coScale(defaultScale, targetScale, 1f));
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
			if (ButtonUpEvent != null)
			{
				ButtonUpEvent(this);
			}
			if (ButtonPressedEvent != null)
			{
				ButtonPressedEvent(this);
			}
			if (base.gameObject.active)
			{
				yield return StartCoroutine(LocalWaitForSeconds(pressedWaitTime));
			}
			if (buttonUpSpriteId != -1)
			{
				sprite.spriteId = buttonUpSpriteId;
			}
		}
		buttonDown = false;
	}

	private void Update()
	{
		if (!buttonDown && Input.GetMouseButtonDown(0))
		{
			Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (base.GetComponent<Collider>().Raycast(ray, out hitInfo, 100000000f) && !Physics.Raycast(ray, hitInfo.distance - 0.01f))
			{
				StartCoroutine(coHandleButtonPress());
			}
		}
	}
}
