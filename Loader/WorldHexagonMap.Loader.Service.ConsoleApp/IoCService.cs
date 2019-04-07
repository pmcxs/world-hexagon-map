using Microsoft.Extensions.DependencyInjection;
using WorldHexagonMap.Core;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Loader.HexagonParsers;
using WorldHexagonMap.Loader.PostProcessors;

namespace WorldHexagonMap.Loader.Service.ConsoleApp
{
    public static class IoCService
    {

        public static ServiceProvider Initialize()
        {

            //TODO: Setup IoC Container

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IHexagonDataLoaderService, HexagonDataLoaderService>()
                .AddSingleton<IHexagonService, HexagonService>()
                .AddSingleton<IHexagonParserFactory, HexagonParserFactory>()
                .AddSingleton<IPostProcessorFactory, PostProcessorFactory>()


                .BuildServiceProvider();

            serviceProvider.GetService<IHexagonDataLoaderService>();

            //builder.Register(c => new HexagonDefinition(10))
            //    .As<HexagonDefinition>()
            //    .Exported(e => e.As<HexagonDefinition>());

            //builder
            //    .RegisterType<LoaderService>()
            //    .As<ILoaderService>()
            //    .Exported(e => e.As<ILoaderService>());

            //builder
            //    .RegisterType<HexagonService>()
            //    .As<IHexagonService>()
            //    .Exported(e => e.As<IHexagonService>());

            //builder.RegisterType<HexagonParserFactory>()
            //    .As<IHexagonParserFactory>()
            //    .Exported(e => e.As<IHexagonParserFactory>());

            //builder.RegisterType<GeoDataLoaderFactory>()
            //    .As<IGeoDataLoaderFactory>()
            //    .Exported(e => e.As<IGeoDataLoaderFactory>());


            //_container = builder.Build();

            return serviceProvider;


        }
        
    }
}