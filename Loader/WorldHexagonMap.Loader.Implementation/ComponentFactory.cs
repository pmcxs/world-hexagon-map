using Autofac;
using WorldHexagonMap.Loader.Contracts;

namespace WorldHexagonMap.Loader.Implementation
{
    public class ComponentFactory : IComponentFactory
    {
        private readonly IComponentContext _container;

        public ComponentFactory(IComponentContext container)
        {

            _container = container;
        }


        public T CreateInstance<T>(string contractName = "")
        {
            return _container.ResolveNamed<T>(contractName);
        }
    }
}
