using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers;
using WorldHexagonMap.Loader.Domain;
using WorldHexagonMap.Loader.Domain.Configuration;
using WorldHexagonMap.Loader.Domain.Enums;
using WorldHexagonMap.Loader.HexagonRepositories;
using WorldHexagonMap.Loader.PostProcessors;
using WorldHexagonMap.Loader.ResultExporters;
using WorldHexagonMap.Loader.Service;

namespace WorldHexagonMap.Loader
{
    public class HexagonDataLoaderService : IHexagonDataLoaderService
    {
        private readonly ILoaderConfiguration _configuration;
        private readonly IHexagonProcessorFactory _hexagonParserFactory;
        private readonly IGeoDataParserFactory _geoDataParserFactory;
        private readonly IPostProcessorFactory _postProcessorFactory;
        private readonly IValueHandlerFactory _valueHandlerFactory;
        private readonly IResultExporterFactory _resultExporterFactory;

        private readonly ILogger<HexagonDataLoaderService> _logger;

        public HexagonDataLoaderService(ILoaderConfiguration configuration, ILoggerFactory loggerFactory, IHexagonProcessorFactory hexagonParserFactory, IGeoDataParserFactory geoDataParserFactory, IPostProcessorFactory postProcessorFactory, IValueHandlerFactory valueHandlerFactory, IResultExporterFactory resultExportFactory)
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

            string basePath = Directory.GetParent(path).ToString();

            return await Process(layers, basePath, exportHandler);
        }

        public async Task<bool> Process(layers layers, string basePath, string exportHandler)
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
                IEnumerable<GeoData> geoData = LoadGeoData(sourceData, basePath);

                //4. Convert the GeoData to hexagons
                IEnumerable<HexagonLoaderResult> result = ConvertGeoDataToHexagons(geoData, sourceData.targets);

                //5. Merge Results into global result-set
                MergeResults(result, globalResult);
                
            });
            
            IEnumerable<Hexagon> results;

            _logger.LogInformation(DateTime.Now + ": Post-Processing");
            //6. Post-Process results
            if (layers.postprocessors != null && layers.postprocessors.Any())
            {
                results = PostProcessResults(globalResult, layers.postprocessors);
            }
            else
            {
                results = globalResult.GetHexagons();
            }

            //7. Export Results
            await ExportResultsAsync(results.ToArray(), exportHandler);

            _logger.LogInformation(DateTime.Now + ": Process Finished in " + clock.Elapsed.TotalSeconds);

            return true;
        }

        private IEnumerable<Hexagon> PostProcessResults(IHexagonRepository results, IEnumerable<layersPostprocessor> postProcessors)
        {
            Hexagon[] hexagons = results.GetHexagons();

            foreach (var postProcessor in postProcessors)
            {
                IPostProcessor handler = _postProcessorFactory.GetInstance(postProcessor.handler);

                for (var i = 0; i < postProcessor.iterations; i++)
                {
                    foreach (Hexagon hexagon in hexagons)
                    {
                        int u = hexagon.LocationUV.U;
                        int v = hexagon.LocationUV.V;

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
            }

            return hexagons;
        }

        private Hexagon AddNeighbour(int u, int v, IHexagonRepository results)
        {
            var hexagon = new Hexagon { LocationUV = new HexagonLocationUV(u, v) };
            hexagon.HexagonData = results.GetHexagonData(hexagon.LocationUV);
            return hexagon;
        }

        protected virtual IEnumerable<GeoData> LoadGeoData(layersLoader sourceData, string basePath)
        {
            IGeoDataParser geoLoader = _geoDataParserFactory.GetInstance(sourceData.source);

            IEnumerable<GeoData> geoData = geoLoader.ParseGeodataFromSource(sourceData, Path.Combine(basePath, sourceData.source));
   
            foreach (GeoData geo in geoData)
            {
                geo.DataType = GetDataType(sourceData.type);

                for (int index = 0; index < geo.Points.Length; index++)
                {
                    geo.Points[index] = geo
                            .Points[index]
                            .Select(p => GeoUtils.CoordinateToPixel(p.X, p.Y))
                            .ToArray();
                }
                yield return geo;
            }
        }

        protected virtual IEnumerable<HexagonLoaderResult> ConvertGeoDataToHexagons(IEnumerable<GeoData> geoData, layersLoaderTarget[] targets)
        {
            foreach (GeoData geo in geoData)
            {
                IHexagonProcessor parser = _hexagonParserFactory.GetInstance(geo.DataType);

                foreach (layersLoaderTarget target in targets)
                {
                    var handler = target.handler != null
                        ? _valueHandlerFactory.GetInstance(target.handler)
                        : null;

                    foreach (HexagonLoaderResult result in parser.ProcessGeoData(geo, handler))
                    {
                        result.Target = target.field;
                        result.MergeStrategy = GetMergeStrategy(target.merge);
                        yield return result;
                    }
                }
            }
        }

        

        protected virtual async Task<bool> ExportResultsAsync(Hexagon[] hexagons, string exportHandler)
        {

            _logger.LogInformation(DateTime.Now + ": Exporting Results with " + exportHandler);
            IResultExporter exporter =  _resultExporterFactory.GetInstance(exportHandler);
            return await exporter.ExportResults(hexagons);

        }

        protected virtual void MergeResults(IEnumerable<HexagonLoaderResult> results, IHexagonRepository globalResult)
        {
            foreach (HexagonLoaderResult result in results)
            {
                HexagonData data;

                if (!globalResult.TryGetValue(result.HexagonLocationUV, out data))
                {
                    data = new HexagonData();
                    globalResult[result.HexagonLocationUV] = data;
                }

                switch (result.MergeStrategy)
                {
                    case MergeStrategy.Mask:
                        data[result.Target] = (int)(data[result.Target] ?? 0) | (int)result.Value;
                        break;
                    case MergeStrategy.Replace:
                        data[result.Target] = result.Value;
                        break;
                    case MergeStrategy.Max:

                        if (result.Value is int)
                        {
                            data[result.Target] = Math.Max((int) (data[result.Target] ?? 0), (int) result.Value);
                        }
                        else if(result.Value is double)
                        {
                            data[result.Target] = Math.Max((double) (data[result.Target] ?? 0.0), (double) result.Value);
                        }
                        else
                        {
                            throw new FormatException("value " + result.Value + " can't be merged");
                        }
                        break;
                    case MergeStrategy.Min:
                        data[result.Target] = Math.Min((int)(data[result.Target] ?? 0), (int)result.Value);
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
                    throw new NotSupportedException("Data Type " + dataType + " not supposrted");
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
