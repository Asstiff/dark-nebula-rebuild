using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
	public float scrollValue = 1f;

	private CameraFollow cameraFollow;

	private static float VirtualScreenHeight = Constants.OriginalScreenHeight;

	private static float VirtualScreenHeightHalf = VirtualScreenHeight / 2f;

	private float _height;

	private Vector3 pos;

	private float winZ;

	private float z;

	private Transform _transform;

	private void Awake()
	{
		_transform = base.transform;
	}

	private void Start()
	{
		_height = 64000f - (float)World.mapHeight * 64f - VirtualScreenHeight;
		cameraFollow = World.cameraFollow;
		pos = _transform.position;
	}

	private void OnDisable()
	{
		cameraFollow = null;
	}

	private void LateUpdate()
	{
		winZ = cameraFollow.windowPosZ;
		z = (_height - winZ) * scrollValue;
		z = _height - z;
		pos.z = winZ - z - VirtualScreenHeightHalf - (0.5f - scrollValue) * VirtualScreenHeight;
		_transform.position = pos;
	}
}
