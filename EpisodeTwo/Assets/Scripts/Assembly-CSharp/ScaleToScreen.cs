using UnityEngine;

[DefaultExecutionOrder(100)]
public class ScaleToScreen : MonoBehaviour
{
	public Camera HudCamera;
	private Vector3 referenceScale;
	private float lastSize = -1f, lastAspect = -1f;
	private bool ready;

	private void Start()
	{
		if (GetComponentInParent<LoadLevelScreen>() != null) return;
		referenceScale = transform.localScale;
		ready = true;
		UpdateScale();
	}

	private void LateUpdate()
	{
		if (ready && HudCamera != null && (!Mathf.Approximately(lastSize, HudCamera.orthographicSize) || !Mathf.Approximately(lastAspect, HudCamera.aspect))) UpdateScale();
	}

	private void UpdateScale()
	{
		if (HudCamera == null) return;
		transform.localScale = referenceScale;
		Vector3 size = GetComponent<Renderer>().bounds.size;
		float width = 2f * HudCamera.orthographicSize * HudCamera.aspect;
		float scale = width / Mathf.Max(size.x, 0.01f);
		transform.localScale = Vector3.Scale(referenceScale, new Vector3(scale, scale, 1f));
		lastSize = HudCamera.orthographicSize;
		lastAspect = HudCamera.aspect;
	}
}
