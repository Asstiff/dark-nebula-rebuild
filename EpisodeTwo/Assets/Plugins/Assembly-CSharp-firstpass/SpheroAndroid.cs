using UnityEngine;

public class SpheroAndroid : Sphero
{
	private AndroidJavaObject m_AndroidJavaSphero;

	private AndroidJavaObject m_UnityBridge = new AndroidJavaClass("orbotix.unity.UnityBridge").CallStatic<AndroidJavaObject>("sharedBridge", new object[0]);

	private AndroidJavaClass m_RGBLEDOutput = new AndroidJavaClass("orbotix.robot.base.RGBLEDOutputCommand");

	private AndroidJavaClass m_RollCommand = new AndroidJavaClass("orbotix.robot.base.RollCommand");

	private AndroidJavaClass m_SetHeadingCommand = new AndroidJavaClass("orbotix.robot.base.SetHeadingCommand");

	private AndroidJavaClass m_BackLEDOutputCommand = new AndroidJavaClass("orbotix.robot.base.BackLEDOutputCommand");

	public AndroidJavaObject AndroidJavaSphero
	{
		get
		{
			return m_AndroidJavaSphero;
		}
	}

	public SpheroAndroid(AndroidJavaObject sphero, string bt_name, string bt_address)
	{
		m_AndroidJavaSphero = sphero;
		m_DeviceInfo = new BluetoothDeviceInfo(bt_name, bt_address);
	}

	public override void SetRGBLED(float red, float green, float blue)
	{
		m_RGBLEDOutput.CallStatic("sendCommand", m_AndroidJavaSphero, (int)(red * 255f), (int)(green * 255f), (int)(blue * 255f));
		m_RGBLEDColor = new Color(red, green, blue, 1f);
	}

	public override void EnableControllerStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask)
	{
		m_UnityBridge.Call("enableControllerStreaming", m_AndroidJavaSphero, (int)divisor, (int)packetFrames, (long)sensorMask);
	}

	public override void DisableControllerStreaming()
	{
		m_UnityBridge.Call("disableControllerStreaming", m_AndroidJavaSphero);
	}

	public override void SetDataStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask, ushort packetCount)
	{
		m_UnityBridge.Call("setDataStreaming", m_AndroidJavaSphero, divisor, packetFrames, sensorMask, packetCount);
	}

	public override void Roll(int heading, float speed)
	{
		m_RollCommand.CallStatic("sendCommand", m_AndroidJavaSphero, (float)heading, speed);
	}

	public override void SetHeading(int heading)
	{
		m_SetHeadingCommand.CallStatic("sendCommand", m_AndroidJavaSphero, (float)heading);
	}

	public override void SetBackLED(float intensity)
	{
		m_BackLEDOutputCommand.CallStatic("sendCommand", m_AndroidJavaSphero, intensity);
	}
}
