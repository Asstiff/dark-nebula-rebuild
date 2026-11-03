public class SpheroDeviceSensorsAsyncData : SpheroDeviceAsyncMessage
{
	private int frameCount;

	private ulong mask;

	private SpheroDeviceSensorsData[] frames;

	public int FrameCount
	{
		get
		{
			return frameCount;
		}
	}

	public ulong Mask
	{
		get
		{
			return mask;
		}
	}

	public SpheroDeviceSensorsData[] Frames
	{
		get
		{
			return frames;
		}
	}

	public SpheroDeviceSensorsAsyncData(SpheroDeviceMessageDecoder decoder)
		: base(decoder)
	{
		frameCount = decoder.DecodeInt32("frameCount");
		mask = decoder.DecodeUInt64("mask");
		object[] array = (object[])decoder.DecodeObject("dataFrames");
		frames = new SpheroDeviceSensorsData[array.Length];
		int num = 0;
		object[] array2 = array;
		foreach (object obj in array2)
		{
			frames[num++] = (SpheroDeviceSensorsData)obj;
		}
	}
}
