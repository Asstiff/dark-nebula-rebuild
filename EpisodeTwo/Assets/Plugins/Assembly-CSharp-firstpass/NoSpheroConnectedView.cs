using UnityEngine;

public class NoSpheroConnectedView : MonoBehaviour
{
	public string m_NextLevel;

	public GUISkin m_GetASpheroButtonSkin;

	public GUISkin m_ConnectButtonSkin;

	public Texture2D m_Background;

	public string m_GetASpheroURL = "http://gosphero.com";

	public Texture2D m_GetASpheroTexture;

	public Texture2D m_ConnectTexture;

	private float m_BackgroundAspectRatio;

	public Texture2D m_Spinner;

	private Vector2 m_SpinnerSize = new Vector2(128f, 128f);

	private float m_SpinnerAngle;

	private Vector2 m_SpinnerPosition = new Vector2(0f, 0f);

	private Vector2 m_SpinnerPivotPos = new Vector2(0f, 0f);

	private Rect m_SpinnerRect;

	private int m_ViewPadding = 20;

	private void Start()
	{
		ViewSetup();
	}

	private void ViewSetup()
	{
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
		}
		else
		{
			ViewSetup();
		}
	}

	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification spheroDeviceNotification = (SpheroDeviceNotification)eventArgs.Message;
		if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED)
		{
			Application.LoadLevel(m_NextLevel);
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		GUI.skin = m_GetASpheroButtonSkin;
		int num = 0;
		int num2 = 0;
		m_BackgroundAspectRatio = (float)m_Background.height / (float)m_Background.width;
		if (m_BackgroundAspectRatio > (float)Screen.height / (float)Screen.width)
		{
			num2 = Screen.height - m_ViewPadding * 2;
			num = (int)((float)num2 / (float)m_Background.height * (float)m_Background.width);
		}
		else
		{
			num = Screen.width - m_ViewPadding * 2;
			num2 = (int)((float)num / (float)m_Background.width * (float)m_Background.height);
		}
		int num3 = Screen.width / 2 - num / 2;
		int num4 = Screen.height / 2 - num2 / 2;
		Rect position = new Rect(num3, num4, num, num2);
		GUI.DrawTexture(position, m_Background);
		float width = (float)num * 0.3f;
		float num5 = (float)num * 0.25f / (float)m_GetASpheroTexture.width * (float)m_GetASpheroTexture.height;
		float left = (float)num3 + (float)num * 0.605f;
		float top = (float)num4 + (float)num2 * 0.85f - num5 / 2f;
		if (GUI.Button(new Rect(left, top, width, num5), string.Empty))
		{
			Application.OpenURL(m_GetASpheroURL);
		}
		GUI.skin = m_ConnectButtonSkin;
		width = (float)num * 0.3f;
		num5 = (float)num * 0.25f / (float)m_GetASpheroTexture.width * (float)m_GetASpheroTexture.height;
		left = (float)num3 + (float)num * 0.1f;
		top = (float)num4 + (float)num2 * 0.85f - num5 / 2f;
		if (GUI.Button(new Rect(left, top, width, num5), string.Empty))
		{
			Application.LoadLevel("SpheroConnectionScene");
		}
	}
}
