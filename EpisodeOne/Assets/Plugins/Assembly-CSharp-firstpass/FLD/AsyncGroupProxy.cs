using System.Collections.Generic;

namespace FLD
{
	internal class AsyncGroupProxy : Async
	{
		private List<Async> _asyncs;

		public AsyncGroupProxy(IEnumerable<Async> pAsyncs)
		{
			_asyncs = new List<Async>(pAsyncs);
			if (_asyncs.Count == 0)
			{
				Complete();
				return;
			}
			for (int i = 0; i < _asyncs.Count; i++)
			{
				_asyncs[i].Wait(CheckCompletion);
			}
		}

		private void CheckCompletion()
		{
			for (int i = 0; i < _asyncs.Count; i++)
			{
				if (!_asyncs[i].IsCompleted)
				{
					return;
				}
			}
			Complete();
		}
	}
}
