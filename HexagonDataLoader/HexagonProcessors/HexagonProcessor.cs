using System;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public static class HexagonProcessor
    {
        public static IEnumerable<Hexagon> ProcessHexagonsFromGeoData(IEnumerable<GeoData> geoData, HexagonDefinition hexagonDefinition, LayersLoaderTarget[] targets)
        {
            foreach (var geo in geoData)
            {
                foreach (PointXY[] coordinates in geo.Points)
                {
                    foreach (PointXY point in coordinates)
                    {
                        (point.X, point.Y) = TileSystem.LatLongToPixelXY(point.X, point.Y, 10);
                    }
                }
                
                switch (geo.DataType)
                {
                    case DataType.Area:
                    {
                        foreach (Hexagon hexagon in ProcessArea(geo, hexagonDefinition, targets))
                        {
                            yield return hexagon;
                        }

                        break;
                    }
                    case DataType.Path:
                    {
                        foreach(Hexagon hexagon in ProcessPath(geo, hexagonDefinition, targets))
                        {
                            yield return hexagon;
                        }

                        break;
                    }
                }
            }
        }

        public static IEnumerable<Hexagon> ProcessPath(GeoData geoData, HexagonDefinition hexagonDefinition, LayersLoaderTarget[] targets = null)
        {
            foreach (var coordinates in geoData.Points)
            for (var i = 0; i < coordinates.Length - 1; i++)
            {
                var currentPoint = coordinates[i];
                var nextPoint = coordinates[i + 1];

                //for each segment check intersection with the edges from the hexagons

                var hexagons = PathHelper.GetHexagonsIntersectedByLine(currentPoint, nextPoint, hexagonDefinition);

                foreach (var hexagonLocation in hexagons)
                {
                    var hexagonPoints = HexagonService.GetPointsXYOfHexagon(hexagonLocation, hexagonDefinition);

                    for (var j = 0; j < hexagonPoints.Count - 1; j++)
                    {
                        var hexagonCurrentPoint = hexagonPoints[j];
                        var hexagonNextPoint = hexagonPoints[j + 1];

                        if (PathHelper.GetLineIntersection(currentPoint, nextPoint, hexagonCurrentPoint,
                                hexagonNextPoint) != null)
                        {
                            
                            Hexagon hexagon = new Hexagon {LocationUV = hexagonLocation, HexagonData = new HexagonData()};

                            //TODO: Merge with logic below
                            if (targets != null)
                            {
                                foreach (LayersLoaderTarget target in targets)
                                {
                                    hexagon.HexagonData[target.Destination] = (int) Math.Pow(2, j);
                                }
                            }

                            yield return hexagon;
                        }   
                    }
                }
            }
        }

        public static IEnumerable<Hexagon> ProcessArea(GeoData geoData, HexagonDefinition hexagonDefinition, LayersLoaderTarget[] targets = null)
        {
            foreach (PointXY[] geoCoordinates in geoData.Points)
            {
                //PointXY[] pixelCoordinates = geoCoordinates.Select(p => GeoUtils.CoordinateToPixel(p.X, p.Y)).ToArray();
                
                var topLeftCoordinate = new PointXY(geoCoordinates.Min(c => c.X), geoCoordinates.Min(c => c.Y));
                var bottomRightCoordinate = new PointXY(geoCoordinates.Max(c => c.X), geoCoordinates.Max(c => c.Y));

                var polygonHexagons = HexagonService.GetHexagonsInsideBoundingBox(topLeftCoordinate, bottomRightCoordinate, hexagonDefinition);

                foreach (var hexagonLocation in polygonHexagons)
                {
                    var center = HexagonService.GetCenterPointXYOfHexagonLocationUV(hexagonLocation, hexagonDefinition);

                    if (AreaHelper.IsPointInsidePolygon(center.X, center.Y, geoCoordinates))
                    {
                        Hexagon hexagon = new Hexagon {LocationUV = hexagonLocation, HexagonData = new HexagonData()};

                        //TODO: Merge with logic above
                        if (targets != null)
                        {
                            foreach (LayersLoaderTarget target in targets)
                            {
                                hexagon.HexagonData[target.Destination] = geoData.Values[target.SourceField];
                            }
                        }

                        yield return hexagon;
                    }
                }
            }
        }

        
    }
}