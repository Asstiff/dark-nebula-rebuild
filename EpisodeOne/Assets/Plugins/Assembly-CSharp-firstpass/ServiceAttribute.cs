using System;
using FLD;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
public sealed class ServiceAttribute : Attribute
{
	private readonly string _name;

	private string[] _platforms;

	public string Name
	{
		get
		{
			return _name;
		}
	}

	public string Description { get; set; }

	public string AvailablePlatforms
	{
		get
		{
			return string.Join(",", _platforms);
		}
		set
		{
			_platforms = value.Split(',');
		}
	}

	public ServiceAttribute(string name)
	{
		_name = name;
	}

	public bool IsAvailableOnPlatform(string pPlatform)
	{
		return _platforms == null || _platforms.Contains(pPlatform);
	}
}
