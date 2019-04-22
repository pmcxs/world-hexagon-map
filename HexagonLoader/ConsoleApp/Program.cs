//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Compression;
//using CommandLine;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//
//namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
//{
//    internal class CommandObject
//    {
//        [Option('i', "input")]
//        public string Input { get; set; }
//        
//        [Option('o', "output")]
//        public string Output { get; set; }
//        
//        [Option('t', "temp")]
//        public string Temporary { get; set; }
//        
//        [Option('e', "export")]
//        public string ExportHandler { get; set; }
//    }
//
//    internal class Program
//    {
//        private static void Main(string[] args)
//        {
//            
//            Parser.Default.ParseArguments<CommandObject>(args)
//                .WithParsed(RunOptionsAndReturnExitCode)
//                .WithNotParsed(HandleParseError);
//        }
//
//        private static void HandleParseError(IEnumerable<Error> obj)
//        {
//            throw new NotImplementedException();
//        }
//
//        private static void RunOptionsAndReturnExitCode(CommandObject command)
//        {
//            var serviceProvider = IoCService.Initialize();
//
//            var loader = serviceProvider.GetService<IHexagonDataLoaderPipelineService>();
//
//            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
//
//            logger.LogInformation("Start");
//
//            foreach (var zipFile in Directory.EnumerateFiles(command.Input))
//            {
//                if (string.IsNullOrEmpty(zipFile)) throw new Exception("ZipFile is null or empty");
//
//                var temporaryFile = Path.Combine(command.Temporary, Path.GetFileName(zipFile));
//
//                File.Move(zipFile, temporaryFile);
//
//                logger.LogInformation("Processing file " + zipFile);
//
//                var targetFolder = Path.Combine(command.Temporary, Path.GetFileNameWithoutExtension(zipFile));
//                ZipFile.ExtractToDirectory(temporaryFile, targetFolder);
//
//                loader.Process(Path.Combine(targetFolder, "manifest.xml"), command.ExportHandler).Wait();
//
//                File.Move(temporaryFile, Path.Combine(command.Output, Path.GetFileName(zipFile)));
//
//                Directory.Delete(targetFolder, true);
//            }
//
//            Console.ReadLine();
//        }
//    }
//}