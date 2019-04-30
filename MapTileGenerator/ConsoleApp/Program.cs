using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.Core.Utils;

namespace WorldHexagonMap.MapTileGenerator.ConsoleApp
{

    internal class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<MapTileGeneratorOptions>(args)
                .WithParsed(RunOptionsAndReturnExitCode);

        }

        private static void RunOptionsAndReturnExitCode(MapTileGeneratorOptions opts)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            
            HexagonDefinition hexagonDefinition = opts.GetHexagonDefinition();
            
            //1. Get hexagons for a particular region
            
            //1.1. Get Bounding box coordinates

            BoundingBox bb;
            
            if(!string.IsNullOrEmpty(opts.Tile))
            {
                string[] tileSegments = opts.Tile.Split(":");

                if (tileSegments.Length != 3)
                {
                    throw new NotSupportedException($"A tile format must be supplied as z:x:y");
                }

                bb = GeoUtils.Tile2BoundingBox(
                    int.Parse(tileSegments[1]),
                    int.Parse(tileSegments[2]),
                    int.Parse(tileSegments[0]));
                
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
            else
            {
                throw new NotSupportedException("Neither proper coordinates nor tile was supplied");
            }
            
            (int pixelXTopLeft, int pixelYTopLeft) = TileSystem.LatLongToPixelXY(bb.North, bb.West, 10);
            (int pixelXBottomRight, int pixelYBottomRight) = TileSystem.LatLongToPixelXY(bb.South, bb.East, 10);

            var nw = new PointXY(pixelXTopLeft, pixelYTopLeft);
            var se = new PointXY(pixelXBottomRight, pixelYBottomRight);

            //2. (Get hexagons for them)

            List<HexagonLocationUV> hexagonLocations = HexagonService.GetHexagonsInsideBoundingBox(nw, se, hexagonDefinition).ToList();
            
            //3. Calculate coordinates for them

            GeoJsonWriter writer = new GeoJsonWriter();
            
            FeatureCollection hexagonCollection = new FeatureCollection();

            foreach (HexagonLocationUV locationUV in hexagonLocations)
            {
                IList<PointXY> points = HexagonService.GetPointsXYOfHexagon(locationUV, hexagonDefinition);
                
                LinearRing ring = new LinearRing(points.Select(pixelCoordinate =>
                {
                    var (latitude, longitude) = TileSystem.PixelXYToLatLong((int) pixelCoordinate.X, (int) pixelCoordinate.Y, 10);
                    return new Coordinate(longitude, latitude);
                    
                }).ToArray());

                Polygon hexagonPolygon = new Polygon(ring);

                Feature hexagonFeature = new Feature(hexagonPolygon, new AttributesTable(
                    new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("U", locationUV.U),
                        new KeyValuePair<string, object>("V", locationUV.V),
                    }));
                
                hexagonCollection.Add(hexagonFeature);
            }
            
            //4. Export Geojson just for the hexagons in this particular tile
            
            string result = writer.Write(hexagonCollection);

            File.WriteAllText(opts.Output, result);
            
            stopwatch.Stop();
            
            Console.WriteLine($"Process took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}