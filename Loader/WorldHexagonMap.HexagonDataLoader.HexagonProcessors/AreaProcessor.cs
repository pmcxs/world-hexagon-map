using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers
{
    public class AreaProcessor : IHexagonProcessor
    {
        private readonly IHexagonService _hexagonService;
        private readonly HexagonDefinition _hexagonDefinition;

        public AreaProcessor(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IEnumerable<HexagonLoaderResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null)
        {
            foreach (var coordinates in geoData.Points)
            {
                var topLeftCoordinate = new PointXY(coordinates.Min(c => c.X), coordinates.Min(c => c.Y));
                var bottomRightCoordinate = new PointXY(coordinates.Max(c => c.X), coordinates.Max(c => c.Y));

                var polygonHexagons = _hexagonService.GetHexagonsInsideBoudingBox(topLeftCoordinate, bottomRightCoordinate, _hexagonDefinition);

                foreach (var hexagonLocation in polygonHexagons)
                {
                    PointXY center = _hexagonService.GetCenterPointXYOfHexagonLocationUV(hexagonLocation, _hexagonDefinition);

                    if (IsPointInsidePolygon(new Coordinate(center.X, center.Y), coordinates))
                    {
                        yield return new HexagonLoaderResult
                        {
                            HexagonLocationUV = hexagonLocation,
                            Value = valueHandler == null ? 1 : valueHandler.GetValue(geoData)
                        };

                    }
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
                bool intersect = yi > y != yj > y && x < (xj - xi) * (y - yi) / (yj - yi) + xi;
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

            public double X { get; internal set; }
            public double Y { get; internal set; }
        }
    }
}
