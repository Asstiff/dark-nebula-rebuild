using System.Collections.Generic;
using UnityEngine;

public class MatrixChaserSpawn : Entity
{
	private static List<MatrixChaser> _chasers;

	private static List<MatrixChaser> _pool;

	private static int lastLevelLoad = -1;

	private static Vector3 offscreen = new Vector3(0f, 1000f, 0f);

	public GameObject MatrixChaserPrefab;

	private int lastSpawnTime;

	private bool _isControlSpawner;

	private void Start()
	{
		if (lastLevelLoad != Time.frameCount)
		{
			_isControlSpawner = true;
			lastLevelLoad = Time.frameCount;
			_chasers = new List<MatrixChaser>();
			_pool = new List<MatrixChaser>();
			for (int i = 0; i < 6; i++)
			{
				_pool.Add(CreateChaser());
			}
		}
	}

	private void OnDisable()
	{
		_chasers = new List<MatrixChaser>();
		_pool = new List<MatrixChaser>();
	}

	private MatrixChaser GetChaserFromPool()
	{
		int num = _pool.Count - 1;
		MatrixChaser matrixChaser;
		if (num >= 0)
		{
			matrixChaser = _pool[num];
			_pool.RemoveAt(num);
		}
		else
		{
			matrixChaser = CreateChaser();
		}
		matrixChaser.transform.position = base.transform.position + new Vector3(128f, 0f, 0f);
		matrixChaser.IsFrozen = false;
		return matrixChaser;
	}

	private void ReturnToPool(MatrixChaser pObject)
	{
		pObject.IsFrozen = true;
		pObject.transform.position = offscreen;
		_pool.Add(pObject);
	}

	private MatrixChaser CreateChaser()
	{
		GameObject gameObject = Object.Instantiate(MatrixChaserPrefab) as GameObject;
		gameObject.AddComponent<MeshDistortionParent>();
		MatrixChaser component = gameObject.GetComponent<MatrixChaser>();
		component.transform.position = offscreen;
		component.IsFrozen = true;
		return component;
	}

	private void Update()
	{
		if (World.IsFrozen(base.transform.position.z) || (float)(World.multipliedStartTime.Value / 100000) + 7f > Time.time)
		{
			return;
		}
		int num = Mathf.FloorToInt((Time.time + 0.66f - 1f) / 4.266667f);
		if (num == lastSpawnTime)
		{
			return;
		}
		lastSpawnTime = num;
		if (_isControlSpawner)
		{
			for (int num2 = _chasers.Count - 1; num2 >= 0; num2--)
			{
				if (World.IsFrozen(_chasers[num2].transform.position.z))
				{
					ReturnToPool(_chasers[num2]);
					_chasers.RemoveAt(num2);
				}
			}
		}
		foreach (MatrixChaser chaser in _chasers)
		{
			if (Mathf.Abs(chaser.transform.position.z - base.transform.position.z) < 380f)
			{
				return;
			}
		}
		_chasers.Add(GetChaserFromPool());
	}
}
