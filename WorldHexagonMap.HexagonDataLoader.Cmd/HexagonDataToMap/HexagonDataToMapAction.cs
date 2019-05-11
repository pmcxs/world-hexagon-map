using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.SQLite.HexagonDataImport;

namespace WorldHexagonMap.HexagonDataLoader.Cmd.HexagonDataToMap
{
    internal static class HexagonDataToMapAction
    {
        
        internal static void Process(HexagonDataToMapOptions opts)
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

            SQLiteHexagonReader reader = new SQLiteHexagonReader(opts.Input);

            List<object> hexagons;

            if (bb == null)
            {
                hexagons = reader.Read(opts.DataProperties.ToArray()).ToList();

            }
            else
            {
                var (left, top) = TileSystem.LatLongToPixelXY(bb.North, bb.West, opts.HexagonReferenceZoom);
                var (right, bottom) = TileSystem.LatLongToPixelXY(bb.South, bb.East, opts.HexagonReferenceZoom);
                hexagons = reader.Read(left, right, top, bottom, opts.DataProperties.ToArray()).ToList();
            }

            GeoJsonWriter writer = new GeoJsonWriter();

            FeatureCollection hexagonCollection = new FeatureCollection();

            foreach (IDictionary<string,object> hexagon in hexagons)
            {

                HexagonLocationUV locationUV = new HexagonLocationUV(Convert.ToInt32(hexagon["U"]), Convert.ToInt32(hexagon["V"]));
                             
                IList<PointXY> points = HexagonUtils.GetPointsXYOfHexagon(locationUV, hexagonDefinition);
                
                LinearRing ring = new LinearRing(points.Select(pixelCoordinate =>
                {
                    var (latitude, longitude) = TileSystem.PixelXYToLatLong((int)pixelCoordinate.X, (int)pixelCoordinate.Y, opts.HexagonReferenceZoom);

                    return new Coordinate(longitude, latitude);
                    
                }).ToArray());


                Polygon hexagonPolygon = new Polygon(ring);

                AttributesTable attributes = new AttributesTable(
                    new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>("U", locationUV.U),
                        new KeyValuePair<string, object>("V", locationUV.V),
                    });


                foreach (var key in hexagon.Keys)
                {
                    switch (key)
                    {
                        case "U":
                        case "V":
                            break;
                        default:
                            object value = hexagon[key];
                            attributes.Add(key, value);
                            break;
                    }
                    
                }

                Feature hexagonFeature = new Feature(hexagonPolygon, attributes);
                hexagonCollection.Add(hexagonFeature);
            }
            
            //4. Export Geojson just for the hexagons in this particular tile
            
            string result = writer.Write(hexagonCollection);

            using (StreamWriter outfile = new StreamWriter(opts.Output))
            {
                outfile.Write(result);
            }

            stopwatch.Stop();
            
            Console.WriteLine($"Process took {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}