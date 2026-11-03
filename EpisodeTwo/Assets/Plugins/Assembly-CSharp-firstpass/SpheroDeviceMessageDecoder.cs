using System;
using System.Collections.Generic;
using JsonFx.Json;

public class SpheroDeviceMessageDecoder
{
	private Dictionary<string, object> dictionaryRepresentation;

	public SpheroDeviceMessageDecoder(string encodedMessage)
	{
		JsonReader jsonReader = new JsonReader(encodedMessage);
		dictionaryRepresentation = jsonReader.Deserialize<Dictionary<string, object>>();
	}

	private SpheroDeviceMessageDecoder(Dictionary<string, object> encodedDictionary)
	{
		dictionaryRepresentation = encodedDictionary;
	}

	public static SpheroDeviceMessage messageFromEncodedString(string encodedMessage)
	{
		SpheroDeviceMessageDecoder spheroDeviceMessageDecoder = new SpheroDeviceMessageDecoder(encodedMessage);
		return (SpheroDeviceMessage)spheroDeviceMessageDecoder.CreateObject();
	}

	private object CreateObject()
	{
		string typeName = "Sphero" + DecodeString("class");
		Type type = Type.GetType(typeName);
		object[] args = new object[1] { this };
		return Activator.CreateInstance(type, args);
	}

	public object DecodeObject(string key)
	{
		object value = null;
		dictionaryRepresentation.TryGetValue(key, out value);
		if (value is Array)
		{
			Dictionary<string, object>[] array = (Dictionary<string, object>[])value;
			object[] array2 = new object[array.Length];
			int num = 0;
			Dictionary<string, object>[] array3 = array;
			foreach (Dictionary<string, object> encodedDictionary in array3)
			{
				SpheroDeviceMessageDecoder spheroDeviceMessageDecoder = new SpheroDeviceMessageDecoder(encodedDictionary);
				object obj = spheroDeviceMessageDecoder.CreateObject();
				array2[num++] = obj;
			}
			value = array2;
		}
		else if (value is Dictionary<string, object>)
		{
			SpheroDeviceMessageDecoder spheroDeviceMessageDecoder2 = new SpheroDeviceMessageDecoder((Dictionary<string, object>)value);
			value = spheroDeviceMessageDecoder2.CreateObject();
		}
		return value;
	}

	public string DecodeString(string key)
	{
		return (string)DecodeObject(key);
	}

	public int DecodeInt32(string key)
	{
		return Convert.ToInt32(DecodeObject(key));
	}

	public long DecodeInt64(string key)
	{
		return Convert.ToInt64(DecodeObject(key));
	}

	public ushort DecodeUInt16(string key)
	{
		string value = DecodeString(key);
		return Convert.ToUInt16(value, 16);
	}

	public ulong DecodeUInt64(string key)
	{
		string value = DecodeString(key);
		return Convert.ToUInt64(value, 16);
	}

	public float DecodeFloat(string key)
	{
		return Convert.ToSingle(DecodeObject(key));
	}
}
