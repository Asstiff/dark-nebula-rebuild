using System;
using System.Collections.Generic;

namespace FLD
{
	public class Pool<T> : IPool<T> where T : class
	{
		private List<T> _storage = new List<T>();

		private Func<T> _creator;

		private bool _allowTakingWhenEmpty;

		public int Count
		{
			get
			{
				return _storage.Count;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return Count == 0;
			}
		}

		public bool CanTake
		{
			get
			{
				return _allowTakingWhenEmpty || !IsEmpty;
			}
		}

		public Pool(Func<T> pCreator, int pInitialCount = 0, bool pAllowTakingWhenEmpty = true)
		{
			if (pCreator == null)
			{
				throw new ArgumentNullException("pCreator");
			}
			if (pInitialCount < 0)
			{
				throw new ArgumentOutOfRangeException("pInitialCount", "Initial count cannot be negative");
			}
			_creator = pCreator;
			_allowTakingWhenEmpty = pAllowTakingWhenEmpty;
			_storage.Capacity = pInitialCount;
			while (_storage.Count < pInitialCount)
			{
				_storage.Add(_creator());
			}
		}

		public T Take()
		{
			if (IsEmpty)
			{
				if (_allowTakingWhenEmpty)
				{
					return (T)_creator();
				}
				return (T)null;
			}
			T result = _storage[_storage.Count - 1];
			_storage.RemoveAt(_storage.Count - 1);
			return result;
		}

		public void Recycle(T pObject)
		{
			_storage.Add(pObject);
		}
	}
}
