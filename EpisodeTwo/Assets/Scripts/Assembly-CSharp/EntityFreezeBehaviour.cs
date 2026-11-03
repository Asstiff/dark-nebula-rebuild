using System.Collections.Generic;
using UnityEngine;

public class EntityFreezeBehaviour : MonoBehaviour
{
	public Entity Host;

	public int delayFrames;

	private static int globalId;

	private int interval;

	private void Awake()
	{
		interval = globalId++ % 3;
	}

	public void LateUpdate()
	{
		if (delayFrames > 0 || Time.frameCount % 3 != interval)
		{
			delayFrames--;
		}
		else if (!Host.IsPooled && World.IsRunning && Host.IsOutsideActiveView())
		{
			if (Host.freezable)
			{
				ForceFreeze();
			}
			else
			{
				Host.RemoveFromWorld();
			}
		}
	}

	public void ForceFreeze()
	{
		World.FreezeManager.AddToFrozenObjects(Host);
		base.enabled = false;
		Host.enabled = false;
		Host.IsFrozen = true;
		Host.DisabledRenderers = new List<Renderer>();
		if (Host.isInScollingLayer)
		{
		}
	}
}
