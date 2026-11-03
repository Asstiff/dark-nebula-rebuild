using System;
using System.Runtime.InteropServices;

public class SpheroDeviceMessenger
{
	public class MessengerEventArgs : EventArgs
	{
		private SpheroDeviceMessage message;

		public SpheroDeviceMessage Message
		{
			get
			{
				return message;
			}
		}

		public MessengerEventArgs(SpheroDeviceMessage message)
		{
			this.message = message;
		}
	}

	public delegate void MessengerEventHandler(object sender, MessengerEventArgs eventArgs);

	private delegate void ReceiveDeviceMessageDelegate(string encodedMessage);

	private static SpheroDeviceMessenger sharedInstance;

	public static SpheroDeviceMessenger SharedInstance
	{
		get
		{
			return sharedInstance;
		}
	}

	public event MessengerEventHandler ResponseDataReceived;

	public event MessengerEventHandler AsyncDataReceived;

	public event MessengerEventHandler NotificationReceived;

	private SpheroDeviceMessenger()
	{
	}

	static SpheroDeviceMessenger()
	{
		sharedInstance = new SpheroDeviceMessenger();
		if (BuildSettings.Current.SpheroEnabled)
		{
			_RegisterRecieveDeviceMessageCallback(ReceiveDeviceMessage);
		}
	}

	protected virtual void OnResponseMessageReceived(MessengerEventArgs eventArgs)
	{
		MessengerEventHandler responseDataReceived = ResponseDataReceived;
		if (responseDataReceived != null)
		{
			responseDataReceived(this, eventArgs);
		}
	}

	protected virtual void OnAsyncMessageReceived(MessengerEventArgs eventArgs)
	{
		MessengerEventHandler asyncDataReceived = AsyncDataReceived;
		if (asyncDataReceived != null)
		{
			asyncDataReceived(this, eventArgs);
		}
	}

	protected virtual void OnNotificationMessageReceived(MessengerEventArgs eventArgs)
	{
		MessengerEventHandler notificationReceived = NotificationReceived;
		if (notificationReceived != null)
		{
			notificationReceived(this, eventArgs);
		}
	}

	[MonoPInvokeCallback(typeof(ReceiveDeviceMessageDelegate))]
	protected static void ReceiveDeviceMessage(string encodedMessage)
	{
		SpheroDeviceMessage spheroDeviceMessage = SpheroDeviceMessageDecoder.messageFromEncodedString(encodedMessage);
		if (spheroDeviceMessage is SpheroDeviceAsyncMessage)
		{
			sharedInstance.OnAsyncMessageReceived(new MessengerEventArgs(spheroDeviceMessage));
		}
		else if (spheroDeviceMessage is SpheroDeviceNotification)
		{
			sharedInstance.OnNotificationMessageReceived(new MessengerEventArgs(spheroDeviceMessage));
		}
	}

	[DllImport("unity_bridge")]
	private static extern void _RegisterRecieveDeviceMessageCallback(ReceiveDeviceMessageDelegate callback);
}
