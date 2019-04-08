using System;
using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class PathProcessor : IHexagonProcessor
    {
        private readonly HexagonDefinition _hexagonDefinition;
        private readonly IHexagonService _hexagonService;

        public PathProcessor(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IEnumerable<HexagonProcessorResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null)
        {
            foreach (var coordinates in geoData.Points)
                for (var i = 0; i < coordinates.Length - 1; i++)
                {
                    var currentPoint = coordinates[i];
                    var nextPoint = coordinates[i + 1];

                    //for each segment check intersection with the edges from the hexagons

                    var hexagons = GetHexagonsIntersectedByLine(currentPoint, nextPoint);

                    foreach (var hexagonLocation in hexagons)
                    {
                        var hexagonPoints = _hexagonService.GetPointsXYOfHexagon(hexagonLocation, _hexagonDefinition);

                        for (var j = 0; j < hexagonPoints.Count - 1; j++)
                        {
                            var hexagonCurrentPoint = hexagonPoints[j];
                            var hexagonNextPoint = hexagonPoints[j + 1];

                            if (GetLineIntersection(currentPoint, nextPoint, hexagonCurrentPoint, hexagonNextPoint) !=
                                null)
                                yield return new HexagonProcessorResult
                                {
                                    HexagonLocationUV = hexagonLocation,
                                    Value = (int) Math.Pow(2, j)
                                };
                        }
                    }
                }
        }

        private IEnumerable<HexagonLocationUV> GetHexagonsIntersectedByLine(PointXY startPoint, PointXY endPoint)
        {
            var startHexagon = _hexagonService.GetHexagonLocationUVForPointXY(startPoint, _hexagonDefinition);
            var endHexagon = _hexagonService.GetHexagonLocationUVForPointXY(endPoint, _hexagonDefinition);

            var minU = new[] {startHexagon.U, endHexagon.U}.Min() - 1;
            var maxU = new[] {startHexagon.U, endHexagon.U}.Max() + 1;

            var minV = new[] {startHexagon.V, endHexagon.V}.Min() - 1;
            var maxV = new[] {startHexagon.V, endHexagon.V}.Max() + 1;

            var matchingHexagons = new List<HexagonLocationUV>();

            for (var u = minU; u <= maxU; u++)
            for (var v = minV; v <= maxV; v++)
            {
                var points = _hexagonService.GetPointsXYOfHexagon(new HexagonLocationUV(u, v), _hexagonDefinition);

                for (var k = 0; k < points.Count() - 1; k++)
                {
                    var p0 = points[k];
                    var p1 = points[k + 1];

                    if (GetLineIntersection(p0, p1, startPoint, endPoint) != null)
                    {
                        matchingHexagons.Add(new HexagonLocationUV(u, v));
                        break;
                    }
                }
            }

            return matchingHexagons;
        }


        private PointXY GetLineIntersection(PointXY p0, PointXY p1, PointXY p2, PointXY p3)
        {
            var s1X = p1.X - p0.X;
            var s1Y = p1.Y - p0.Y;
            var s2X = p3.X - p2.X;
            var s2Y = p3.Y - p2.Y;
            var s = (-s1Y * (p0.X - p2.X) + s1X * (p0.Y - p2.Y)) / (-s2X * s1Y + s1X * s2Y);
            var t = (s2X * (p0.Y - p2.Y) - s2Y * (p0.X - p2.X)) / (-s2X * s1Y + s1X * s2Y);
            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                var intX = p0.X + t * s1X;
                var intY = p0.Y + t * s1Y;
                return new PointXY(intX, intY);
            }

            return null;
        }
    }
}