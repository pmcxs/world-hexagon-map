using System;
using System.Collections.Generic;
using System.Diagnostics;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.HexagonDataLoader.Cmd.ValueHandlers;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonDataExporters;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors;
using WorldHexagonMap.HexagonDataLoader.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.Cmd.GeoDataToHexagonData
{
    internal static class GeoDataToHexagonDataAction
    {
        internal static void Process(GeoDataToHexagonDataOptions opts)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            LayersConfiguration layer = opts.GetLayersConfiguration();
            MergeStrategy mergeStrategy = opts.Merge;

            ValueHandlerFactory valueHandlerFactory = new ValueHandlerFactory();
            valueHandlerFactory.RegisterImplementation<WikimediaAltitudeHandler>("wikimedia_altitude");

            HexagonDefinition hexagonDefinition = opts.GetHexagonDefinition();
            HexagonProcessor hexagonProcessor = new HexagonProcessor(hexagonDefinition, valueHandlerFactory);

            using (IGeoDataParser geoParser = opts.GetGeoDataParser())
            using (IHexagonDataExporter exporter = opts.GetResultExporter())
            {
                IEnumerable<GeoData> geoDataList = geoParser.ParseGeodataFromSource(layer);
                IEnumerable<Hexagon> results = hexagonProcessor.ProcessHexagonsFromGeoData(geoDataList, layer.Targets);

                ExportResults(exporter, results, hexagonDefinition, mergeStrategy);
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Process took {stopwatch.ElapsedMilliseconds} ms");
        }
        
        private static void ExportResults(IHexagonDataExporter exporter, IEnumerable<Hexagon> results, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy)
        {
            exporter.ExportResults(results, hexagonDefinition, mergeStrategy).Wait();
        }
    
    }
    
    
}