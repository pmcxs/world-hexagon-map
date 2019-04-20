
/*
using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{


    internal enum InputType
    {
        None,
        Shapefile,
        Raster,
        XYZ,
        GeoJson,
        Manifest
    }
    
    
    internal class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('i', "input")]
        public string Input { get; set; }
        
        [Option(longName:"inputtype")]
        public InputType InputType { get; set; }
        
        [Option('t', "targets")]
        public IEnumerable<string> Targets { get; set; }


        internal LayersConfiguration GetLayersConfiguration()
        {
            var targets = new List<LayersLoaderTarget>();

            foreach (string target in Targets)
            {
                string[] lines = target.Split(":");

                switch (lines.Length)
                {
                    case 1:
                        targets.Add(new LayersLoaderTarget { Field = lines[0], Destination = lines[0]});
                        break;
                    case 2:
                        targets.Add(new LayersLoaderTarget { Field = lines[0], Destination = lines[1]});
                        break;
                }
            }
            
            var layers = new LayersConfiguration
            {
                Source = Input, 
                Targets = targets.ToArray()
            };

            return layers;

        }

        internal IGeoDataParser GetParser(ServiceProvider serviceProvider)
        {
            var factory = serviceProvider.GetService<IGeoDataParserFactory>();

            if (InputType == InputType.None)
            {
                return factory.GetInstance(Input);
            }

            switch (InputType)
            {
                case InputType.Raster:
                    return factory.GetInstance<GeoTiffParser>();
                case InputType.Shapefile:
                    return factory.GetInstance<ShapefileParser>();
                case InputType.XYZ:
                    return factory.GetInstance<XYZParser>();
                case InputType.GeoJson:
                    return factory.GetInstance<GeojsonParser>();
                default:
                    throw new NotSupportedException($"{InputType} not supported");
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptionsAndReturnExitCode)
                .WithNotParsed(HandleParseError);
            
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            var serviceProvider = IoCService.Initialize();

            
            
            var layer = opts.GetLayersConfiguration();
            
            var loader = serviceProvider.GetService<IHexagonDataLoaderService>();

            loader.Process(new Layers {Sources = new[] {layer}}, "").Wait();

        }
    }
}*/