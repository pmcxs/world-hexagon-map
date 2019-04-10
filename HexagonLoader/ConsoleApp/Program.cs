using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    class CommandArgs
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public string Temporary { get; set; }
        public string ExportHandler { get; set; }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var command = Args.Configuration.Configure<CommandArgs>().CreateAndBind(args);

            var serviceProvider = IoCService.Initialize();

            var loader = serviceProvider.GetService<IHexagonDataLoaderService>();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            logger.LogInformation("Start");

            System.Console.ReadLine();
        }
    }
}