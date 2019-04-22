using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors;
using WorldHexagonMap.HexagonDataLoader.ResultExporters;

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
    
    internal enum OutputType
    {
        None,
        Console,
        SQLite
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
        
        [Option('f', "filters")]
        public IEnumerable<string> Filters { get; set; }

        [Option('m', "merge", Required = false, Default = MergeStrategy.Replace, HelpText = "Defines how a new value should be merged with an existing one" )]
        public MergeStrategy Merge { get; set; }

        [Option('o',"output", Required = false)]
        public string Output { get; set; }

        [Option('h',"hexagonsize", Required = true)]
        public double HexagonSize { get; set; }

        [Option(longName:"outputtype", Required = false, Default = OutputType.Console)]
        public OutputType OutputType { get; set; }
        

        
        internal LayersConfiguration GetLayersConfiguration()
        {
            var targets = new List<LayersLoaderTarget>();

            foreach (string target in Targets)
            {
                string[] lines = target.Split(":");

                switch (lines.Length)
                {
                    case 1:
                        targets.Add(new LayersLoaderTarget { SourceField = lines[0], Destination = lines[0]});
                        break;
                    case 2:
                        targets.Add(new LayersLoaderTarget { SourceField = lines[0], Destination = lines[1]});
                        break;
                }
            }

            var filters = new List<LayersLoaderFilter>();

            foreach (string filter in Filters)
            {
                string[] lines = filter.Split(":");

                if (lines.Length != 2)
                {
                    throw new NotSupportedException($"A filter needs 2 parts: <field>:<comparisonValue>");
                }
                
                filters.Add(new LayersLoaderFilter { Field = lines[0], Value = lines[1]});
                
            }
            
            var layers = new LayersConfiguration
            {
                Source = Input, 
                Targets = targets.ToArray(),
                Filters = filters.ToArray()
            };

            return layers;

        }

        internal IGeoDataParser GetGeoDataParser()
        {
            if (InputType == InputType.None)
            {
                var extension = Path.GetExtension(Input);

                switch (extension)
                {
                    case ".json":
                    case ".geojson":
                        return new GeojsonParser();
                    case ".xyz":
                        return new XYZParser();
                    case ".tif":
                        return new GeoTiffParser();
                    case ".shp":
                        return new ShapefileParser();
                    default:
                        throw new NotImplementedException("Unknown extension: " + extension);
                }
            }

            switch (InputType)
            {
                case InputType.GeoJson:
                    return new GeojsonParser();
                case InputType.XYZ:
                    return new XYZParser();
                case InputType.Raster:
                    return new GeoTiffParser();
                case InputType.Shapefile:
                    return new ShapefileParser();
                default:
                    throw new NotSupportedException($"{InputType} not supported");
            }
        }

        public IResultExporter GetResultExporter()
        {
            switch (OutputType)
            {
                case OutputType.None:
                case OutputType.Console:
                    return new ConsoleExporter();
                case OutputType.SQLite:
                    return new SQLiteExporter();
                default:
                    throw new NotSupportedException($"{OutputType} not supported");
            }
        }

        public HexagonDefinition GetHexagonDefinition()
        {
            return new HexagonDefinition(HexagonSize);
        }

    
    }

    internal class Program
    {
        public static void Main(string[] args)
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

            Stopwatch stopwatch = Stopwatch.StartNew();
            
            LayersConfiguration layer = opts.GetLayersConfiguration();
            MergeStrategy mergeStrategy = opts.Merge;

 
            using (IGeoDataParser geoParser = opts.GetGeoDataParser())
            using (IResultExporter exporter = opts.GetResultExporter())
            {
                HexagonDefinition hexagonDefinition = opts.GetHexagonDefinition();

                IEnumerable<GeoData> geoData = LoadGeoData(geoParser, layer, opts.Input);
                IEnumerable<Hexagon> results = HexagonProcessor.ProcessHexagonsFromGeoData(geoData, hexagonDefinition, layer.Targets);

                ExportResults(exporter, results, hexagonDefinition, mergeStrategy);
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Process took {stopwatch.ElapsedMilliseconds} ms");
        }

        private static void ExportResults(IResultExporter exporter, IEnumerable<Hexagon> results, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy)
        {
            exporter.ExportResults(results, hexagonDefinition, mergeStrategy).Wait();
        }

        private static IEnumerable<GeoData> LoadGeoData(IGeoDataParser geoParser, LayersConfiguration sourceData, string basePath)
        {
            var geoData = geoParser.ParseGeodataFromSource(sourceData, Path.Combine(basePath, sourceData.Source));

            foreach (var geo in geoData)
            {
                for (var index = 0; index < geo.Points.Length; index++)
                    geo.Points[index] = geo
                        .Points[index]
                        .Select(p => GeoUtils.CoordinateToPixel(p.X, p.Y))
                        .ToArray();
                yield return geo;
            }
        }

    }
}