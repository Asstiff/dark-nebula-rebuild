using UnityEngine;

public class UpdateValues : MonoBehaviour
{
	private Sphero m_Sphero;

	private bool streaming;

	private SpheroAccelerometerData.Acceleration acceleration = new SpheroAccelerometerData.Acceleration();

	private float pitch;

	private float roll;

	private float yaw;

	private float q0 = 1f;

	private float q1 = 1f;

	private float q2 = 1f;

	private float q3 = 1f;

	public GUIStyle boxStyle;

	public GUIStyle labelStyle;

	private void ViewSetup()
	{
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
		if (SpheroProvider.GetSharedProvider().GetConnectedSpheros().Length == 0)
		{
			Application.LoadLevel("SpheroConnectionScene");
		}
	}

	private void Start()
	{
		ViewSetup();
	}

	private void Update()
	{
		if (!streaming && SpheroProvider.GetSharedProvider().GetConnectedSpheros().Length > 0)
		{
			SpheroDeviceMessenger.SharedInstance.AsyncDataReceived += ReceiveAsyncMessage;
			Sphero[] connectedSpheros = SpheroProvider.GetSharedProvider().GetConnectedSpheros();
			m_Sphero = connectedSpheros[0];
			m_Sphero.EnableControllerStreaming(20, 1, SpheroDataStreamingMask.QuaternionAll | SpheroDataStreamingMask.IMUAnglesFilteredAll | SpheroDataStreamingMask.AccelerometerFilteredAll);
			streaming = true;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			if (streaming)
			{
				SpheroDeviceMessenger.SharedInstance.AsyncDataReceived -= ReceiveAsyncMessage;
				m_Sphero.DisableControllerStreaming();
				streaming = false;
			}
			SpheroDeviceMessenger.SharedInstance.NotificationReceived -= ReceiveNotificationMessage;
			SpheroProvider.GetSharedProvider().DisconnectSpheros();
		}
		else
		{
			ViewSetup();
			streaming = false;
		}
	}

	private void OnGUI()
	{
		GUI.BeginGroup(new Rect(10f, 10f, 250f, 400f));
		GUI.Box(new Rect(0f, 0f, 250f, 400f), string.Empty, boxStyle);
		GUI.Label(new Rect(4f, 30f, 400f, 36f), "accel x: " + acceleration.X, labelStyle);
		GUI.Label(new Rect(4f, 66f, 400f, 36f), "accel y: " + acceleration.Y, labelStyle);
		GUI.Label(new Rect(4f, 102f, 400f, 36f), "accel z: " + acceleration.Z, labelStyle);
		GUI.Label(new Rect(4f, 138f, 242f, 36f), "pitch: " + pitch, labelStyle);
		GUI.Label(new Rect(4f, 174f, 242f, 36f), "roll: " + roll, labelStyle);
		GUI.Label(new Rect(4f, 210f, 242f, 36f), "yaw: " + yaw, labelStyle);
		GUI.Label(new Rect(4f, 246f, 242f, 36f), "q0: " + q0, labelStyle);
		GUI.Label(new Rect(4f, 282f, 242f, 36f), "q1: " + q1, labelStyle);
		GUI.Label(new Rect(4f, 318f, 242f, 36f), "q2: " + q2, labelStyle);
		GUI.Label(new Rect(4f, 354f, 242f, 36f), "q3: " + q3, labelStyle);
		GUI.EndGroup();
	}

	private void ReceiveAsyncMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceSensorsAsyncData spheroDeviceSensorsAsyncData = (SpheroDeviceSensorsAsyncData)eventArgs.Message;
		SpheroDeviceSensorsData spheroDeviceSensorsData = spheroDeviceSensorsAsyncData.Frames[0];
		acceleration = spheroDeviceSensorsData.AccelerometerData.Normalized;
		pitch = spheroDeviceSensorsData.AttitudeData.Pitch;
		roll = spheroDeviceSensorsData.AttitudeData.Roll;
		yaw = spheroDeviceSensorsData.AttitudeData.Yaw;
		q0 = spheroDeviceSensorsData.QuaternionData.Q0;
		q1 = spheroDeviceSensorsData.QuaternionData.Q1;
		q2 = spheroDeviceSensorsData.QuaternionData.Q2;
		q3 = spheroDeviceSensorsData.QuaternionData.Q3;
	}

	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification spheroDeviceNotification = (SpheroDeviceNotification)eventArgs.Message;
		Sphero sphero = SpheroProvider.GetSharedProvider().GetSphero(spheroDeviceNotification.RobotID);
		if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.DISCONNECTED)
		{
			sphero.ConnectionState = Sphero.Connection_State.Disconnected;
			streaming = false;
			Application.LoadLevel("NoSpheroConnectedScene");
		}
	}
}
