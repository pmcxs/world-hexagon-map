using System;
using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Utils;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class HexagonProcessor
    {
        private readonly HexagonDefinition hexagonDefinition;
        private readonly ValueHandlerFactory valueHandlerFactory;

        public HexagonProcessor(HexagonDefinition hexagonDefinition, ValueHandlerFactory valueHandlerFactory = null)
        {
            this.hexagonDefinition = hexagonDefinition;
            this.valueHandlerFactory = valueHandlerFactory;
        }

        public IEnumerable<Hexagon> ProcessHexagonsFromGeoData(IEnumerable<GeoData> geoData, LayersLoaderTarget[] targets, BoundingBox boundingBox)
        {
            foreach (var geo in geoData)
            {
                bool intersectsBoundingBox = false;

                foreach (PointXY[] coordinates in geo.Points)
                {
                    foreach (PointXY point in coordinates)
                    {
                        //Check if point is outside the specified bounds
                        if(boundingBox == null || (point.X > boundingBox.West && point.X < boundingBox.West && point.Y > boundingBox.South && point.Y < boundingBox.North))
                        {
                            intersectsBoundingBox = true;
                        }

                        (point.X, point.Y) = TileSystem.LatLongToPixelXY(point.Y, point.X, hexagonDefinition.ReferenceZoom);
                    }
                }
                
                if(intersectsBoundingBox)
                {
                    switch (geo.DataType)
                    {
                        case DataType.Area:
                            {
                                foreach (Hexagon hexagon in ProcessArea(geo, targets))
                                {
                                    yield return hexagon;
                                }

                                break;
                            }
                        case DataType.Path:
                            {
                                foreach (Hexagon hexagon in ProcessPath(geo, targets))
                                {
                                    yield return hexagon;
                                }

                                break;
                            }
                        case DataType.Pixel:
                            {
                                foreach (Hexagon hexagon in ProcessPixel(geo, targets))
                                {
                                    yield return hexagon;
                                }

                                break;
                            }
                    }
                }
                
            }
        }

        private IEnumerable<Hexagon> ProcessPixel(GeoData geoData, LayersLoaderTarget[] targets)
        {
            foreach (PointXY[] coordinates in geoData.Points)
            {

                if (coordinates.Length == 1)
                //Single point mode 
                {
                    var hexagonLocation = HexagonUtils.GetHexagonLocationUVForPointXY(coordinates[0], hexagonDefinition);

                    yield return CreateHexagon(geoData, targets, hexagonLocation, (geo, target) => target.Handler == null ? 1 : valueHandlerFactory.GetInstance(target.Handler).GetValue(geo));
                }
                else
                //Square mode
                {
                    IList<HexagonLocationUV> hexagons = HexagonUtils.GetHexagonsInsideBoundingBox(coordinates[0], coordinates[1], hexagonDefinition).ToList();

                    foreach (var hexagonLocation in hexagons)
                    {
                        yield return CreateHexagon(geoData, targets, hexagonLocation, (geo, target) => target.Handler == null ? 1 : valueHandlerFactory.GetInstance(target.Handler).GetValue(geo));
                    }

                }


            }
        }

        public IEnumerable<Hexagon> ProcessPath(GeoData geoData, LayersLoaderTarget[] targets = null)
        {
            foreach (var coordinates in geoData.Points)
            for (var i = 0; i < coordinates.Length - 1; i++)
            {
                var currentPoint = coordinates[i];
                var nextPoint = coordinates[i + 1];

                //for each segment check intersection with the edges from the hexagons

                var hexagonLocations = PathHelper.GetHexagonsIntersectedByLine(currentPoint, nextPoint, hexagonDefinition);

                foreach (var hexagonLocation in hexagonLocations)
                {
                    var hexagonPoints = HexagonUtils.GetPointsXYOfHexagon(hexagonLocation, hexagonDefinition);

                    for (var j = 0; j < hexagonPoints.Count - 1; j++)
                    {
                        var hexagonCurrentPoint = hexagonPoints[j];
                        var hexagonNextPoint = hexagonPoints[j + 1];

                        if (PathHelper.GetLineIntersection(currentPoint, nextPoint, hexagonCurrentPoint,
                                hexagonNextPoint) != null)
                        {
                            yield return CreateHexagon(geoData, targets, hexagonLocation, (geo, target) => (int)Math.Pow(2, j));
                        }   
                    }
                }
            }
        }


        public IEnumerable<Hexagon> ProcessArea(GeoData geoData, LayersLoaderTarget[] targets = null)
        {
            foreach (PointXY[] geoCoordinates in geoData.Points)
            {
                var topLeftCoordinate = new PointXY(geoCoordinates.Min(c => c.X), geoCoordinates.Min(c => c.Y));
                var bottomRightCoordinate = new PointXY(geoCoordinates.Max(c => c.X), geoCoordinates.Max(c => c.Y));

                var hexagonLocations = HexagonUtils.GetHexagonsInsideBoundingBox(topLeftCoordinate, bottomRightCoordinate, hexagonDefinition);

                foreach (var hexagonLocation in hexagonLocations)
                {
                    var center = HexagonUtils.GetCenterPointXYOfHexagonLocationUV(hexagonLocation, hexagonDefinition);

                    if (AreaHelper.IsPointInsidePolygon(center.X, center.Y, geoCoordinates))
                    {
                        yield return CreateHexagon(geoData, targets, hexagonLocation,(geo, target) => geo.Values[target.SourceField]);
                    }
                }
            }
        }

        private delegate object TargetHandler(GeoData geoData, LayersLoaderTarget target);

        private static Hexagon CreateHexagon(GeoData geoData, LayersLoaderTarget[] targets, HexagonLocationUV hexagonLocation, TargetHandler TargetValueStrategy)
        {
            Hexagon hexagon = new Hexagon { LocationUV = hexagonLocation, HexagonData = new HexagonData() };

            if (targets != null)
            {
                foreach (LayersLoaderTarget target in targets)
                {
                    hexagon.HexagonData[target.Destination] = TargetValueStrategy(geoData, target);

                }
            }

            return hexagon;
        }

    }
}