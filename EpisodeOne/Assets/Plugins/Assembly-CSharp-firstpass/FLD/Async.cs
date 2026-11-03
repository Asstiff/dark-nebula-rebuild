using System;
using System.Collections.Generic;

namespace FLD
{
	public class Async
	{
		private bool _completed;

		private List<Action> _waitCallbacks = new List<Action>();

		public bool IsCompleted
		{
			get
			{
				return _completed;
			}
		}

		public static Async MakePrecompleted()
		{
			Async async = new Async();
			async.Complete();
			return async;
		}

		public static Async<TResult> MakePrecompleted<TResult>(TResult pResult)
		{
			return Async<TResult>.MakePrecompleted(pResult);
		}

		public void Complete()
		{
			if (_completed)
			{
				return;
			}
			_completed = true;
			if (_waitCallbacks != null)
			{
				for (int i = 0; i < _waitCallbacks.Count; i++)
				{
					_waitCallbacks[i]();
				}
				_waitCallbacks = null;
			}
		}

		public void Wait(Action pCallback)
		{
			if (_completed)
			{
				pCallback();
			}
			else
			{
				_waitCallbacks.Add(pCallback);
			}
		}

		public Async Then(Action pContinuation)
		{
			Async proxy = new Async();
			Wait(() =>
			{
				pContinuation();
				proxy.Complete();
			});
			return proxy;
		}

		public Async<TNewResult> Then<TNewResult>(Func<TNewResult> pContinuation)
		{
			Async<TNewResult> proxy = new Async<TNewResult>();
			Wait(() =>
			{
				proxy.Complete(pContinuation());
			});
			return proxy;
		}

		public Async Then(Func<Async> pContinuation)
		{
			Async proxy = new Async();
			Wait(() =>
			{
				pContinuation().Wait(proxy.Complete);
			});
			return proxy;
		}

		public Async<TNewResult> Then<TNewResult>(Func<Async<TNewResult>> pContinuation)
		{
			Async<TNewResult> proxy = new Async<TNewResult>();
			Wait(() =>
			{
				pContinuation().Wait((TNewResult v) =>
				{
					proxy.Complete(v);
				});
			});
			return proxy;
		}

		public static void WaitMultiple(IEnumerable<Async> pAsyncs, Action pCallback)
		{
			Group(pAsyncs).Wait(pCallback);
		}

		public static Async Group(IEnumerable<Async> pAsyncs)
		{
			return new AsyncGroupProxy(pAsyncs);
		}

		public static void WaitMultiple<T>(IEnumerable<Async<T>> pAsyncs, Action<List<T>> pCallback)
		{
			WaitMultiple(pAsyncs, pCallback, new List<T>());
		}

		private static void WaitMultiple<T>(IEnumerable<Async<T>> pAsyncs, Action<List<T>> pCallback, List<T> pResponses)
		{
			List<Async<T>> asyncsLeft = new List<Async<T>>(pAsyncs);
			foreach (Async<T> pAsync in pAsyncs)
			{
				if (!pAsync.IsCompleted)
				{
					pAsync.Wait(delegate
					{
						WaitMultiple(asyncsLeft, pCallback, pResponses);
					});
					return;
				}
				pResponses.Add(pAsync.Result);
				asyncsLeft.Remove(pAsync);
			}
			pCallback(pResponses);
		}
	}
	public class Async<TResult>
	{
		private TResult _result = default(TResult);

		private List<Action<TResult>> _waitCallbacks = new List<Action<TResult>>();

		public bool IsCompleted { get; private set; }

		public TResult Result
		{
			get
			{
				return _result;
			}
		}

		public static Async<TResult> MakePrecompleted(TResult pSynchronousResult)
		{
			Async<TResult> async = new Async<TResult>();
			async.Complete(pSynchronousResult);
			return async;
		}

		public Async<TResult> Complete(TResult pResult)
		{
			if (!IsCompleted)
			{
				_result = pResult;
				IsCompleted = true;
				if (_waitCallbacks != null)
				{
					for (int i = 0; i < _waitCallbacks.Count; i++)
					{
						_waitCallbacks[i](_result);
					}
					_waitCallbacks = null;
				}
			}
			return this;
		}

		public void Wait(Action<TResult> pCallback)
		{
			if (IsCompleted)
			{
				pCallback(_result);
			}
			else
			{
				_waitCallbacks.Add(pCallback);
			}
		}

		public Async Then(Action<TResult> pContinuation)
		{
			Async proxy = new Async();
			Wait((TResult v) =>
			{
				pContinuation(v);
				proxy.Complete();
			});
			return proxy;
		}

		public Async<TNewResult> Then<TNewResult>(Func<TResult, TNewResult> pContinuation)
		{
			Async<TNewResult> proxy = new Async<TNewResult>();
			Wait((TResult v) =>
			{
				proxy.Complete(pContinuation(v));
			});
			return proxy;
		}

		public Async Then(Func<TResult, Async> pContinuation)
		{
			Async proxy = new Async();
			Wait((TResult v) =>
			{
				pContinuation(v).Wait(proxy.Complete);
			});
			return proxy;
		}

		public Async<TNewResult> Then<TNewResult>(Func<TResult, Async<TNewResult>> pContinuation)
		{
			Async<TNewResult> proxy = new Async<TNewResult>();
			Wait((TResult v) =>
			{
				pContinuation(v).Wait((TNewResult w) =>
				{
					proxy.Complete(w);
				});
			});
			return proxy;
		}
	}
}
