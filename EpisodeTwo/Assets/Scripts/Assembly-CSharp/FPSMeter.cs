using UnityEngine;

public class FPSMeter : MonoBehaviour
{
	private float _lastSecond;

	private Rect rect = new Rect(5f, 5f, 100f, 30f);

	public int lastFPS;

	private int frames;

	private void Start()
	{
		_lastSecond = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		float num = Time.realtimeSinceStartup - _lastSecond;
		if (num >= 1f)
		{
			num--;
			lastFPS = frames;
			frames = 1;
			_lastSecond = Time.realtimeSinceStartup - num;
		}
		else
		{
			frames++;
		}
	}

	private void OnGUI()
	{
		GUI.Label(rect, lastFPS.ToString("000"));
	}
}
