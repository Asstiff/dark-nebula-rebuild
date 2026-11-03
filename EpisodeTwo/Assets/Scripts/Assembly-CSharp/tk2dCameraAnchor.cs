using UnityEngine;

[AddComponentMenu("2D Toolkit/Camera/tk2dCameraAnchor")]
[ExecuteInEditMode]
public class tk2dCameraAnchor : MonoBehaviour
{
	public enum Anchor
	{
		UpperLeft = 0,
		UpperCenter = 1,
		UpperRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		LowerLeft = 6,
		LowerCenter = 7,
		LowerRight = 8
	}

	public Anchor anchor;

	public Vector2 offset = Vector2.zero;

	public tk2dCamera tk2dCamera;

	public Camera mainCamera;

	private Transform _transform;

	private void Awake()
	{
		_transform = base.transform;
	}

	private void Start()
	{
		UpdateTransform();
	}

	private void UpdateTransform()
	{
		if (tk2dCamera != null && mainCamera != null)
		{
			Vector3 localPosition = _transform.localPosition;
			Vector3 vector = Vector3.zero;
			switch (anchor)
			{
			case Anchor.UpperLeft:
				vector = new Vector3(0f, tk2dCamera.resolution.y, localPosition.z);
				break;
			case Anchor.UpperCenter:
				vector = new Vector3(tk2dCamera.resolution.x / 2f, tk2dCamera.resolution.y, localPosition.z);
				break;
			case Anchor.UpperRight:
				vector = new Vector3(tk2dCamera.resolution.x, tk2dCamera.resolution.y, localPosition.z);
				break;
			case Anchor.MiddleLeft:
				vector = new Vector3(0f, tk2dCamera.resolution.y / 2f, localPosition.z);
				break;
			case Anchor.MiddleCenter:
				vector = new Vector3(tk2dCamera.resolution.x / 2f, tk2dCamera.resolution.y / 2f, localPosition.z);
				break;
			case Anchor.MiddleRight:
				vector = new Vector3(tk2dCamera.resolution.x, tk2dCamera.resolution.y / 2f, localPosition.z);
				break;
			case Anchor.LowerLeft:
				vector = new Vector3(0f, 0f, localPosition.z);
				break;
			case Anchor.LowerCenter:
				vector = new Vector3(tk2dCamera.resolution.x / 2f, 0f, localPosition.z);
				break;
			case Anchor.LowerRight:
				vector = new Vector3(tk2dCamera.resolution.x, 0f, localPosition.z);
				break;
			}
			Vector3 vector2 = vector + new Vector3(offset.x, offset.y, 0f);
			Vector3 localPosition2 = _transform.localPosition;
			if (localPosition2 != vector2)
			{
				_transform.localPosition = vector2;
			}
		}
	}

	private void Update()
	{
		UpdateTransform();
	}
}
