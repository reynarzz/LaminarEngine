using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Data
{
    public interface IEngineService
    {
    }

    internal class EngineServices
    {
        private static readonly Dictionary<Type, IEngineService> _services = new();
        internal static T GetService<T>() where T : class, IEngineService, new()
        {
            if(!_services.TryGetValue(typeof(T), out var service))
            {
                service = new T();
                _services.Add(typeof(T), service);
            }

            return service as T;
        }
    }
}
