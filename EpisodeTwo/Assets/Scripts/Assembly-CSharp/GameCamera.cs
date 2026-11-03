using System.Collections.Generic;
using UnityEngine;

public class GameCamera : ICamera
{
	public const int CAMERA_PRIORITY_CINEMATIC = 1;

	public const int CAMERA_PRIORITY_ACTION = 2;

	protected Vector2 localPos;

	private List<ICamera> _children;

	public ICamera _parent { get; set; }

	public ICamera Parent
	{
		get
		{
			return _parent;
		}
	}

	public GameCamera(Vector2 p)
	{
		_children = new List<ICamera>();
		localPos = p;
	}

	public virtual void Reset()
	{
	}

	public virtual void Remove()
	{
	}

	public virtual void UpdateCamera()
	{
	}

	public void UpdateCascading()
	{
		UpdateCamera();
		for (int i = 0; i < _children.Count; i++)
		{
			_children[i].UpdateCascading();
		}
	}

	public Vector2 WorldPos()
	{
		if (Parent != null)
		{
			return Parent.WorldPos() + localPos;
		}
		return localPos;
	}

	public void SetWorldPos(Vector2 newPosition)
	{
		if (newPosition.y == 0f)
		{
			Debug.Log("Debug this");
		}
		if (_parent != null)
		{
			localPos = newPosition - _parent.WorldPos();
		}
		else
		{
			localPos = newPosition;
		}
	}

	public void AddChild(ICamera pChild)
	{
		pChild._parent = this;
		_children.Add(pChild);
	}
}
