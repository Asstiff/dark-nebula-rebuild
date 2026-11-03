using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	private const int MAX_CAMERA_LAYERS = 10;

	private ICamera[] cameraLayer;

	private CameraMixer cameraMixer;

	private CameraShaker cameraShaker;

	private List<ICamera> cameras;

	private new ICamera camera;

	private float cameraLimitLeft;

	private float cameraLimitRight;

	private float targetGameCameraLimitLeft;

	private float targetGameCameraLimitRight;

	public Vector2 cameraPosition;

	public float narrow = 0.05f;

	public void Awake()
	{
		cameras = new List<ICamera>();
		cameraLayer = new ICamera[10];
		for (int i = 0; i < 10; i++)
		{
			cameraLayer[i] = null;
		}
		cameraPosition = new Vector2(0f, World.mapHeight * -64);
		cameraMixer = new CameraMixer(cameraPosition);
		cameraShaker = new CameraShaker(Vector2.zero);
		cameraMixer.AddChild(cameraShaker);
	}

	public void UpdateCamera()
	{
		for (int i = 0; i < cameras.Count; i++)
		{
			cameras[i].UpdateCamera();
		}
		ICamera camera = FindHighestPriorityCamera();
		if (camera != null)
		{
			if (camera != this.camera)
			{
				cameraMixer.StartMixing(this.camera, camera);
				this.camera = camera;
			}
			if (this.camera != null)
			{
				this.camera.UpdateCamera();
			}
			cameraMixer.UpdateCascading();
			Vector2 vector = cameraShaker.WorldPos();
			FindCameraLimits();
			SmoothCameraLimit(vector.x);
			vector.x = Mathf.Max(vector.x, cameraLimitLeft);
			vector.x = Mathf.Min(vector.x, cameraLimitRight);
			cameraPosition.x = vector.x;
			cameraPosition.y = vector.y;
		}
	}

	public void ActivateCameraPriority(ICamera c, int priority)
	{
		cameraLayer[priority] = c;
	}

	public void DeactivateCamera(ICamera c)
	{
		for (int i = 0; i < 10; i++)
		{
			if (cameraLayer[i] == c)
			{
				cameraLayer[i] = null;
			}
		}
	}

	public ICamera FindHighestPriorityCamera()
	{
		for (int i = 0; i < 10; i++)
		{
			if (cameraLayer[i] != null)
			{
				return cameraLayer[i];
			}
		}
		return null;
	}

	private void SmoothCameraLimit(float x)
	{
		if (x > cameraLimitLeft)
		{
			cameraLimitLeft = Math.Min(x, targetGameCameraLimitLeft);
		}
		if (x < cameraLimitRight)
		{
			cameraLimitRight = Math.Max(x, targetGameCameraLimitRight);
		}
		float num = (targetGameCameraLimitLeft - cameraLimitLeft) * Time.deltaTime;
		if (Math.Abs(num) > 8f)
		{
			num = Mathf.Sign(num) * 8f;
		}
		cameraLimitLeft += num;
		float num2 = (targetGameCameraLimitRight - cameraLimitRight) * Time.deltaTime;
		if (Math.Abs(num2) > 8f)
		{
			num2 = Mathf.Sign(num2) * 8f;
		}
		cameraLimitRight += num2;
	}

	private void FindCameraLimits()
	{
		if (World.vessel == null)
		{
			return;
		}
		Ray ray = new Ray(World.vessel.transform.position + new Vector3(0f, 0f, 32f), Vector3.left);
		float num = 0f;
		float num2 = 0f;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 960f, 256))
		{
			num = hitInfo.point.x + narrow;
			targetGameCameraLimitLeft = num + 160f;
		}
		ray.direction = Vector3.right;
		if (Physics.Raycast(ray, out hitInfo, 960f, 256))
		{
			num2 = hitInfo.point.x - narrow;
			targetGameCameraLimitRight = num2 - 160f;
		}
		if (cameraLimitRight == 0f)
		{
			if (targetGameCameraLimitRight == 0f)
			{
				targetGameCameraLimitLeft = 0f;
				targetGameCameraLimitRight = 2048f;
			}
			cameraLimitLeft = targetGameCameraLimitLeft;
			cameraLimitRight = targetGameCameraLimitRight;
		}
	}

	public void SetCameraPosition(Vector2 p)
	{
		cameraPosition = p;
	}
}
