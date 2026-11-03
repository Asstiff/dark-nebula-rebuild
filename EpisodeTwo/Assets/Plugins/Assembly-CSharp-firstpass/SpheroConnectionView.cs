using UnityEngine;

public class SpheroConnectionView : MonoBehaviour
{
	private const float inertiaDuration = 0.5f;

	public string m_NextLevel;

	public bool m_MultipleSpheros;

	public GUISkin m_SpheroConnectionSkin;

	public Texture2D m_Spinner;

	private Vector2 m_SpinnerSize = new Vector2(128f, 128f);

	private float m_SpinnerAngle;

	private Vector2 m_SpinnerPosition = new Vector2(0f, 0f);

	private Vector2 m_SpinnerPivotPos = new Vector2(0f, 0f);

	private Rect m_SpinnerRect;

	public Texture2D m_CheckMark;

	public Texture2D m_RedX;

	public Texture2D m_SplashScreen;

	private int m_ViewPadding = 20;

	private int m_ElementPadding = 10;

	private int m_ButtonWidth = 200;

	private int m_ButtonHeight = 55;

	private int m_TitleHeight = 40;

	private string m_Title;

	private int m_SpheroLabelWidth = 250;

	private int m_SpheroLabelHeight = 60;

	private int m_SpheroLabelSelected = -1;

	private SpheroProvider m_SpheroProvider;

	private string[] m_RobotNames;

	private string m_ConnectingRobotName;

	private int selected = -1;

	private float scrollVelocity;

	private float timeTouchPhaseEnded;

	private Vector2 scrollPosition;

	private Vector2 windowMargin = new Vector2(0f, 0f);

	private Vector2 listMargin = new Vector2(40f, 40f);

	private Rect windowRect;

	private void ViewSetup()
	{
		m_SpheroProvider = SpheroProvider.GetSharedProvider();
		SetupAndroid();
	}

