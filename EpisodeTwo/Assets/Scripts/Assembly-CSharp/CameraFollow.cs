using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public float windowPosZ;

	public float windowPosX;

	public Camera LightCamera;

	public Renderer RenderedLight;

	private float scrollLine;

	private Transform _transform;

	public bool UseCameraManager = true;

	private void Awake()
	{
		EnableDarkness(false);
	}

	private void Start()
	{
		_transform = base.transform;
		scrollLine = 240f;
		windowPosZ = _transform.position.z - scrollLine;
	}

	public void EnableDarkness(bool on)
	{
		LightCamera.gameObject.SetActive(on);
		RenderedLight.enabled = on;
		RenderedLight.GetComponent<Renderer>().sharedMaterial.renderQueue = 4000;
	}

	private void LateUpdate()
	{
		if (Time.timeScale != 0f && !(World.CameraManager == null))
		{
			World.CameraManager.UpdateCamera();
			Vector3 position = _transform.position;
			if (World.vessel != null && !UseCameraManager)
			{
				position.x = World.vessel.transform.position.x;
				position.z = World.vessel.transform.position.z;
			}
			else
			{
				position.x = World.CameraManager.cameraPosition.x;
				position.z = World.CameraManager.cameraPosition.y;
			}
			windowPosX = position.x;
			windowPosZ = position.z;
			_transform.position = position;
		}
	}
}
