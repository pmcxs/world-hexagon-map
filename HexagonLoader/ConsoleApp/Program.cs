using System;
using System.IO;
using System.IO.Compression;
using Args;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorldHexagonMap.HexagonDataLoader.ConsoleApp;

namespace WorldHexagonMap.Loader.Service.ConsoleApp
{
    internal class CommandObject
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
            var command = Configuration.Configure<CommandObject>().CreateAndBind(args);

            IoCService.Initialize();

            var serviceProvider = IoCService.Initialize();

            var loader = serviceProvider.GetService<IHexagonDataLoaderService>();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            logger.LogInformation("Start");

            foreach (var zipFile in Directory.EnumerateFiles(command.Input))
            {
                if (string.IsNullOrEmpty(zipFile)) throw new Exception("ZipFile is null or empty");

                var temporaryFile = Path.Combine(command.Temporary, Path.GetFileName(zipFile));

                File.Move(zipFile, temporaryFile);

                logger.LogInformation("Processing file " + zipFile);

                var targetFolder = Path.Combine(command.Temporary, Path.GetFileNameWithoutExtension(zipFile));
                ZipFile.ExtractToDirectory(temporaryFile, targetFolder);

                loader.Process(Path.Combine(targetFolder, "manifest.xml"), command.ExportHandler).Wait();

                File.Move(temporaryFile, Path.Combine(command.Output, Path.GetFileName(zipFile)));

                Directory.Delete(targetFolder, true);
            }

            Console.ReadLine();
        }
    }
}