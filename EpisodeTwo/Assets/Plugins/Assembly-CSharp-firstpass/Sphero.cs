using UnityEngine;

public abstract class Sphero
{
	public class BluetoothDeviceInfo
	{
		private string m_Name;

		private string m_UniqueId;

		public string Name
		{
			get
			{
				return m_Name;
			}
			set
			{
				m_Name = value;
			}
		}

		public string UniqueId
		{
			get
			{
				return m_UniqueId;
			}
			set
			{
				m_UniqueId = value;
			}
		}

		public BluetoothDeviceInfo(string name, string uniqueId)
		{
			m_Name = name;
			m_UniqueId = uniqueId;
		}
	}

	public enum Connection_State
	{
		Failed = 0,
		Disconnected = 1,
		Connecting = 2,
		Connected = 3
	}

	protected BluetoothDeviceInfo m_DeviceInfo;

	protected Color m_RGBLEDColor;

	protected Connection_State m_ConnectionState = Connection_State.Disconnected;

	public BluetoothDeviceInfo DeviceInfo
	{
		get
		{
			return m_DeviceInfo;
		}
	}

	public Color RGBLEDColor
	{
		get
		{
			return m_RGBLEDColor;
		}
	}

	public Connection_State ConnectionState
	{
		get
		{
			return m_ConnectionState;
		}
		set
		{
			m_ConnectionState = value;
		}
	}

	public Sphero()
	{
	}

	public abstract void SetRGBLED(float red, float green, float blue);

	public abstract void SetDataStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask, ushort packetCount);

	public abstract void EnableControllerStreaming(ushort divisor, ushort packetFrames, SpheroDataStreamingMask sensorMask);

	public abstract void DisableControllerStreaming();

	public abstract void Roll(int heading, float speed);

	public abstract void SetHeading(int heading);

	public abstract void SetBackLED(float intensity);
}
