using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors;
using WorldHexagonMap.HexagonDataLoader.ResultExporters;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{

    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ConsoleExporterOptions>(args)
                .WithParsed(RunOptionsAndReturnExitCode);

        }

        private static void RunOptionsAndReturnExitCode(ConsoleExporterOptions opts)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            LayersConfiguration layer = opts.GetLayersConfiguration();
            MergeStrategy mergeStrategy = opts.Merge;

            using (IGeoDataParser geoParser = opts.GetGeoDataParser())
            using (IResultExporter exporter = opts.GetResultExporter())
            {
                HexagonDefinition hexagonDefinition = opts.GetHexagonDefinition();

                IEnumerable<GeoData> geoDataList = geoParser.ParseGeodataFromSource(layer, Path.Combine(opts.Input, layer.Source));
 
//                foreach
//                
//                foreach (PointXY[] geoCoordinates in geoData.Points)
//                {
//                    //PointXY[] pixelCoordinates = geoCoordinates.Select(p => GeoUtils.CoordinateToPixel(p.X, p.Y)).ToArray();

                
                
                                
                IEnumerable<Hexagon> results = HexagonProcessor.ProcessHexagonsFromGeoData(geoDataList, hexagonDefinition, layer.Targets);

                ExportResults(exporter, results, hexagonDefinition, mergeStrategy);
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Process took {stopwatch.ElapsedMilliseconds} ms");
        }

        private static void ExportResults(IResultExporter exporter, IEnumerable<Hexagon> results, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy)
        {
            exporter.ExportResults(results, hexagonDefinition, mergeStrategy).Wait();
        }

    }
}