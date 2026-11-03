using System;
using UnityEngine;

public class SpheroAccelerometer : MonoBehaviour
{
	public static SpheroAccelerometer _instance;

	public Vector3 rawSphero = Vector3.zero;

	public Vector3 position = Vector3.zero;

	private bool ObservingDataStreaming;

	private bool CheckForSpheroConnection;

	private Sphero connectedSphero;

	private static SpheroProvider SpheroProvider;

	private int failedTries;

	private SpheroDeviceSensorsAsyncData message;

	public static SpheroAccelerometer Instance
	{
		get
		{
			if (_instance == null)
			{
				SpheroProvider = SpheroProvider.GetSharedProvider();
				GameObject gameObject = new GameObject("!SpheroAccelerometer");
				_instance = gameObject.AddComponent<SpheroAccelerometer>();
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return _instance;
		}
	}

	public static bool GetIsConnected()
	{
		return BuildSettings.Current.SpheroEnabled && Instance.connectedSphero != null && Instance.connectedSphero.ConnectionState == Sphero.Connection_State.Connected;
	}

	public void Disconnect()
	{
		if (BuildSettings.Current.SpheroEnabled && SpheroProvider != null)
		{
			SpheroProvider.DisconnectSpheros();
			CheckForSpheroConnection = false;
		}
	}

	private void Awake()
	{
		SpheroConnectScreen.SpheroConnected += OnSpheroConnected;
	}

	private void OnSpheroConnected()
	{
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
		ObservingDataStreaming = true;
		TryConnect();
	}

	private void FixedUpdate()
	{
		if (BuildSettings.Current.SpheroEnabled)
		{
			HandleEvents();
		}
	}

	private void HandleEvents()
	{
		if (message != null)
		{
			SpheroDeviceSensorsData spheroDeviceSensorsData = message.Frames[0];
			message = null;
			rawSphero = new Vector3(spheroDeviceSensorsData.AccelerometerData.Normalized.X, spheroDeviceSensorsData.AccelerometerData.Normalized.Y, 0f);
			rawSphero = rawSphero.normalized * Mathf.Min(1f, rawSphero.magnitude * 2f);
			position = Vector3.Lerp(position, rawSphero, Time.deltaTime * 15f);
		}
	}

	private void Update()
	{
		if (BuildSettings.Current.SpheroEnabled)
		{
			if (!ObservingDataStreaming && SpheroConnectScreen.HasConnectedRobot)
			{
				SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
				ObservingDataStreaming = true;
				failedTries = 0;
				Debug.Log(base.name + " ObservingDataStreaming");
			}
			if (CheckForSpheroConnection && Time.frameCount % 30 == 0 && failedTries < 10)
			{
				TryConnect();
				failedTries++;
			}
			HandleEvents();
		}
	}

	protected void TryConnect()
	{
		if (!SpheroProvider.IsAdapterEnabled())
		{
			return;
		}
		Sphero[] array = null;
		try
		{
			array = SpheroProvider.GetConnectedSpheros();
		}
		catch (Exception ex)
		{
			Log("Failed trying to SpheroProvider.GetConnectedSpheros() " + ex.Message);
		}
		if (array != null && array.Length > 0)
		{
			Log(" Enable streaming sphero");
			connectedSphero = array[0];
			if (connectedSphero != null)
			{
				connectedSphero.EnableControllerStreaming(20, 1, SpheroDataStreamingMask.QuaternionAll | SpheroDataStreamingMask.IMUAnglesFilteredAll | SpheroDataStreamingMask.AccelerometerFilteredAll);
				connectedSphero.SetBackLED(1f);
				connectedSphero.SetRGBLED(0.5f, 0.5f, 0.5f);
				SpheroDeviceMessenger.SharedInstance.AsyncDataReceived += ReceiveAsyncMessage;
				CheckForSpheroConnection = false;
			}
			else
			{
				Log(" Enable streaming sphero Failed");
			}
		}
	}

	private void Log(string msg)
	{
	}

	private void OnApplicationPause(bool pause)
	{
		if (DarkNebulaAds.IsAboutToLeaveAppAndComeBack)
		{
			return;
		}
		if (pause)
		{
			if (ObservingDataStreaming)
			{
				SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
				Debug.Log(" NOT ObservingDataStreaming");
			}
			if (connectedSphero != null)
			{
				connectedSphero.DisableControllerStreaming();
				connectedSphero.SetRGBLED(0f, 0f, 0f);
				SpheroDeviceMessenger.SharedInstance.AsyncDataReceived -= ReceiveAsyncMessage;
			}
			Instance.Disconnect();
		}
		else
		{
			if (BuildSettings.Current.SpheroEnabled)
			{
				SpheroProvider.FindRobots();
			}
			ObservingDataStreaming = false;
			CheckForSpheroConnection = true;
			position = Vector3.zero;
			failedTries = 0;
		}
	}

	private void OnApplicationQuit()
	{
		OnApplicationPause(true);
	}

	private void ReceiveAsyncMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		message = (SpheroDeviceSensorsAsyncData)eventArgs.Message;
	}

	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification spheroDeviceNotification = (SpheroDeviceNotification)eventArgs.Message;
		if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED)
		{
			Log(" Connection Success");
			CheckForSpheroConnection = true;
			failedTries = 0;
			TryConnect();
		}
		else if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.DISCONNECTED)
		{
			Log(" Sphero DISCONNECTED");
			failedTries = 0;
			Sphero sphero = SpheroProvider.GetSphero(spheroDeviceNotification.RobotID);
			if (sphero != null)
			{
				sphero.ConnectionState = Sphero.Connection_State.Disconnected;
			}
			if (connectedSphero == sphero)
			{
				connectedSphero = null;
			}
		}
	}
}
