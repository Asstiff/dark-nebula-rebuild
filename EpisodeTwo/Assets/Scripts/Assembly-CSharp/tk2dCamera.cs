using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/Camera/tk2dCamera")]
public class tk2dCamera : MonoBehaviour
{
	public tk2dCameraResolutionOverride[] resolutionOverride;

	private tk2dCameraResolutionOverride currentResolutionOverride;

	[HideInInspector]
	public Camera mainCamera;

	public static tk2dCamera inst;

	[NonSerialized]
	public float orthoSize = 1f;

	[NonSerialized]
	public Vector2 resolution = new Vector2(1f, 1f);

	[HideInInspector]
	public bool forceResolutionInEditor;

	[HideInInspector]
	public Vector2 forceResolution = new Vector2(960f, 640f);

	private void Awake()
	{
		mainCamera = GetComponent<Camera>();
		if (mainCamera != null)
		{
			UpdateCameraMatrix();
		}
		inst = this;
	}

	private void LateUpdate()
	{
		UpdateCameraMatrix();
	}

	public void UpdateCameraMatrix()
	{
		inst = this;
		float pixelWidth = mainCamera.pixelWidth;
		float pixelHeight = mainCamera.pixelHeight;
		if (currentResolutionOverride == null || (currentResolutionOverride != null && ((float)currentResolutionOverride.width != pixelWidth || (float)currentResolutionOverride.height != pixelHeight)))
		{
			currentResolutionOverride = null;
			if (resolutionOverride != null)
			{
				tk2dCameraResolutionOverride[] array = resolutionOverride;
				foreach (tk2dCameraResolutionOverride tk2dCameraResolutionOverride2 in array)
				{
					if (tk2dCameraResolutionOverride2.Match((int)pixelWidth, (int)pixelHeight))
					{
						currentResolutionOverride = tk2dCameraResolutionOverride2;
						break;
					}
				}
			}
		}
		float num = ((currentResolutionOverride == null) ? 1f : currentResolutionOverride.scale);
		float num2 = 0f;
		float num3 = 0f;
		float num4 = pixelWidth;
		float num5 = pixelHeight;
		float farClipPlane = mainCamera.farClipPlane;
		float near = mainCamera.nearClipPlane;
		orthoSize = (num5 - num3) / 2f;
		resolution = new Vector2(num4 / num, num5 / num);
		bool flag = false;
		float num6 = ((Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor) ? 0f : 1f);
		float value = 2f / (num4 - num2) * num;
		float value2 = 2f / (num5 - num3) * num;
		float value3 = -2f / (farClipPlane - near);
		float value4 = (0f - (num4 + num2 + num6)) / (num4 - num2);
		float value5 = (0f - (num3 + num5 - num6)) / (num5 - num3);
		float value6 = (0f - 2f * farClipPlane * near) / (farClipPlane - near);
		Matrix4x4 projectionMatrix = default(Matrix4x4);
		projectionMatrix[0, 0] = value;
		projectionMatrix[0, 1] = 0f;
		projectionMatrix[0, 2] = 0f;
		projectionMatrix[0, 3] = value4;
		projectionMatrix[1, 0] = 0f;
		projectionMatrix[1, 1] = value2;
		projectionMatrix[1, 2] = 0f;
		projectionMatrix[1, 3] = value5;
		projectionMatrix[2, 0] = 0f;
		projectionMatrix[2, 1] = 0f;
		projectionMatrix[2, 2] = value3;
		projectionMatrix[2, 3] = value6;
		projectionMatrix[3, 0] = 0f;
		projectionMatrix[3, 1] = 0f;
		projectionMatrix[3, 2] = 0f;
		projectionMatrix[3, 3] = 1f;
		mainCamera.projectionMatrix = projectionMatrix;
	}
}
