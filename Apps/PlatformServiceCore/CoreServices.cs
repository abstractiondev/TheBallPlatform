using System;

namespace TheBall.Core
{
    public class CoreServices
    {
        public static Func<Type, string, string> ServiceNameFunc = (serviceType, instanceName) =>
            $"SRV_{serviceType.FullName}_{instanceName}";

        public static void RegisterService(object service, string instanceName = null)
        {
            var serviceType = service.GetType();
            var serviceItemName = ServiceNameFunc(serviceType, instanceName);
            CallContext.LogicalSetData(serviceItemName, service);
        }

        public static T GetCurrent<T>(string name) where T : ICoreService
        {
            var serviceItemName = ServiceNameFunc(typeof(T), name);
            var service = (T) CallContext.LogicalGetData(serviceItemName);
            return service;
        }

        public static T GetCurrent<T>() where T : ICoreService
        {
            return GetCurrent<T>(null);
        }
    }
}
