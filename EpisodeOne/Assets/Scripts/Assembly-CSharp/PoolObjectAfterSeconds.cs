using UnityEngine;

public class PoolObjectAfterSeconds : MonoBehaviour
{
	public float secondsToLive = 1f;

	private float _started;

	private Entity _entity;

	private void Start()
	{
		_started = Time.time;
		_entity = GetComponent<Entity>();
	}

	private void Update()
	{
		if (_started + secondsToLive < Time.time && _entity != null)
		{
			ObjectPool.instance.PoolObject(_entity);
		}
	}
}
