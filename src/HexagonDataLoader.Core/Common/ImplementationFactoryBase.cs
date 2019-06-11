using System;
using System.Collections.Generic;

namespace HexagonDataLoader.Core.Common
{
    public abstract class ImplementationFactoryBase<T>
    {
        private readonly IDictionary<string, T> _implementations = new Dictionary<string, T>();

        public void RegisterImplementation<T1>(string implementation) where T1 : T
        {
            T1 instance = Activator.CreateInstance<T1>();
            _implementations.Add(implementation, instance);
        }

        public T GetInstance(string implementation)
        {

            if (_implementations.ContainsKey(implementation ?? throw new Exception("implementation can't be null")))
            {
                return _implementations[implementation];
            }

            throw new NotSupportedException($"Unknown implementation: {implementation}");

        }
    }
}
