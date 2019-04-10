using Microsoft.Extensions.DependencyInjection;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.ConsoleApp.Configuration;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;
using WorldHexagonMap.HexagonDataLoader.ResultExporters;
using WorldHexagonMap.HexagonDataLoader.ResultPostProcessors;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    public static class IoCService
    {
        public static ServiceProvider Initialize()
        {
            //TODO: Setup IoC Container

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton(new LoaderConfiguration { Parallelism = 4})
                .AddSingleton(new HexagonDefinition(10))
                .AddSingleton<IHexagonDataLoaderService, HexagonDataLoaderService>()
                .AddSingleton<IHexagonService, HexagonService>()
                .AddSingleton<IGeoDataParserFactory, GeoDataParserFactory>()
                .AddSingleton<IHexagonProcessorFactory, HexagonProcessorFactory>()
                .AddSingleton<IValueHandlerFactory, ValueHandlerFactory>()
                .AddSingleton<IPostProcessorFactory, PostProcessorFactory>()
                .AddSingleton<IResultExporterFactory, ResultExporterFactory>()
                .BuildServiceProvider();

            serviceProvider.GetService<IHexagonDataLoaderService>();

            return serviceProvider;
        }
    }
}