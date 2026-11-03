using System;
using System.Collections;
using System.Collections.Generic;
using FLD;
using UnityEngine;

[AddComponentMenu("Gameplay/ObjectPool")]
public class ObjectPool : MonoBehaviour
{
	[Serializable]
	public class ObjectPoolEntry
	{
		[SerializeField]
		public Entity Prefab;

		[SerializeField]
		public int Count;
	}

	private static Vector3 offscreen = new Vector3(0f, 1000f, 0f);

	public Dictionary<Type, Pool<Entity>> _pools;

	public ObjectPoolEntry[] Entries;

	protected GameObject ContainerObject;

	private Transform _transform;

	public static ObjectPool instance { get; private set; }

	private void OnEnable()
	{
		instance = this;
		_transform = base.transform;
	}

	private void OnDisable()
	{
		instance = null;
	}

	private void Start()
	{
		_pools = new Dictionary<Type, Pool<Entity>>();
		ObjectPoolEntry[] entries = Entries;
		foreach (ObjectPoolEntry objectPoolEntry in entries)
		{
			Entity prefab = objectPoolEntry.Prefab;
			_pools.Add(objectPoolEntry.Prefab.GetType(), new Pool<Entity>(() =>
			{
				Entity entity = (Entity)UnityEngine.Object.Instantiate(prefab);
				entity.transform.parent = _transform;
				entity.transform.position = offscreen;
				return entity;
			}, objectPoolEntry.Count));
		}
		GameObject gameObject = new GameObject("TmpCamera");
		Camera camera = gameObject.AddComponent<Camera>();
		camera.orthographic = true;
		camera.nearClipPlane = -100f;
		camera.farClipPlane = 100f;
		camera.depth = -100f;
		camera.transform.position = offscreen;
		camera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
		StartCoroutine("RemoveTemporaryCamera", gameObject);
	}

	private IEnumerator RemoveTemporaryCamera(GameObject pG)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		UnityEngine.Object.Destroy(pG);
	}

	public T GetObjectForType<T>() where T : Entity
	{
		Pool<Entity> value;
		if (_pools.TryGetValue(typeof(T), out value))
		{
			return value.Take() as T;
		}
		Debug.Log("No pool of that type! " + typeof(T).ToString());
		return (T)null;
	}

	public void PoolObject<T>(T pEntity) where T : Entity
	{
		Pool<Entity> value;
		if (_pools.TryGetValue(typeof(T), out value))
		{
			value.Recycle(pEntity);
		}
		else
		{
			Debug.Log("No pool of that type! " + typeof(T).ToString());
		}
		pEntity.transform.parent = _transform;
		pEntity.transform.position = offscreen;
	}
}
