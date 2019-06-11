using HexagonDataLoader.Core.Common;
using HexagonDataLoader.HexagonDataImporters;
using HexagonDataLoader.MapExporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WorldHexagonMap.Core.Utils;

namespace HexagonDataLoader.Cmd.HexagonDataToMap
{
    internal static class HexagonDataToMapAction
    {
        internal static void Process(HexagonDataToMapArgs opts)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            HexagonDefinition hexagonDefinition = opts.GetHexagonDefinition();

            //1.1. Get Bounding box coordinates

            BoundingBox bb = null;

            if (!string.IsNullOrEmpty(opts.Tile))
            {
                string[] tileSegments = opts.Tile.Split(":");

                if (tileSegments.Length != 3)
                {
                    throw new NotSupportedException($"A tile format must be supplied as z:x:y");
                }

                int zoom = int.Parse(tileSegments[0]);
                int tileX = int.Parse(tileSegments[1]);
                int tileY = int.Parse(tileSegments[2]);

                var (pixelXLeft, pixelYTop) = TileSystem.TileXYToPixelXY(tileX, tileY);
                var (n, w) = TileSystem.PixelXYToLatLong(pixelXLeft, pixelYTop, opts.HexagonReferenceZoom);

                var (pixelXRight, pixelYBottom) = TileSystem.TileXYToPixelXY(tileX + 1, tileY + 1);
                var (s, e) = TileSystem.PixelXYToLatLong(pixelXRight, pixelYBottom, opts.HexagonReferenceZoom);

                bb = new BoundingBox
                {
                    East = e,
                    West = w,
                    North = n,
                    South = s
                };

            }
            else if (!string.IsNullOrEmpty(opts.East) && !string.IsNullOrEmpty(opts.West) &&
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

            List<object> hexagons;

            var hexagonDataReader = new SQLiteHexagonReader(opts.Input);

            if (bb == null)
            {
                hexagons = hexagonDataReader.Read(opts.DataProperties.ToArray()).ToList();
            }
            else
            {
                var (left, top) = TileSystem.LatLongToPixelXY(bb.North, bb.West, opts.HexagonReferenceZoom);
                var (right, bottom) = TileSystem.LatLongToPixelXY(bb.South, bb.East, opts.HexagonReferenceZoom);
                hexagons = hexagonDataReader.Read(left, right, top, bottom, opts.DataProperties.ToArray()).ToList();
            }

            using (StreamWriter outfile = new StreamWriter(opts.Output))
            {
                GeojsonExporter exporter = new GeojsonExporter();

                var result = exporter.GenerateMap(hexagons, hexagonDefinition, opts.HexagonReferenceZoom);
                outfile.Write(result);
            }


            stopwatch.Stop();

            Console.WriteLine($"Process took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}