	private void Start()
	{
		ViewSetup();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
			m_SpheroProvider.DisconnectSpheros();
		}
		else
		{
			ViewSetup();
		}
	}

	private void SetupIOS()
	{
		m_MultipleSpheros = false;
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
		SpheroProvider.GetSharedProvider().Connect(0);
		Invoke("CheckForSpheroConnection", 1.5f);
	}

	private void CheckForSpheroConnection()
	{
		if (m_SpheroProvider.GetConnectedSpheros().Length == 0)
		{
			Application.LoadLevel("NoSpheroConnectedScene");
		}
	}

	private void SetupAndroid()
	{
		m_SpheroProvider = SpheroProvider.GetSharedProvider();
		if (!m_SpheroProvider.IsAdapterEnabled())
		{
			m_Title = "Bluetooth Not Enabled";
			m_RobotNames = new string[0];
			return;
		}
		m_Title = "Connect to a Sphero";
		m_SpheroProvider.FindRobots();
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
		if (m_MultipleSpheros)
		{
			m_SpinnerSize = new Vector2(m_SpheroLabelHeight - 10, m_SpheroLabelHeight - 10);
		}
		m_RobotNames = m_SpheroProvider.GetRobotNames();
		if (m_RobotNames.Length == 1)
		{
			m_SpheroProvider.PairedSpheros[0].ConnectionState = Sphero.Connection_State.Connecting;
			m_SpheroLabelSelected = 0;
			m_ConnectingRobotName = m_SpheroProvider.PairedSpheros[0].DeviceInfo.Name;
		}
	}

	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification spheroDeviceNotification = (SpheroDeviceNotification)eventArgs.Message;
		if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED)
		{
			if (!m_MultipleSpheros)
			{
				m_Title = "Connection Success";
				SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
				Application.LoadLevel(m_NextLevel);
			}
		}
		else if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTION_FAILED)
		{
			Sphero sphero = m_SpheroProvider.GetSphero(spheroDeviceNotification.RobotID);
			if (m_ConnectingRobotName.Equals(sphero.DeviceInfo.Name))
			{
				m_Title = "Connection Failed";
			}
		}
	}

	private void Update()
	{
		if (Input.touchCount != 1)
		{
			selected = -1;
			if (scrollVelocity != 0f)
			{
				float num = (Time.time - timeTouchPhaseEnded) / 0.5f;
				float num2 = Mathf.Lerp(scrollVelocity, 0f, num);
				scrollPosition.y += num2 * Time.deltaTime;
				if (num >= 0.5f)
				{
					scrollVelocity = 0f;
				}
			}
			return;
		}
		Touch touch = Input.touches[0];
		if (touch.phase == TouchPhase.Began)
		{
			selected = TouchToRowIndex(touch.position);
			scrollVelocity = 0f;
		}
		else if (touch.phase == TouchPhase.Canceled)
		{
			selected = -1;
		}
		else if (touch.phase == TouchPhase.Moved)
		{
			selected = -1;
			scrollPosition.y += touch.deltaPosition.y;
		}
		else
		{
			if (touch.phase != TouchPhase.Ended)
			{
				return;
			}
			if (selected > -1 && selected < m_RobotNames.Length)
			{
				Debug.Log("Player selected row " + selected);
				if (m_MultipleSpheros)
				{
					ConnectSphero(selected);
				}
			}
			else
			{
				if (Mathf.Abs(touch.deltaPosition.y) >= 10f)
				{
					scrollVelocity = (int)(touch.deltaPosition.y / touch.deltaTime);
				}
				timeTouchPhaseEnded = Time.time;
			}
		}
	}

	private void ConnectSphero(int row)
	{
		if (m_SpheroProvider.GetConnectingSphero() == null && m_SpheroProvider.PairedSpheros[row].ConnectionState != Sphero.Connection_State.Connected)
		{
			m_SpheroProvider.Connect(row);
			m_SpheroLabelSelected = row;
			m_Title = "Connecting to " + m_RobotNames[m_SpheroLabelSelected];
			m_ConnectingRobotName = m_RobotNames[m_SpheroLabelSelected];
		}
	}

	private void OnGUI()
	{
		if (m_RobotNames == null)
		{
			return;
		}
		GUI.skin = m_SpheroConnectionSkin;
		GUI.Label(new Rect(m_ViewPadding, m_ViewPadding, Screen.width - m_ViewPadding * 2, m_TitleHeight), m_Title, "label");
		if (m_SpheroProvider.GetConnectingSphero() != null)
		{
			GUI.enabled = false;
		}
		int num = m_ViewPadding + m_TitleHeight + m_ElementPadding * 2;
		windowMargin = new Vector2(m_ViewPadding, num);
		windowRect = new Rect(windowMargin.x, windowMargin.y - 15f, (float)Screen.width - 2f * windowMargin.x, (float)Screen.height - 2f * windowMargin.y);
		GUI.Window(0, windowRect, DoWindow, string.Empty);
		string text = "Connect";
		if (m_MultipleSpheros)
		{
			text = "Done";
		}
		int num2 = Screen.width / 2 - m_ButtonWidth / 2;
		int num3 = Screen.height - m_ViewPadding - m_ButtonHeight;
		if (m_MultipleSpheros && m_SpheroProvider.GetConnectedSpheros().Length == 0)
		{
			GUI.enabled = false;
		}
		if (GUI.Button(new Rect(num2, num3, m_ButtonWidth, m_ButtonHeight), text))
		{
			if (text.Equals("Done"))
			{
				SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
				Application.LoadLevel(m_NextLevel);
			}
			else if (m_SpheroLabelSelected >= 0)
			{
				ConnectSphero(m_SpheroLabelSelected);
			}
		}
		GUI.enabled = true;
		if (!m_MultipleSpheros && m_SpheroProvider.GetConnectingSphero() != null)
		{
			m_SpinnerPosition.x = Screen.width / 2;
			m_SpinnerPosition.y = Screen.height / 2;
			m_SpinnerRect = new Rect(m_SpinnerPosition.x - m_SpinnerSize.x * 0.5f, m_SpinnerPosition.y - m_SpinnerSize.y * 0.5f, m_SpinnerSize.x, m_SpinnerSize.y);
			m_SpinnerPivotPos = new Vector2(m_SpinnerRect.xMin + m_SpinnerRect.width * 0.5f, m_SpinnerRect.yMin + m_SpinnerRect.height * 0.5f);
			GUI.Box(m_SpinnerRect, string.Empty);
			Matrix4x4 matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(m_SpinnerAngle, m_SpinnerPivotPos);
			GUI.DrawTexture(m_SpinnerRect, m_Spinner);
			GUI.matrix = matrix;
			m_SpinnerAngle += 3f;
		}
	}

	private void DoWindow(int windowID)
	{
		Vector2 vector = new Vector2(windowRect.width - 2f * listMargin.x, windowRect.height - 2f * listMargin.y);
		scrollPosition = GUI.BeginScrollView(new Rect(listMargin.x, listMargin.y, vector.x, vector.y), viewRect: new Rect(0f, 0f, m_SpheroLabelWidth, m_RobotNames.Length * m_SpheroLabelHeight), scrollPosition: scrollPosition, alwaysShowHorizontal: false, alwaysShowVertical: false);
		if (m_MultipleSpheros)
		{
			for (int i = 0; i < m_RobotNames.Length; i++)
			{
				m_SpinnerPosition.x = m_SpinnerSize.x / 2f;
				m_SpinnerPosition.y = (float)(i * m_SpheroLabelHeight) + m_SpinnerSize.y / 2f;
				m_SpinnerRect = new Rect(m_SpinnerPosition.x - m_SpinnerSize.x * 0.5f, m_SpinnerPosition.y - m_SpinnerSize.y * 0.5f, m_SpinnerSize.x, m_SpinnerSize.y);
				m_SpinnerPivotPos = new Vector2(m_SpinnerRect.xMin + m_SpinnerRect.width * 0.5f, m_SpinnerRect.yMin + m_SpinnerRect.height * 0.5f);
				Sphero sphero = m_SpheroProvider.PairedSpheros[i];
				if (sphero.ConnectionState == Sphero.Connection_State.Connecting)
				{
					Matrix4x4 matrix = GUI.matrix;
					GUIUtility.RotateAroundPivot(m_SpinnerAngle, m_SpinnerPivotPos);
					GUI.DrawTexture(m_SpinnerRect, m_Spinner);
					GUI.matrix = matrix;
					m_SpinnerAngle += 3f;
				}
				else if (sphero.ConnectionState == Sphero.Connection_State.Connected)
				{
					GUI.DrawTexture(m_SpinnerRect, m_CheckMark);
				}
				else if (sphero.ConnectionState == Sphero.Connection_State.Failed)
				{
					GUI.DrawTexture(m_SpinnerRect, m_RedX);
				}
				else
				{
					GUI.DrawTexture(m_SpinnerRect, m_Spinner);
				}
				GUI.Label(new Rect(m_SpinnerSize.x, i * m_SpheroLabelHeight, m_SpheroLabelWidth, m_SpheroLabelHeight), m_RobotNames[i]);
			}
		}
		else
		{
			m_SpheroLabelSelected = GUI.SelectionGrid(new Rect(0f, 0f, m_SpheroLabelWidth, m_SpheroLabelHeight * m_RobotNames.Length), m_SpheroLabelSelected, m_RobotNames, 1, "toggle");
		}
		GUI.EndScrollView();
	}

	private int TouchToRowIndex(Vector2 touchPos)
	{
		float num = (float)Screen.height - touchPos.y;
		num += scrollPosition.y;
		num -= windowMargin.y;
		num -= listMargin.y;
		int a = (int)(num / (float)m_SpheroLabelHeight);
		return Mathf.Min(a, m_RobotNames.Length);
	}
}
