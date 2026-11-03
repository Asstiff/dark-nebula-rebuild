using UnityEngine;

public class Blink : MonoBehaviour
{
	public float interval = 0.5f;

	public bool startVisible;

	private Renderer[] _renderers;

	private bool _visible = true;

	private void Awake()
	{
		_renderers = GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		if (startVisible)
		{
			Invoke("Toggle", interval);
		}
		else
		{
			Toggle();
		}
	}

	private void Toggle()
	{
		_visible = !_visible;
		Renderer[] renderers = _renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.enabled = _visible;
		}
		Invoke("Toggle", interval);
	}
}
