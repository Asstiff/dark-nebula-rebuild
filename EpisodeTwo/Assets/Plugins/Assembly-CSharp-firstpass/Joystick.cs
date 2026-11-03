using System;
using UnityEngine;

public class Joystick : MonoBehaviour
{
	private struct Boundary
	{
		public Vector2 min;

		public Vector2 max;
	}

	private int lastFingerId = -1;

	private bool firstTouch = true;

	public float velocityScale = 0.6f;

	public LegacyGUITexture puck;

	public LegacyGUITexture background;

	public float joystickScale;

	private Rect defaultRect;

	private Rect defaultBackRect;

	private Boundary guiBoundary;

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;

	private bool stopped = true;

	private float lastHeading;

	private Vector2 startScreenSize;

	private Vector3 originalTransformPos;

	private Sphero[] m_Spheros;

	private void Start()
	{
		m_Spheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
		startScreenSize = new Vector2(Camera.main.pixelRect.width, Camera.main.pixelRect.height);
		originalTransformPos = base.transform.position;
		SetJoystickSize();
	}

	private void Disable()
	{
		base.gameObject.active = false;
	}

	private void ResetJoystick()
	{
		puck.pixelInset = defaultRect;
		background.pixelInset = defaultBackRect;
		lastFingerId = -1;
	}

	private void SetJoystickSize()
	{
		base.transform.position = originalTransformPos;
		float num = Mathf.Min(startScreenSize.x, startScreenSize.y) * joystickScale;
		float num2 = 0.32f;
		Rect pixelInset = new Rect((0f - num) * 0.5f, (0f - num) * 0.5f, num, num);
		background.pixelInset = pixelInset;
		Rect pixelInset2 = new Rect((0f - num) * 0.5f * num2, (0f - num) * 0.5f * num2, num * num2, num * num2);
		puck.pixelInset = pixelInset2;
		defaultRect = puck.pixelInset;
		defaultRect.x += base.transform.position.x * startScreenSize.x;
		defaultRect.y += base.transform.position.y * startScreenSize.y;
		defaultBackRect = background.pixelInset;
		defaultBackRect.x += base.transform.position.x * startScreenSize.x;
		defaultBackRect.y += base.transform.position.y * startScreenSize.y;
		base.transform.position = new Vector3(0f, 0f, 0f);
		guiTouchOffset.x = defaultRect.width * 0.5f;
		guiTouchOffset.y = defaultRect.height * 0.5f;
		guiCenter.x = defaultRect.x + guiTouchOffset.x;
		guiCenter.y = defaultRect.y + guiTouchOffset.y;
		guiBoundary.min.x = defaultBackRect.x;
		guiBoundary.max.x = defaultBackRect.x + defaultBackRect.width;
		guiBoundary.min.y = defaultBackRect.y;
		guiBoundary.max.y = defaultBackRect.y + defaultBackRect.height;
	}

	private bool IsFingerDown()
	{
		return lastFingerId != -1;
	}

	private void LatchedFinger(int fingerId)
	{
		if (lastFingerId == fingerId)
		{
			ResetJoystick();
		}
	}

	private void Update()
	{
		if (startScreenSize.x != Camera.main.pixelRect.width || startScreenSize.y != Camera.main.pixelRect.height)
		{
			startScreenSize = new Vector2(Camera.main.pixelRect.width, Camera.main.pixelRect.height);
			SetJoystickSize();
		}
		int touchCount = Input.touchCount;
		float num = 0f;
		float num2 = 0f;
		float num3 = (guiBoundary.max.y - guiBoundary.min.y) * 0.5f;
		if (touchCount == 0)
		{
			if (!stopped)
			{
				stopped = true;
				Sphero[] spheros = m_Spheros;
				foreach (Sphero sphero in spheros)
				{
					sphero.Roll((int)lastHeading, 0f);
				}
			}
			firstTouch = true;
			ResetJoystick();
			return;
		}
		for (int j = 0; j < touchCount; j++)
		{
			Touch touch = Input.GetTouch(j);
			Vector2 vector = touch.position - guiTouchOffset;
			bool flag = false;
			if (puck.HitTest(touch.position) && firstTouch)
			{
				flag = true;
			}
			if (flag && (lastFingerId == -1 || lastFingerId != touch.fingerId))
			{
				lastFingerId = touch.fingerId;
			}
			if (lastFingerId == touch.fingerId)
			{
				Vector2 vector2 = touch.position - guiTouchOffset;
				num = Mathf.Atan2(touch.position.y - guiCenter.y, touch.position.x - guiCenter.x);
				num2 = (touch.position - guiCenter).magnitude;
				if ((touch.position - guiCenter).magnitude > num3)
				{
					Vector2 vector3 = new Vector2(num3 * Mathf.Cos(num), num3 * Mathf.Sin(num));
					vector2 = guiCenter + vector3 - guiTouchOffset;
				}
				puck.pixelInset = new Rect(vector2.x, vector2.y, puck.pixelInset.width, puck.pixelInset.height);
				num = (num + (float)Math.PI * 2f) % ((float)Math.PI * 2f);
				num -= (float)Math.PI / 2f;
				if (num < 0f)
				{
					num += (float)Math.PI * 2f;
				}
				float num4 = 360f - 57.29578f * num;
				float num5 = num2 / num3;
				num5 *= velocityScale;
				if (num5 > velocityScale)
				{
					num5 = velocityScale;
				}
				if (num5 > 0f)
				{
					Sphero[] spheros2 = m_Spheros;
					foreach (Sphero sphero2 in spheros2)
					{
						sphero2.Roll((int)num4, num5);
					}
					stopped = false;
					lastHeading = num4;
				}
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					ResetJoystick();
				}
			}
			firstTouch = false;
		}
	}
}
