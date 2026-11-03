using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
	public float secondsToLive = 1f;

	private float _started;

	private void Start()
	{
		_started = Time.time;
	}

	private void Update()
	{
		if (_started + secondsToLive < Time.time)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
