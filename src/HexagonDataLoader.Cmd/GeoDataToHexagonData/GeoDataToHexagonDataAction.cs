using HexagonDataLoader.Cmd.ValueHandlers;
using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Common.Enums;
using HexagonDataLoader.Core.Contracts;
using HexagonDataLoader.GeoDataParsers.HexagonProcessors;
using HexagonDataLoader.GeoDataParsers.ValueHandlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HexagonDataLoader.Cmd.GeoDataToHexagonData
{
    internal static class GeoDataToHexagonDataAction
    {
        internal static void Process(GeoDataToHexagonDataArgs opts)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            LayersConfiguration layer = opts.GetLayersConfiguration();
            MergeStrategy mergeStrategy = opts.Merge;

            ValueHandlerFactory valueHandlerFactory = new ValueHandlerFactory();
            valueHandlerFactory.RegisterImplementation<WikimediaAltitudeHandler>("wikimedia_altitude");

            HexagonDefinition hexagonDefinition = opts.GetHexagonDefinition();
            HexagonProcessor hexagonProcessor = new HexagonProcessor(hexagonDefinition, valueHandlerFactory);

            BoundingBox bb = null;

            if (!string.IsNullOrEmpty(opts.East) && !string.IsNullOrEmpty(opts.West) &&
                     !string.IsNullOrEmpty(opts.North) && !string.IsNullOrEmpty(opts.South))
            {
                bb = new BoundingBox
                {
                    East = double.Parse(opts.East),
                    West = double.Parse(opts.West),
                    North = double.Parse(opts.North),
                    South = double.Parse(opts.South)
                };
            }

            using (IGeoDataParser geoParser = opts.GetGeoDataParser())
            using (IHexagonDataExporter exporter = opts.GetResultExporter())
            {
                IEnumerable<GeoData> geoDataList = geoParser.ParseGeodataFromSource(layer);

                IEnumerable<Hexagon> results = hexagonProcessor.ProcessHexagonsFromGeoData(geoDataList, layer.Targets, bb);

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