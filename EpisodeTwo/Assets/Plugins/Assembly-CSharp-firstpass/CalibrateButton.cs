using System;
using UnityEngine;

public class CalibrateButton : MonoBehaviour
{
	private int lastFingerId = -1;

	private bool firstTouch = true;

	private bool isCalibrating;

	private int currentHeading;

	public float buttonScale = 0.1f;

	public float ringScale = 0.7f;

	public LegacyGUITexture buttonBackground;

	public LegacyGUITexture buttonForeground;

	public LegacyGUITexture innerRing;

	public LegacyGUITexture middleRing;

	public LegacyGUITexture outerRing;

	private Rect ringRect;

	private Rect guiOuterRingRect;

	private Rect guiInnerRingRect;

	private Vector2 ringCenter;

	private Vector2 guiRingCenter;

	private Rect buttonRect;

	private Vector2 buttonCenter;

	private Vector2 startScreenSize;

	private Vector3 originalTransformPos;

	private Sphero[] m_Spheros;

	private void Start()
	{
		m_Spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		startScreenSize = new Vector2(Screen.width, Screen.height);
		originalTransformPos = base.transform.position;
		SetCalibrateButtonSize();
	}

	private void Disable()
	{
		base.gameObject.active = false;
	}

	private void SetCalibrateButtonSize()
	{
		base.transform.position = new Vector3(0f, 0f, 0f);
		SetRingsVisibility(false);
		innerRing.gameObject.active = false;
		outerRing.gameObject.active = false;
		float num = Mathf.Min(Screen.width, Screen.height);
		float num2 = num * buttonScale;
		Vector2 vector = new Vector2(originalTransformPos.x * (float)Screen.width, originalTransformPos.y * (float)Screen.height);
		Rect pixelInset = new Rect(vector.x - num2 * 0.5f, vector.y - num2 * 0.5f, num2, num2);
		buttonBackground.pixelInset = pixelInset;
		buttonForeground.pixelInset = pixelInset;
		float num3 = num * ringScale;
		Rect pixelInset2 = new Rect(vector.x - num3 * 0.5f, pixelInset.y, num3, num3);
		outerRing.pixelInset = pixelInset2;
		guiOuterRingRect = new Rect(vector.x - num3 * 0.5f, (float)Screen.height - pixelInset.y - num3, num3, num3);
		float num4 = num3 * 0.88235f;
		middleRing.pixelInset = new Rect(vector.x - num4 * 0.5f, pixelInset.y + (num3 - num4) * 0.5f, num4, num4);
		float num5 = num3 * 0.9f;
		innerRing.pixelInset = new Rect(vector.x - num5 * 0.5f, pixelInset.y + (num3 - num5) * 0.5f, num5, num5);
		guiInnerRingRect = new Rect(vector.x - num5 * 0.5f, (float)Screen.height - (pixelInset.y + (num3 - num5) * 0.5f) - num5, num5, num5);
		ringCenter = new Vector2(pixelInset2.x + pixelInset2.width * 0.5f, pixelInset2.y + pixelInset2.height * 0.5f);
		guiRingCenter = new Vector2(pixelInset2.x + pixelInset2.width * 0.5f, (float)Screen.height - pixelInset2.y - pixelInset2.height * 0.5f);
		buttonCenter = new Vector2(pixelInset.x + pixelInset.width * 0.5f, pixelInset.y + pixelInset.height * 0.5f);
	}

	private void SetRingsVisibility(bool visible)
	{
		middleRing.gameObject.active = visible;
	}

	private bool IsFingerDown()
	{
		return lastFingerId != -1;
	}

	private void Update()
	{
		if ((float)Screen.width != startScreenSize.x || (float)Screen.height != startScreenSize.y)
		{
			SetCalibrateButtonSize();
			startScreenSize = new Vector2(Screen.width, Screen.height);
		}
		int touchCount = Input.touchCount;
		if (touchCount == 0)
		{
			if (isCalibrating)
			{
				Sphero[] spheros = m_Spheros;
				foreach (Sphero sphero in spheros)
				{
					sphero.SetBackLED(0f);
					sphero.SetHeading(0);
				}
			}
			isCalibrating = false;
			firstTouch = true;
			SetRingsVisibility(false);
			return;
		}
		for (int j = 0; j < touchCount; j++)
		{
			Touch touch = Input.GetTouch(j);
			bool flag = false;
			if (buttonBackground.HitTest(touch.position) && firstTouch)
			{
				isCalibrating = true;
				Sphero[] spheros2 = m_Spheros;
				foreach (Sphero sphero2 in spheros2)
				{
					sphero2.SetBackLED(1f);
					sphero2.SetHeading(0);
				}
				SetRingsVisibility(true);
				flag = true;
			}
			if (flag && (lastFingerId == -1 || lastFingerId != touch.fingerId))
			{
				lastFingerId = touch.fingerId;
			}
			if (lastFingerId == touch.fingerId && isCalibrating)
			{
				Vector2 vector = touch.position - ringCenter;
				float num = Mathf.Atan2(vector.y, vector.x);
				num = (num + 7.853982f) % ((float)Math.PI * 2f);
				float num2 = 360f - 57.29578f * num;
				currentHeading = (int)num2;
				Sphero[] spheros3 = m_Spheros;
				foreach (Sphero sphero3 in spheros3)
				{
					sphero3.Roll(currentHeading, 0f);
				}
			}
			firstTouch = false;
		}
	}

	private void OnGUI()
	{
		if (isCalibrating)
		{
			GUI.Box(new Rect(0f, 0f, Screen.width, Screen.height), string.Empty);
			Matrix4x4 matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(currentHeading, guiRingCenter);
			GUI.DrawTexture(guiOuterRingRect, outerRing.texture);
			GUI.matrix = matrix;
			GUIUtility.RotateAroundPivot(-currentHeading, guiRingCenter);
			GUI.DrawTexture(guiInnerRingRect, innerRing.texture);
		}
	}
}
