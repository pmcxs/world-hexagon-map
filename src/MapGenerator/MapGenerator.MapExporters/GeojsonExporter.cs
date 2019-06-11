using GeoAPI.Geometries;
using MapGenerator.Core.Common;
using MapGenerator.Core.Contracts;
using MapGenerator.Core.Utils;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Utils;

namespace MapGenerator.MapExporters
{
    public class GeojsonExporter : IMapExporter<string>
    {
        public string GenerateMap(List<object> hexagons, HexagonDefinition hexagonDefinition, int hexagonReferenceZoom)
        {
            GeoJsonWriter writer = new GeoJsonWriter();

            FeatureCollection hexagonCollection = new FeatureCollection();

            foreach (IDictionary<string, object> hexagon in hexagons)
            {

                HexagonLocationUV locationUV = new HexagonLocationUV(Convert.ToInt32(hexagon["U"]), Convert.ToInt32(hexagon["V"]));

                IList<PointXY> points = HexagonUtils.GetPointsXYOfHexagon(locationUV, hexagonDefinition);

                LinearRing ring = new LinearRing(points.Select(pixelCoordinate =>
                {
                    var (latitude, longitude) = TileSystem.PixelXYToLatLong((int)pixelCoordinate.X, (int)pixelCoordinate.Y, hexagonReferenceZoom);

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

            return result;


        }

    }
}
