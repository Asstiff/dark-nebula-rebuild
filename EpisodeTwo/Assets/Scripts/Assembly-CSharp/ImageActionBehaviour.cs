using UnityEngine;

public class ImageActionBehaviour : MonoBehaviour
{
	private float showTime;

	private bool isDone;

	private GameObject Image;

	private float duration;

	private CameraFade _fade;

	public SimpleText[] texts;

	public Camera HudCamera;

	private bool isFading;

	private Renderer background;
	private Vector3 backgroundScale;
	private Vector3 backgroundPosition;
	private float fittedSize = -1f;
	private float fittedAspect = -1f;

	private void Awake()
	{
		SetTextVisibility(false);
		base.gameObject.SetActive(false);
		_fade = base.gameObject.AddComponent<CameraFade>();
	}

	public void Show(string imageName)
	{
		showTime = Time.realtimeSinceStartup;
		isDone = false;
		switch (imageName)
		{
		case "instructions":
		case "calibration":
			SetTextVisibility(true);
			UpdateImage(imageName);
			duration = 5f;
			if (imageName == "calibration")
			{
				tk2dFontData font = texts[3].GetComponent<tk2dTextMesh>().font;
				texts[1].GetComponent<tk2dTextMesh>().font = font;
				texts[2].GetComponent<tk2dTextMesh>().font = font;
				texts[0].SetStringID("CALIBRATE CONTROLS");
				texts[1].SetStringID("CALIBRATE CONTROLS_INSTRUCTIONS1");
				texts[2].SetStringID("CALIBRATE CONTROLS_INSTRUCTIONS2");
				texts[3].SetStringID("CALIBRATE CONTROLS_INSTRUCTIONS3");
				texts[4].SetStringID("CALIBRATE CONTROLS_INSTRUCTIONS4");
				texts[5].SetStringID("CALIBRATE CONTROLS_INSTRUCTIONS5");
				for (int i = 1; i < 6; i++)
				{
					texts[i].transform.position = texts[0].transform.position + new Vector3(0f, i * -20 - 20, 0f);
				}
			}
			break;
		default:
			SetTextVisibility(false);
			UpdateImage(imageName);
			duration = 3f;
			break;
		}
		base.gameObject.SetActive(true);
		isFading = false;
		_fade.FadeIn();
		Hud.ImageActionRunning = true;
	}

	private void SetTextVisibility(bool pVisible)
	{
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			base.gameObject.transform.GetChild(i).gameObject.SetActive(pVisible);
		}
	}

	protected void UpdateImage(string imageName)
	{
		if (Image != null)
		{
			Object.Destroy(Image);
		}
		Image = Object.Instantiate(LevelAssets.GetAsset(imageName).gameObject) as GameObject;
		if (Image != null)
		{
			Image.transform.parent = base.transform;
			Image.transform.localPosition = Vector3.zero;
			Image.SetActive(true);
			World.hud.ShowHudIcons(false);
			Transform backdrop = Image.transform.Find("Background");
			background = backdrop != null ? backdrop.GetComponent<Renderer>() : null;
			if (background != null)
			{
				backgroundScale = background.transform.localScale;
				backgroundPosition = background.transform.localPosition;
			}
			FixAspect();
		}
	}

	private void FixAspect()
	{
		if (Image == null || HudCamera == null) return;
		if (background != null)
		{
			// Instructions/calibration are a hierarchy, not one renderer. Fill only
			// their scenery; retain the phone, arrows, labels and icons at native scale.
			Transform backdrop = background.transform;
			backdrop.localScale = backgroundScale;
			backdrop.localPosition = backgroundPosition;
			Bounds bounds = background.bounds;
			float height = HudCamera.orthographicSize * 2f;
			float width = height * HudCamera.aspect;
			float cover = Mathf.Max(width / Mathf.Max(bounds.size.x, 0.01f), height / Mathf.Max(bounds.size.y, 0.01f));
			backdrop.localScale = Vector3.Scale(backgroundScale, new Vector3(cover, cover, 1f));
			Vector3 position = backdrop.position;
			bounds = background.bounds;
			position.x += HudCamera.transform.position.x - bounds.center.x;
			position.y += HudCamera.transform.position.y - bounds.center.y;
			backdrop.position = position;
		}
		else if (Image.GetComponent<Renderer>() != null)
		{
			Image.transform.localScale = Vector3.one;
			Vector3 size = Image.GetComponent<Renderer>().bounds.size;
			if (size.x == 0f || size.y == 0f)
			{
				size.x = 256f;
				size.y = 512f;
			}
			Vector3 vector = HudCamera.ScreenToWorldPoint(Vector3.zero);
			Vector3 vector2 = HudCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
			Vector2 vector3 = new Vector2(vector2.x - vector.x, vector2.y - vector.y);
			Image.transform.localScale = new Vector3(vector3.x / size.x, vector3.y / size.y, 1f);
		}
		fittedSize = HudCamera.orthographicSize;
		fittedAspect = HudCamera.aspect;
	}

	private void LateUpdate()
	{
		if (Image != null && HudCamera != null &&
			(!Mathf.Approximately(fittedSize, HudCamera.orthographicSize) || !Mathf.Approximately(fittedAspect, HudCamera.aspect))) FixAspect();
	}

	private void Update()
	{
		if (!isFading && Time.realtimeSinceStartup > showTime + duration)
		{
			_fade.FadeOut();
			isFading = true;
		}
		if (!isDone && Time.realtimeSinceStartup > showTime + duration + 0.5f)
		{
			if (Image != null)
			{
				Object.Destroy(Image);
			}
			base.gameObject.SetActive(false);
			isDone = true;
			Hud.ImageActionRunning = false;
			Hud.Instance.ShowHudIcons();
		}
	}

	public bool IsDone()
	{
		return isDone;
	}
}
