using UnityEngine;

public abstract class SpheroProvider
{
	private static SpheroProvider sharedProvider;

	protected Sphero[] m_PairedSpheros;

	public Sphero[] PairedSpheros
	{
		get
		{
			return m_PairedSpheros;
		}
		set
		{
			m_PairedSpheros = value;
		}
	}

	public SpheroProvider()
	{
	}

	public static SpheroProvider GetSharedProvider()
	{
		if (sharedProvider == null)
		{
			sharedProvider = new SpheroProviderAndroid();
		}
		return sharedProvider;
	}

	public Sphero GetConnectingSphero()
	{
		Sphero[] pairedSpheros = m_PairedSpheros;
		foreach (Sphero sphero in pairedSpheros)
		{
			if (sphero.ConnectionState == Sphero.Connection_State.Connecting)
			{
				return sphero;
			}
		}
		return null;
	}

	public string[] GetRobotNames()
	{
		Debug.Log(m_PairedSpheros);
		string[] array = new string[m_PairedSpheros.Length];
		for (int i = 0; i < m_PairedSpheros.Length; i++)
		{
			array[i] = m_PairedSpheros[i].DeviceInfo.Name;
		}
		return array;
	}

	public abstract Sphero[] GetConnectedSpheros();

	public abstract void DisconnectSpheros();

	public abstract void FindRobots();

	public abstract bool IsAdapterEnabled();

	public abstract void Connect(int index);

	public abstract Sphero GetSphero(string spheroId);
}
