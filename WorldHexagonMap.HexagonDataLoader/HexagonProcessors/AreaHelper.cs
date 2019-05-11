using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public static class AreaHelper
    {
        public static bool IsPointInsidePolygon(double x, double y, PointXY[] points)
        {
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
    }
}