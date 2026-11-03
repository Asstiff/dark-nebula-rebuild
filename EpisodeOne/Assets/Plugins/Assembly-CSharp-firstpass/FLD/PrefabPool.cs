using System;
using UnityEngine;

namespace FLD
{
	public sealed class PrefabPool : IPool<Component>
	{
		public enum RecyclingStrategy
		{
			SetAside = 0,
			Deactivate = 1,
			DoNothing = 2
		}

		private Component _prefab;

		private RecyclingStrategy _recyclingStrategy;

		private IPool<Component> _underlyingPool;

		public int Balance
		{
			get
			{
				return _underlyingPool.Balance;
			}
		}

		public int Count
		{
			get
			{
				return _underlyingPool.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return _underlyingPool.IsEmpty;
			}
		}

		public bool CanTake
		{
			get
			{
				return _underlyingPool.CanTake;
			}
		}

		public PrefabPool(Component pPrefab)
			: this(pPrefab, 0)
		{
		}

		public PrefabPool(Component pPrefab, int pInitialCount)
			: this(pPrefab, pInitialCount, RecyclingStrategy.Deactivate)
		{
		}

		public PrefabPool(Component pPrefab, int pInitialCount, RecyclingStrategy pRecyclingStrategy)
		{
			if (pPrefab == null)
			{
				throw new ArgumentNullException("pPrefab");
			}
			_prefab = pPrefab;
			_recyclingStrategy = pRecyclingStrategy;
			_underlyingPool = new Pool<Component>(MakePrefab, pInitialCount);
		}

		private Component MakePrefab()
		{
			Component component = (Component)UnityEngine.Object.Instantiate(_prefab);
			component.transform.parent = PrefabPoolContainer.Get("PooledGeneric").transform;
			CleanUpPrefab(component);
			return component;
		}

		public Component Take()
		{
			Component component = _underlyingPool.Take();
			if (component != null)
			{
				if (_recyclingStrategy == RecyclingStrategy.Deactivate)
				{
					component.gameObject.SetActive(true);
				}
				if (component.GetComponent<Renderer>() != null)
				{
					component.GetComponent<Renderer>().enabled = true;
				}
			}
			return component;
		}

		public void Recycle(Component pObject)
		{
			CleanUpPrefab(pObject);
			_underlyingPool.Recycle(pObject);
		}

		private void CleanUpPrefab(Component pObject)
		{
			switch (_recyclingStrategy)
			{
			case RecyclingStrategy.SetAside:
				pObject.transform.position = new Vector3(2345f, 0f, 0f);
				if (pObject.GetComponent<Renderer>() != null)
				{
					pObject.GetComponent<Renderer>().enabled = false;
				}
				break;
			case RecyclingStrategy.Deactivate:
				pObject.gameObject.SetActive(false);
				break;
			case RecyclingStrategy.DoNothing:
				break;
			}
		}
	}
}
