﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.Console.Configuration;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;
using WorldHexagonMap.HexagonDataLoader.ResultExporters;
using WorldHexagonMap.HexagonDataLoader.ResultPostProcessors;


namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    public sealed class HexagonDataLoaderService : IHexagonDataLoaderService
    {
        private readonly ILoaderConfiguration _configuration;
        private readonly IGeoDataParserFactory _geoDataParserFactory;
        private readonly IHexagonProcessorFactory _hexagonParserFactory;

        private readonly ILogger<HexagonDataLoaderService> _logger;
        private readonly IPostProcessorFactory _postProcessorFactory;
        private readonly IResultExporterFactory _resultExporterFactory;
        private readonly IValueHandlerFactory _valueHandlerFactory;

        public HexagonDataLoaderService(ILoaderConfiguration configuration, ILoggerFactory loggerFactory,
            IHexagonProcessorFactory hexagonParserFactory, IGeoDataParserFactory geoDataParserFactory,
            IPostProcessorFactory postProcessorFactory, IValueHandlerFactory valueHandlerFactory,
            IResultExporterFactory resultExportFactory)
        {
            _configuration = configuration;
            _hexagonParserFactory = hexagonParserFactory;
            _geoDataParserFactory = geoDataParserFactory;
            _postProcessorFactory = postProcessorFactory;
            _valueHandlerFactory = valueHandlerFactory;
            _resultExporterFactory = resultExportFactory;

            _logger = loggerFactory.CreateLogger<HexagonDataLoaderService>();
        }


        public async Task<bool> Process(string path, string exportHandler)
        {
            var layers = XmlUtils.DeserializeFromFile<layers>(path);

            var basePath = Directory.GetParent(path).ToString();

            return await Process(layers, basePath, exportHandler);
        }

        private async Task<bool> Process(layers layers, string basePath, string exportHandler)
        {
            var clock = new Stopwatch();

            clock.Start();

            //1. Prepare global result-set
            IHexagonRepository globalResult = new MemoryHexagonRepository();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = _configuration.Parallelism
            };

            _logger.LogInformation(DateTime.Now + ": Loading Data");

            //2. Iterate the various source data
            Parallel.ForEach(layers.sources, options, sourceData =>
            {
                //3. Load GeoData
                var geoData = LoadGeoData(sourceData, basePath);

                //4. Convert the GeoData to hexagons
                var result = ConvertGeoDataToHexagons(geoData, sourceData.targets);

                //5. Merge Results into global result-set
                MergeResults(result, globalResult);
            });

            IEnumerable<Hexagon> results;

            _logger.LogInformation(DateTime.Now + ": Post-Processing");
            //6. Post-Process results
            if (layers.postprocessors != null && layers.postprocessors.Any())
                results = PostProcessResults(globalResult, layers.postprocessors);
            else
                results = globalResult.GetHexagons();

            //7. Export Results
            await ExportResultsAsync(results.ToArray(), exportHandler);

            _logger.LogInformation(DateTime.Now + ": Process Finished in " + clock.Elapsed.TotalSeconds);

            return true;
        }

        private IEnumerable<Hexagon> PostProcessResults(IHexagonRepository results,
            IEnumerable<layersPostprocessor> postProcessors)
        {
            var hexagons = results.GetHexagons();

            foreach (var postProcessor in postProcessors)
            {
                var handler = _postProcessorFactory.GetInstance(postProcessor.handler);

                for (var i = 0; i < postProcessor.iterations; i++)
                    foreach (var hexagon in hexagons)
                    {
                        var u = hexagon.LocationUV.U;
                        var v = hexagon.LocationUV.V;

                        var surroundingHexagons = new[]
                        {
                            AddNeighbour(u - 1, v, results),
                            AddNeighbour(u, v - 1, results),
                            AddNeighbour(u + 1, v - 1, results),
                            AddNeighbour(u + 1, v, results),
                            AddNeighbour(u, v + 1, results),
                            AddNeighbour(u - 1, v + 1, results)
                        };

                        handler.ProcessHexagon(hexagon, surroundingHexagons);
                    }
            }

            return hexagons;
        }

        private Hexagon AddNeighbour(int u, int v, IHexagonRepository results)
        {
            var hexagon = new Hexagon {LocationUV = new HexagonLocationUV(u, v)};
            hexagon.HexagonData = results.GetHexagonData(hexagon.LocationUV);
            return hexagon;
        }

        private IEnumerable<GeoData> LoadGeoData(layersLoader sourceData, string basePath)
        {
            var geoLoader = _geoDataParserFactory.GetInstance(sourceData.source);

            var geoData = geoLoader.ParseGeodataFromSource(sourceData, Path.Combine(basePath, sourceData.source));

            foreach (var geo in geoData)
            {
                geo.DataType = GetDataType(sourceData.type);

                for (var index = 0; index < geo.Points.Length; index++)
                    geo.Points[index] = geo
                        .Points[index]
                        .Select(p => GeoUtils.CoordinateToPixel(p.X, p.Y))
                        .ToArray();
                yield return geo;
            }
        }

        private IEnumerable<HexagonProcessorResult> ConvertGeoDataToHexagons(IEnumerable<GeoData> geoData,
            layersLoaderTarget[] targets)
        {
            foreach (var geo in geoData)
            {
                var parser = _hexagonParserFactory.GetInstance(geo.DataType);

                foreach (var target in targets)
                {
                    var handler = target.handler != null
                        ? _valueHandlerFactory.GetInstance(target.handler)
                        : null;

                    foreach (var result in parser.ProcessGeoData(geo, handler))
                    {
                        result.Target = target.field;
                        result.MergeStrategy = GetMergeStrategy(target.merge);
                        yield return result;
                    }
                }
            }
        }


        private async Task<bool> ExportResultsAsync(IEnumerable<Hexagon> hexagons, string exportHandler)
        {
            _logger.LogInformation(DateTime.Now + ": Exporting Results with " + exportHandler);
            var exporter = _resultExporterFactory.GetInstance(exportHandler);
            return await exporter.ExportResults(hexagons);
        }

        private static void MergeResults(IEnumerable<HexagonProcessorResult> results, IHexagonRepository globalResult)
        {
            foreach (var result in results)
            {
                if (!globalResult.TryGetValue(result.HexagonLocationUV, out var data))
                {
                    data = new HexagonData();
                    globalResult[result.HexagonLocationUV] = data;
                }

                switch (result.MergeStrategy)
                {
                    case MergeStrategy.Mask:
                        data[result.Target] = (int) (data[result.Target] ?? 0) | (int) result.Value;
                        break;
                    case MergeStrategy.Replace:
                        data[result.Target] = result.Value;
                        break;
                    case MergeStrategy.Max:

                        switch (result.Value)
                        {
                            case int valInt:
                                data[result.Target] = Math.Max((int) (data[result.Target] ?? 0), valInt);
                                break;
                            case double valDouble:
                                data[result.Target] =
                                    Math.Max((double) (data[result.Target] ?? 0.0), valDouble);
                                break;
                            default:
                                throw new FormatException("value " + result.Value + " can't be merged");
                        }

                        break;
                    case MergeStrategy.Min:
                        data[result.Target] = Math.Min((int) (data[result.Target] ?? 0), (int) result.Value);
                        break;
                }
            }
        }

        private DataType GetDataType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "area":
                    return DataType.Area;
                case "way":
                    return DataType.Way;
                case "pixel":
                    return DataType.Pixel;
                default:
                    throw new NotSupportedException("Data Type " + dataType + " not supported");
            }
        }

        private MergeStrategy GetMergeStrategy(string mergeStrategy)
        {
            switch (mergeStrategy.ToLower())
            {
                case "replace":
                    return MergeStrategy.Replace;
                case "mask":
                    return MergeStrategy.Mask;
                case "max":
                    return MergeStrategy.Max;
                case "min":
                    return MergeStrategy.Min;
                default:
                    throw new NotSupportedException("Merge Strategy " + mergeStrategy + " not supported");
            }
        }
    }
}