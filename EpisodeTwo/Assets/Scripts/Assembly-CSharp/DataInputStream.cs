using System;
using System.IO;
using System.Text;
using UnityEngine;

public class DataInputStream
{
	public BinaryReader _stream;

	private byte[] a16 = new byte[2];

	private byte[] a32 = new byte[4];

	public bool HasReachedEnd
	{
		get
		{
			return _stream.BaseStream.Position >= _stream.BaseStream.Length - 1;
		}
	}

	public void Open(string filename)
	{
		Open(new BinaryReader(File.OpenRead(filename)));
	}

	public void Open(BinaryReader br)
	{
		_stream = br;
	}

	public void Close()
	{
		_stream.Close();
	}

	public int ReadInt()
	{
		a32 = _stream.ReadBytes(4);
		Array.Reverse(a32);
		return BitConverter.ToInt32(a32, 0);
	}

	public float ReadFloat()
	{
		a32 = _stream.ReadBytes(4);
		Array.Reverse(a32);
		return BitConverter.ToSingle(a32, 0);
	}

	public Vector2 ReadPosition()
	{
		return new Vector2(ReadFloat(), ReadFloat());
	}

	public byte ReadByte()
	{
		return _stream.ReadByte();
	}

	public ushort ReadUInt16()
	{
		a16 = _stream.ReadBytes(2);
		return (ushort)((a16[0] << 8) + a16[1]);
	}

	public string ReadString()
	{
		ushort count = ReadUInt16();
		return Encoding.UTF8.GetString(_stream.ReadBytes(count));
	}
}
