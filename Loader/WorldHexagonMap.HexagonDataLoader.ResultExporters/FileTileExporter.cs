using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using Newtonsoft.Json;
using System;
using WorldHexagonMap.Loader.Domain.Configuration;
using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.ResultExporters
{
    /// <summary>
    /// Base classs for exporting a tile file
    /// </summary>
    public abstract class FileTileExporter : IResultExporter
    {
        private readonly IHexagonService _hexagonService;
        private readonly HexagonDefinition _hexagonDefinition;

        protected FileTileExporter(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public async Task<bool> ExportResults(IEnumerable<Hexagon> hexagons)
        {
            ITileExporterConfiguration configuration = GetExportConfiguration();

            //1. Create structure to hold the tiles info
            var targetTiles = new Dictionary<string, VectorTile>();

            IEqualityComparer<Hexagon> comparer = new Hexagon();

            foreach (Hexagon hexagon in hexagons)
            {
                //2. find tiles to which this hexagon belongs to
                IEnumerable<TileInfo> relatedTiles = _hexagonService.GetTilesContainingHexagon(hexagon, configuration.MinZoom, configuration.MaxZoom, _hexagonDefinition, configuration.TileSize);

                foreach (TileInfo tileInfo in relatedTiles)
                {
                    //3. Check if those tiles already belong to the master collection
                    string key = tileInfo.Z + "_" + tileInfo.X + "_" + tileInfo.Y;

                    VectorTile tile;

                    if (targetTiles.ContainsKey(key))
                    {
                        tile = targetTiles[key];
                    }
                    else
                    {
                        double edgeSize = _hexagonDefinition.EdgeSize / Math.Pow(2, 10 - tileInfo.Z);
                        tile = new VectorTile { TileInfo = tileInfo, Hexagons = new HashSet<Hexagon>(comparer), HexagonDefinition = new HexagonDefinition(edgeSize) };

                        targetTiles.Add(key, tile);
                    }

                    //4. Add the hexagon to the hexagon tile
                    tile.AddHexagon(hexagon);
                }
            }

            //5. Export tiles according to the configuration
            return await ExportTilesAsync(targetTiles, configuration.TileFormat);
        }

        protected async Task<bool>  ExportTilesAsync(Dictionary<string, VectorTile> targetTiles, TileFormat tileFormat)
        {
            int total = targetTiles.Count;
            int count = 0;

            await PrepareContainer();

            IEqualityComparer<Hexagon> comparer = new Hexagon();

            int previousPercentage = 0;

            foreach (VectorTile vectorTile in targetTiles.Values)
            {
                
                int currentPercentage = count++ * 100 / total;

                if (currentPercentage > previousPercentage)
                {
                    previousPercentage = currentPercentage;

                    if (previousPercentage % 5 == 0)
                    {
                        Console.Write("..{0}%", previousPercentage);
                    }
                }


                VectorTile existingVectorTile = await FindVectorTileInContainer(vectorTile.TileInfo);

                if (existingVectorTile != null)
                {
                    //Merge data on the hexagons that are shared by the current tile and the one currently stored on the container
                    IEnumerable<Hexagon> intersection = vectorTile.Hexagons.Intersect(existingVectorTile.Hexagons,
                        comparer);

                    Dictionary<string, Hexagon> existingHexagons =
                        existingVectorTile.Hexagons.ToDictionary(k => k.LocationUV.U + "_" + k.LocationUV.V, k => k);
                    foreach (Hexagon hexagon in intersection)
                    {
                        var hexagonKey = hexagon.GetHexagonKey();

                        if (existingHexagons.ContainsKey(hexagonKey))
                        {
                            var existingHexagon = existingHexagons[hexagonKey];
                            hexagon.MergeFrom(existingHexagon);
                        }
                    }

                    //Union to add the hexagons that only existed on the currently stored tile
                    vectorTile.Hexagons =
                        new HashSet<Hexagon>(vectorTile.Hexagons.Union(existingVectorTile.Hexagons, comparer));
                }

                await ExportTileToContainer(vectorTile);
            }

            return true;
        }

        protected string GetJson(VectorTile hexagonTile)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(hexagonTile, jsonSettings);
        }

        protected Stream GetBinaryStream(VectorTile hexagonTile)
        {
            Stream stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, hexagonTile);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        }

        protected abstract ITileExporterConfiguration GetExportConfiguration();

        protected abstract Task<bool> PrepareContainer();

        protected abstract Task<VectorTile> FindVectorTileInContainer(TileInfo tileInfo);

        protected abstract Task<bool> ExportTileToContainer(VectorTile vectorTile);


    }

}
