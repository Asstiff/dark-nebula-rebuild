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

	public static ObjectPool _instance;

	public Dictionary<Type, Pool<Entity>> _pools;

	public ObjectPoolEntry[] Entries;

	private Transform _transform;

	public static ObjectPool instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = GameObject.Find("!ObjectPool").GetComponent<ObjectPool>();
			}
			return _instance;
		}
		private set
		{
			_instance = value;
		}
	}

	private void OnEnable()
	{
		instance = this;
		_transform = base.transform;
	}

	private void Awake()
	{
		instance = this;
		_transform = base.transform;
	}

	private void OnDisable()
	{
		instance = null;
	}

	public static void AddAsset<T>(string pAssetPath, int pCount) where T : Entity
	{
	}

	public static void AddAsset<T>(T pAsset, int pCount) where T : Entity
	{
		if (!instance.HasAsset<T>())
		{
			int num = instance.Entries.Length + 1;
			ObjectPoolEntry[] array = new ObjectPoolEntry[num];
			instance.Entries.CopyTo(array, 0);
			instance.Entries = array;
			ObjectPoolEntry objectPoolEntry = new ObjectPoolEntry();
			objectPoolEntry.Prefab = pAsset;
			objectPoolEntry.Count = pCount;
			instance.Entries[num - 1] = objectPoolEntry;
		}
	}

	public bool HasAsset<T>()
	{
		ObjectPoolEntry[] entries = Entries;
		foreach (ObjectPoolEntry objectPoolEntry in entries)
		{
			if (objectPoolEntry.Prefab is T)
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		_pools = new Dictionary<Type, Pool<Entity>>();
		ObjectPoolEntry[] entries = Entries;
		foreach (ObjectPoolEntry objectPoolEntry in entries)
		{
			Entity prefab = objectPoolEntry.Prefab;
			Func<Entity> pCreator = () =>
			{
				Entity entity = (Entity)UnityEngine.Object.Instantiate(prefab);
				if (entity == null)
				{
					Debug.LogError("Cannot create from pool " + prefab.GetType());
					return (Entity)null;
				}
				entity.transform.parent = _transform;
				entity.transform.position = offscreen;
				entity.OnReturnedToPool();
				return entity;
			};
			_pools.Add(objectPoolEntry.Prefab.GetType(), new Pool<Entity>(pCreator, objectPoolEntry.Count));
		}
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
			object obj = value.Take();
			T val = obj as T;
			if (val == null)
			{
				Debug.LogWarning("Could not Take " + typeof(T).ToString() + " obj: " + obj);
			}
			else
			{
				val.OnTakenFromPool();
			}
			return val;
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
			pEntity.transform.parent = _transform;
			pEntity.transform.position = offscreen;
			pEntity.OnReturnedToPool();
		}
		else
		{
			Debug.Log("No pool of that type! " + typeof(T).ToString());
		}
	}
}
