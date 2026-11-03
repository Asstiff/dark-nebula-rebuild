using System.Collections.Generic;
using UnityEngine;

public class SpheroProviderAndroid : SpheroProvider
{
	private AndroidJavaObject m_RobotProvider;

	public SpheroProviderAndroid()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Looper"))
		{
			androidJavaClass.CallStatic("prepare");
		}
		using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("orbotix.robot.base.RobotProvider"))
		{
			m_RobotProvider = androidJavaClass2.CallStatic<AndroidJavaObject>("getDefaultProvider", new object[0]);
		}
		SpheroDeviceMessenger.SharedInstance.NotificationReceived += ReceiveNotificationMessage;
	}

	private void ReceiveNotificationMessage(object sender, SpheroDeviceMessenger.MessengerEventArgs eventArgs)
	{
		SpheroDeviceNotification spheroDeviceNotification = (SpheroDeviceNotification)eventArgs.Message;
		Sphero sphero = GetSphero(spheroDeviceNotification.RobotID);
		if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTED)
		{
			sphero.ConnectionState = Sphero.Connection_State.Connected;
		}
		else if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.DISCONNECTED)
		{
			sphero.ConnectionState = Sphero.Connection_State.Disconnected;
		}
		else if (spheroDeviceNotification.NotificationType == SpheroDeviceNotification.SpheroNotificationType.CONNECTION_FAILED)
		{
			sphero.ConnectionState = Sphero.Connection_State.Failed;
		}
	}

	public override void DisconnectSpheros()
	{
		if (m_RobotProvider != null)
		{
			m_RobotProvider.Call("disconnectControlledRobots");
		}
		if (m_PairedSpheros != null)
		{
			Sphero[] pairedSpheros = m_PairedSpheros;
			foreach (Sphero sphero in pairedSpheros)
			{
				sphero.ConnectionState = Sphero.Connection_State.Disconnected;
			}
		}
	}

	public override void FindRobots()
	{
		if (IsAdapterEnabled())
		{
			m_RobotProvider.Call("findRobots");
			AndroidJavaObject androidJavaObject = m_RobotProvider.Call<AndroidJavaObject>("getRobots", new object[0]);
			int num = androidJavaObject.Call<int>("size", new object[0]);
			m_PairedSpheros = new Sphero[num];
			for (int i = 0; i < num; i++)
			{
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("get", new object[1] { i });
				string bt_name = androidJavaObject2.Call<string>("getName", new object[0]);
				string bt_address = androidJavaObject2.Call<string>("getUniqueId", new object[0]);
				m_PairedSpheros[i] = new SpheroAndroid(androidJavaObject2, bt_name, bt_address);
			}
		}
	}

	public override bool IsAdapterEnabled()
	{
		return m_RobotProvider.Call<bool>("isAdapterEnabled", new object[0]);
	}

	public override void Connect(int index)
	{
		if (GetConnectingSphero() == null)
		{
			m_RobotProvider.Call("control", ((SpheroAndroid)m_PairedSpheros[index]).AndroidJavaSphero);
			m_RobotProvider.Call<AndroidJavaObject>("connectControlledRobots", new object[0]);
			m_PairedSpheros[index].ConnectionState = Sphero.Connection_State.Connecting;
		}
	}

	public override Sphero GetSphero(string spheroId)
	{
		Sphero[] pairedSpheros = m_PairedSpheros;
		foreach (Sphero sphero in pairedSpheros)
		{
			if (sphero.DeviceInfo.UniqueId.Equals(spheroId))
			{
				return sphero;
			}
		}
		return null;
	}

	public override Sphero[] GetConnectedSpheros()
	{
		List<Sphero> list = new List<Sphero>();
		Sphero[] pairedSpheros = m_PairedSpheros;
		foreach (Sphero sphero in pairedSpheros)
		{
			if (sphero.ConnectionState == Sphero.Connection_State.Connected)
			{
				list.Add(sphero);
			}
		}
		Sphero[] array = new Sphero[list.Count];
		int num = 0;
		foreach (Sphero item in list)
		{
			array[num] = item;
			num++;
		}
		return array;
	}
}
