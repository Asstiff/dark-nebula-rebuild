using System;
using System.Collections.Generic;
using UnityEngine;

public class SpheroConnectScreen : MonoBehaviour
{
	public SimpleButton ButtonGoBack;

	public SimpleButton ButtonSphero;

	public SimpleText Title;

	public SimpleText SubTitle;

	public SimpleText Instructions;

	public List<SimpleButton> SpheroButtons;

	public Transform SpheroImage;

	private SpheroProvider spheroProvider;

	private string ConnectingRobotName;

	public static string[] RobotNames;

	public static bool HasConnectedRobot;

	private bool isListeningToNotifications;

	private SpheroDeviceNotification message;

	private bool IsTryingToConnect;

	public static bool HasPairedRobots
	{
		get
		{
			return RobotNames != null && RobotNames.Length > 0;
		}
	}

	public event Action EventGoBack;

	public static event Action SpheroConnected;

	private void Start()
	{
		if (!BuildSettings.Current.SpheroEnabled) return;
		spheroProvider = SpheroProvider.GetSharedProvider();
		ButtonGoBack.ButtonUpEvent += delegate
		{
			OnGoBack();
		};
		Instructions.SetStringID("Sphero Instructions");
	}

	private void OnEnable()
	{
		ButtonSphero.gameObject.SetActive(false);
	}

	private void ClearSpheroButtons()
	{
		if (SpheroButtons != null)
		{
			foreach (SimpleButton spheroButton in SpheroButtons)
			{
				UnityEngine.Object.DestroyImmediate(spheroButton.gameObject);
			}
		}
		SpheroButtons = new List<SimpleButton>();
	}

	public void Search(bool AutoConnect = true)
	{
		ShowInstructions(false);
		ClearSpheroButtons();
		SetupAndroid(AutoConnect);
	}

	private void SetupAndroid(bool AutoConnect)
	{
		if (!spheroProvider.IsAdapterEnabled())
		{
			SetFeedback("Bluetooth Not Enabled");
			return;
		}
		spheroProvider.FindRobots();
		RobotNames = spheroProvider.GetRobotNames();
		if (RobotNames == null)
		{
			RobotNames = new string[0];
		}
		if (RobotNames.Length == 1 && AutoConnect)
		{
			ConnectSphero(0);
			return;
		}
		if (RobotNames.Length == 0)
		{
			SetFeedback("Need to pair a Sphero");
			return;
		}
		SetFeedback("TAP A SPHERO TO CONNECT");
		CreateSpheroButtons(RobotNames);
	}

	private void SetupIOS()
	{
		if (!spheroProvider.IsAdapterEnabled())
		{
			SetFeedback("Bluetooth Not Enabled");
			return;
		}
		ConnectSphero(0);
		Invoke("CheckForSpheroConnection", 1.5f);
	}

	private void CheckForSpheroConnection()
	{
		if (spheroProvider.GetConnectedSpheros().Length == 0)
		{
			Debug.Log("No spheroes connected");
			SetFeedback("Need to pair a Sphero");
		}
		else
		{
			ShowInstructions(true);
			ClearSpheroButtons();
		}
	}

	private void OnDestroy()
	{
		ListenToNotifications(false);
	}

	private void ListenToNotifications(bool pWantToListen)
	{
		if (pWantToListen != isListeningToNotifications || pWantToListen)
		{
			if (pWantToListen)
			{
				SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
				isListeningToNotifications = true;
			}
			else
			{
				SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
				isListeningToNotifications = false;
			}
		}
		else
		{
			Debug.Log("Did not need to change ListenToNotifications(" + isListeningToNotifications + ") already was ");
		}
	}

	private void ShowInstructions(bool show)
	{
		Instructions.gameObject.SetActive(show);
		if (SpheroImage != null)
		{
			SpheroImage.gameObject.SetActive(show);
		}
	}

	private void HandleEvents()
	{
		if (message == null)
		{
			return;
		}
		Debug.Log("Handle message " + message.NotificationType);
		switch (message.NotificationType)
		{
		case SpheroDeviceNotification.SpheroNotificationType.CONNECTED:
			SetFeedback("Connection Success");
			ListenToNotifications(false);
			ShowInstructions(true);
			DarkNebulaMetrics.SpheroConnected();
			ClearSpheroButtons();
			HasConnectedRobot = true;
			if (SpheroConnected != null)
			{
				SpheroConnected();
			}
			break;
		case SpheroDeviceNotification.SpheroNotificationType.CONNECTION_FAILED:
		{
			Sphero sphero = spheroProvider.GetSphero(message.RobotID);
			if (sphero == null)
			{
				break;
			}
			sphero.ConnectionState = Sphero.Connection_State.Disconnected;
			if (ConnectingRobotName == null || ConnectingRobotName == sphero.DeviceInfo.Name)
			{
				SetFeedback("Connection Failed");
				ListenToNotifications(false);
				ConnectingRobotName = null;
				HasConnectedRobot = false;
				if (IsTryingToConnect)
				{
					IsTryingToConnect = false;
					Search(false);
				}
			}
			break;
		}
		}
		message = null;
	}

	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		message = (SpheroDeviceNotification)eventArgs.Message;
	}

	private void Log(string msg)
	{
	}

	private void CreateSpheroButtons(string[] pRobotNames)
	{
		int num = Math.Min(5, pRobotNames.Length);
		Debug.Log("Spheros: " + string.Join(",", pRobotNames));
		for (int i = 0; i < num; i++)
		{
			int spheroId = i;
			SimpleButton simpleButton = UnityEngine.Object.Instantiate(ButtonSphero) as SimpleButton;
			simpleButton.transform.parent = base.transform;
			simpleButton.transform.position = ButtonSphero.transform.position;
			simpleButton.transform.Translate(0f, i * -50, 0f);
			simpleButton.ButtonPressedEvent += (SimpleButton x) =>
			{
				ConnectSphero(spheroId);
			};
			string text = pRobotNames[i];
			if (text == null)
			{
				text = "Unkown";
			}
			simpleButton.gameObject.SetActive(true);
			SimpleText componentInChildren = simpleButton.gameObject.GetComponentInChildren<SimpleText>();
			if (componentInChildren != null)
			{
				componentInChildren.SetText(text.ToUpper());
			}
			SpheroButtons.Add(simpleButton);
		}
	}

	private void SetFeedbackText(string text)
	{
		Debug.Log(text);
		SubTitle.SetText(text.ToUpper());
	}

	private void SetFeedback(string text)
	{
		Debug.Log(Language.Get(text));
		SubTitle.SetText(Language.Get(text).ToUpper());
	}

	private void ConnectSphero(int row)
	{
		ClearSpheroButtons();
		try
		{
			if (spheroProvider.GetConnectingSphero() != null || spheroProvider.PairedSpheros[row].ConnectionState == Sphero.Connection_State.Connected)
			{
				SetFeedback("Already connected");
				return;
			}
			ConnectingRobotName = RobotNames[row];
			SetFeedbackText(Language.Get("CONNECTING to ") + ConnectingRobotName);
			ListenToNotifications(true);
			IsTryingToConnect = true;
			spheroProvider.Connect(row);
		}
		catch (Exception)
		{
			SetFeedback("Failed to connect");
		}
	}

	public void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			OnGoBack();
		}
		HandleEvents();
	}

	private void OnGoBack()
	{
		ClearSpheroButtons();
		if (EventGoBack != null)
		{
			EventGoBack();
		}
	}
}
