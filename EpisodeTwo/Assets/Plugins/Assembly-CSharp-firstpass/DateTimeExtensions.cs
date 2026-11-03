using System;
using FLD.JSON;

public static class DateTimeExtensions
{
	public static DateTime UnixEpoch
	{
		get
		{
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		}
	}

	public static DateTime FromJsonUnixTime(this JValue jsonUnixTime)
	{
		if (jsonUnixTime.IsNull)
		{
			return UnixEpoch;
		}
		return FromUnixTime((int)jsonUnixTime);
	}

	public static DateTime FromUnixTime(this long unixTime)
	{
		return UnixEpoch.AddSeconds(unixTime);
	}

	public static long ToUnixTime(this DateTime date)
	{
		return Convert.ToInt64((date - UnixEpoch).TotalSeconds);
	}
}
