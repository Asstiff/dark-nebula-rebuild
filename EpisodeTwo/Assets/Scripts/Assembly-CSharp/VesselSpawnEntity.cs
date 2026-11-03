using System;
using System.Collections.Generic;
using UnityEngine;

public class VesselSpawnEntity : Entity
{
	private float scrollInLength;

	private float scrollInTime;

	protected VesselSkinType skinType;

	protected bool spawned;

	protected bool reportedIntroductionDone;

	protected AnimatedCameraEntity animatedCamera;

	public virtual void Init(Vector3 position, int doneFlag)
	{
		throw new InvalidOperationException("Cannot call this function directly it must be overrided.");
	}

	public virtual void Init(Vector3 position, bool respawn, int flag, int scrollInLength, int scrollTime)
	{
		throw new InvalidOperationException("Cannot call this function directly it must be overrided.");
	}

	public virtual void Init(Vector3 p, int length, int scrollTime)
	{
		base.alwaysAlive = true;
		base.transform.position = p;
		scrollInLength = length;
		scrollInTime = scrollTime;
		if (scrollInLength > 0f)
		{
			AddScrollInCamera(new Vector2(p.x, p.z));
			return;
		}
		Vector2 vector = default(Vector2);
		vector.x = (int)p.x;
		vector.y = (int)p.z;
		World.CameraManager.SetCameraPosition(p);
	}

	public void SetSkinType(VesselSkinType skin)
	{
		skinType = skin;
	}

	public virtual void Update()
	{
		if (!reportedIntroductionDone && AnimationIsDone())
		{
			OnIntroductionDone();
			reportedIntroductionDone = true;
		}
	}

	private bool AnimationIsDone()
	{
		return animatedCamera == null || animatedCamera.IsAnimationDone();
	}

	private void AddScrollInCamera(Vector2 p)
	{
		List<AnimatedEntityNode> list = new List<AnimatedEntityNode>();
		AnimatedEntityNode animatedEntityNode = new AnimatedEntityNode();
		animatedEntityNode.InitWith(p.x, 0f - p.y + scrollInLength, scrollInTime, 0f, 0f, 0f);
		list.Add(animatedEntityNode);
		AnimatedEntityNode animatedEntityNode2 = new AnimatedEntityNode();
		animatedEntityNode2.InitWith(p.x, 0f - p.y, 0f, 0.5f, 0f, 0f);
		list.Add(animatedEntityNode2);
		animatedCamera = ObjectPool.instance.GetObjectForType<AnimatedCameraEntity>();
		animatedCamera.nodes = list;
		animatedCamera.SetWorldPos(p - new Vector2(0f, scrollInLength));
		animatedCamera.PostInit();
		animatedCamera.OnActivateEvent();
	}

	public virtual void OnIntroductionDone()
	{
	}
}
