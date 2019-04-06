using System.Collections.Generic;
using System.Linq;
using Autofac;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Implementation;
using WorldHexagonMap.Loader.Implementation.HexagonParsers;
using WorldHexagonMap.Loader.Implementation.GeoDataLoaders;
using WorldHexagonMap.Loader.Contracts;

namespace WorldHexagonMap.Loader.VectorTileGenerator.Console
{
    public static class IoCService
    {
        private static IContainer _container;

        public static void Initialize()
        {
            var catalog = new AggregateCatalog(new AssemblyCatalog(typeof (VectorLoaderProcess).Assembly));

            var builder = new ContainerBuilder();

            builder.RegisterComposablePartCatalog(catalog);

            builder.Register(c => new HexagonDefinition(10))
                .As<HexagonDefinition>()
                .Exported(e => e.As<HexagonDefinition>());

            builder
                .RegisterType<VectorLoaderProcess>()
                .As<IVectorLoaderProcess>()
                .Exported(e => e.As<IVectorLoaderProcess>());

            builder
                .RegisterType<HexagonService>()
                .As<IHexagonService>()
                .As(e => e.As<IHexagonService>());

            builder.RegisterType<HexagonParserFactory>()
                .As<IHexagonParserFactory>()
                .Exported(e => e.As<IHexagonParserFactory>());

            builder.RegisterType<GeoDataLoaderFactory>()
                .As<IGeoDataLoaderFactory>()
                .Exported(e => e.As<IGeoDataLoaderFactory>());

            builder.RegisterType<ComponentFactory>()
                .As<IComponentFactory>()
                .Exported(e => e.As<IComponentFactory>());

            _container = builder.Build();
        }

        public static T GetInstance<T>()
        {
            return _container.Resolve<T>();
        }

        public static T GetInstanceWithParameters<T>(params KeyValuePair<string, object>[] arguments)
        {
            return _container.Resolve<T>(arguments.Select(a => new NamedParameter(a.Key, a.Value)));
        }
    }
}