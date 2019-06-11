using System;
using System.Collections.Generic;
using System.Linq;

namespace MapGenerator.Core.Utils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Type> GetImplementationsOfT<T>()
        {
            var type = typeof(T);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && t.IsClass);

            return types;
        }
    }
}