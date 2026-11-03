using System;
using System.Collections;
using UnityEngine;

namespace FLD
{
	public static class Timer
	{
		private static TimerSurrogate _surrogate;

		public static event Action ApplicationPaused
		{
			add
			{
				GetSurrogate().ApplicationPaused += value;
			}
			remove
			{
				GetSurrogate().ApplicationPaused -= value;
			}
		}

		public static event Action ApplicationUnpaused
		{
			add
			{
				GetSurrogate().ApplicationUnpaused += value;
			}
			remove
			{
				GetSurrogate().ApplicationUnpaused -= value;
			}
		}

		public static event Action ApplicationQuit
		{
			add
			{
				GetSurrogate().ApplicationQuit += value;
			}
			remove
			{
				GetSurrogate().ApplicationQuit -= value;
			}
		}

		private static TimerSurrogate GetSurrogate()
		{
			if (_surrogate == null)
			{
				GameObject gameObject = new GameObject("Timer Surrogate");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_surrogate = gameObject.AddComponent<TimerSurrogate>();
			}
			return _surrogate;
		}

		public static Async Delay(float pDelaySeconds, Action pAction)
		{
			Async async = new Async();
			if (pAction != null)
			{
				GetSurrogate().StartCoroutine(DelayCoroutine(pAction, async, pDelaySeconds));
			}
			else
			{
				Debug.LogError("null delegate");
				async.Complete();
			}
			return async;
		}

		private static IEnumerator DelayCoroutine(Action pAction, Async pAsync, float pDelay)
		{
			yield return new WaitForSeconds(pDelay);
			pAction();
			pAsync.Complete();
		}

		public static Async<TRetVal> Delay<TRetVal>(float pDelaySeconds, Func<TRetVal> pFunc)
		{
			Async<TRetVal> async = new Async<TRetVal>();
			if (pFunc != null)
			{
				GetSurrogate().StartCoroutine(DelayCoroutine(pFunc, async, pDelaySeconds));
			}
			else
			{
				Debug.LogError("null delegate");
				async.Complete(default(TRetVal));
			}
			return async;
		}

		private static IEnumerator DelayCoroutine<TRetVal>(Func<TRetVal> pFunc, Async<TRetVal> pAsync, float pDelay)
		{
			yield return new WaitForSeconds(pDelay);
			TRetVal rv = pFunc();
			pAsync.Complete(rv);
		}

		public static Async<T> WaitFor<T>(T pOperation) where T : class
		{
			Async<T> async = new Async<T>();
			GetSurrogate().StartCoroutine(WaitForCoroutine(pOperation, async));
			return async;
		}

		private static IEnumerator WaitForCoroutine<T>(T pOperation, Async<T> pAsync)
		{
			yield return pOperation;
			pAsync.Complete(pOperation);
		}

		public static Coroutine StartCoroutine(IEnumerator pCoroutine)
		{
			return GetSurrogate().StartCoroutine(pCoroutine);
		}
	}
}
