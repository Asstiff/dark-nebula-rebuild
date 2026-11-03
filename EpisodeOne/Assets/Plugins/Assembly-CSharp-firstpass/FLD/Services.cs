using System;
using System.Collections.Generic;

namespace FLD
{
	public static class Services
	{
		private class ServiceInfo
		{
			public IService Provider;

			public ServiceStatus Status;
		}

		private static Dictionary<Type, ServiceInfo> _loadedServices = new Dictionary<Type, ServiceInfo>();

		private static bool _initializationStarted;

		private static Async _initializationAsync;

		public static Async Initialize()
		{
			if (!_initializationStarted)
			{
				_initializationStarted = true;
				SystemConfiguration.Instance.RegisterServices();
				IEnumerable<Async> pAsyncs = _loadedServices.Values.Map((ServiceInfo srvInfo) => srvInfo.Provider.Initialize().Then((bool ok) =>
				{
					FinishServiceInit(srvInfo, ok);
				}));
				_initializationAsync = Async.Group(pAsyncs);
			}
			return _initializationAsync;
		}

		private static void FinishServiceInit(ServiceInfo pService, bool pSuccess)
		{
			pService.Status = ((!pSuccess) ? ServiceStatus.Error : ServiceStatus.Ready);
		}

		public static void RegisterServiceProvider<TService>(TService pProvider) where TService : IService
		{
			if (!_loadedServices.ContainsKey(typeof(TService)))
			{
				ServiceInfo serviceInfo = new ServiceInfo();
				serviceInfo.Provider = pProvider;
				serviceInfo.Status = ServiceStatus.Pending;
				ServiceInfo value = serviceInfo;
				_loadedServices.Add(typeof(TService), value);
			}
		}

		private static ServiceInfo GetInfo<TService>() where TService : IService
		{
			ServiceInfo value;
			_loadedServices.TryGetValue(typeof(TService), out value);
			return value;
		}

		public static ServiceStatus GetStatus<TService>() where TService : IService
		{
			ServiceInfo info = GetInfo<TService>();
			if (info != null)
			{
				return info.Status;
			}
			return ServiceStatus.Unavailable;
		}

		public static TService GetService<TService>() where TService : IService
		{
			ServiceInfo info = GetInfo<TService>();
			if (info != null && info.Status == ServiceStatus.Ready)
			{
				return (TService)info.Provider;
			}
			return default(TService);
		}
	}
}
