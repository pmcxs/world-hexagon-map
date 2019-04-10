using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class AreaProcessor : IHexagonProcessor
    {
        private readonly HexagonDefinition _hexagonDefinition;
        private readonly IHexagonService _hexagonService;

        public AreaProcessor(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IEnumerable<HexagonProcessorResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null)
        {
            foreach (var coordinates in geoData.Points)
            {
                var topLeftCoordinate = new PointXY(coordinates.Min(c => c.X), coordinates.Min(c => c.Y));
                var bottomRightCoordinate = new PointXY(coordinates.Max(c => c.X), coordinates.Max(c => c.Y));

                var polygonHexagons =
                    _hexagonService.GetHexagonsInsideBoundingBox(topLeftCoordinate, bottomRightCoordinate,
                        _hexagonDefinition);

                foreach (var hexagonLocation in polygonHexagons)
                {
                    var center =
                        _hexagonService.GetCenterPointXYOfHexagonLocationUV(hexagonLocation, _hexagonDefinition);

                    if (IsPointInsidePolygon(new Coordinate(center.X, center.Y), coordinates))
                        yield return new HexagonProcessorResult
                        {
                            HexagonLocationUV = hexagonLocation,
                            Value = valueHandler == null ? 1 : valueHandler.GetValue(geoData)
                        };
                }
            }
        }

        private bool IsPointInsidePolygon(Coordinate target, PointXY[] points)
        {
            var x = target.X;
            var y = target.Y;
            var inside = false;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                double xi = points[i].X, yi = points[i].Y;
                double xj = points[j].X, yj = points[j].Y;
                var intersect = yi > y != yj > y && x < (xj - xi) * (y - yi) / (yj - yi) + xi;
                if (intersect) inside = !inside;
            }

            return inside;
        }

        private class Coordinate
        {
            public Coordinate(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }
        }
    }
}