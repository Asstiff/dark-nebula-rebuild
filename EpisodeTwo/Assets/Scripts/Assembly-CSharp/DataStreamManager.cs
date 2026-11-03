using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataStreamManager
{
	public static DataInputStream internalLoadDataFromName(string path, ushort version)
	{
		DataInputStream dataInputStream = new DataInputStream();
		dataInputStream.Open(path);
		return internalLoadDataFromName(dataInputStream, version);
	}

	public static DataInputStream LoadResourceStream(string pResourcePath, ushort version)
	{
		TextAsset textAsset = Resources.Load(pResourcePath) as TextAsset;
		if (textAsset == null)
		{
			Debug.Log("Can't Resources.Load(" + pResourcePath + ")");
			return null;
		}
		Stream input = new MemoryStream(textAsset.bytes);
		DataInputStream dataInputStream = new DataInputStream();
		dataInputStream.Open(new BinaryReader(input));
		return internalLoadDataFromName(dataInputStream, version);
	}

	private static DataInputStream internalLoadDataFromName(DataInputStream stream, ushort version)
	{
		ushort num = stream.ReadUInt16();
		if (num != version)
		{
			Debug.LogError("Not same version  " + num + "-" + version);
		}
		return stream;
	}

	public static void readEntityParametersForObject(IEntity entity, DataInputStream stream)
	{
		byte b = stream.ReadByte();
		for (byte b2 = 0; b2 < b; b2++)
		{
			string key = stream.ReadString();
			string normalValue = stream.ReadString();
			string expertValue = stream.ReadString();
			if (entity != null)
			{
				entity.SetParameter(key, normalValue, expertValue);
			}
		}
		readArrayParameters(entity, stream);
	}

	public static Dictionary<string, string> readEntityParametersForObject(DataInputStream stream)
	{
		byte b = stream.ReadByte();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (byte b2 = 0; b2 < b; b2++)
		{
			dictionary.Add(stream.ReadString(), stream.ReadString());
			stream.ReadString();
		}
		readArrayParameters(stream);
		return dictionary;
	}

	private static void readArrayParameters(IEntity entity, DataInputStream stream)
	{
		byte b = stream.ReadByte();
		for (byte b2 = 0; b2 < b; b2++)
		{
			string name = stream.ReadString();
			readStructureArray(entity, name, stream);
		}
	}

	private static void readArrayParameters(DataInputStream stream)
	{
		byte b = stream.ReadByte();
		for (byte b2 = 0; b2 < b; b2++)
		{
			string name = stream.ReadString();
			readStructureArray(name, stream);
		}
	}

	private static void readStructureArray(IEntity entity, string name, DataInputStream stream)
	{
		byte b = stream.ReadByte();
		List<AnimatedEntityNode> list = new List<AnimatedEntityNode>();
		for (byte b2 = 0; b2 < b; b2++)
		{
			stream.ReadByte();
			AnimatedEntityNode animatedEntityNode = new AnimatedEntityNode();
			readEntityParametersForObject(animatedEntityNode, stream);
			list.Add(animatedEntityNode);
		}
		setArrayToObject(entity, name, list);
	}

	private static void readStructureArray(string name, DataInputStream stream)
	{
		byte b = stream.ReadByte();
		for (byte b2 = 0; b2 < b; b2++)
		{
			stream.ReadByte();
			readEntityParametersForObject(stream);
		}
	}

	private static void setArrayToObject(IEntity entity, string memberName, List<AnimatedEntityNode> array)
	{
		if (entity != null)
		{
			entity.SetParameterArray(memberName, array);
		}
	}

	public static string convertEnumStringsToValue(string s)
	{
		switch (s)
		{
		case "red":
			return "1";
		case "blue":
			return "2";
		case "green":
			return "3";
		case "box":
			return "4";
		case "circle":
			return "0";
		default:
			return s;
		}
	}
}
