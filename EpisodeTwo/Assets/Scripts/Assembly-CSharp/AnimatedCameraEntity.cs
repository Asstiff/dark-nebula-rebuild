using UnityEngine;

public class AnimatedCameraEntity : AnimatedEntity, ICamera
{
	private bool isDone;

	private bool isActive;

	public ICamera _parent { get; set; }

	public override bool PostInit()
	{
		if (base.PostInit())
		{
			return true;
		}
		isActive = true;
		animationOneShot = true;
		if (curveAnimation != null)
		{
			curveAnimation.shouldLoop = false;
			base.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
			base.transform.position = base.transform.position;
		}
		else
		{
			Debug.LogWarning("AnimatedCameraEntity started without a curveAnimation.");
		}
		return false;
	}

	public void SetWorldPos(Vector2 p)
	{
		base.transform.localPosition = new Vector3(p.x, base.transform.localPosition.y, p.y);
	}

	public Vector2 WorldPos()
	{
		return new Vector2(base.transform.localPosition.x, base.transform.localPosition.z);
	}

	public override void OnActivateEvent()
	{
		base.OnActivateEvent();
		World.CameraManager.ActivateCameraPriority(this, 1);
	}

	public override void OnAnimationDone()
	{
		if (isActive)
		{
			base.OnAnimationDone();
			World.TriggerFlag(GetParameterInt("animationDoneTriggerFlag"));
			World.CameraManager.DeactivateCamera(this);
			isDone = true;
		}
	}

	public bool IsAnimationDone()
	{
		return isDone;
	}

	public void Remove()
	{
		if (isActive)
		{
			isActive = false;
			curveAnimation = null;
			ObjectPool.instance.PoolObject(this);
		}
	}

	public override void Reset()
	{
	}

	public override void Update()
	{
	}

	public void UpdateCamera()
	{
		base.Update();
	}

	public void UpdateCascading()
	{
	}
}
