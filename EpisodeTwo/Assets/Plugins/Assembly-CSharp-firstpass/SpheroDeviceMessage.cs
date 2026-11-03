using System;

public abstract class SpheroDeviceMessage
{
	private long timeStamp;

	private string robotId;

	public long TimeStamp
	{
		get
		{
			return timeStamp;
		}
	}

	public string RobotID
	{
		get
		{
			return robotId;
		}
	}

	public SpheroDeviceMessage()
	{
		timeStamp = DateTime.Now.Ticks;
	}

	public SpheroDeviceMessage(SpheroDeviceMessageDecoder decoder)
	{
		timeStamp = decoder.DecodeInt64("timeStamp");
		robotId = decoder.DecodeString("robotId");
	}
}
