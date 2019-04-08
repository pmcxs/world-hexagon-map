using System;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Core.Utils
{
    public static class GeoUtils
    {
        public static PointXY CoordinateToPixel(double lon, double lat)
        {
            const int levelOfDetail = 10;

            var sinLatitude = Math.Sin(lat * Math.PI / 180);
            var pixelX = (lon + 180) / 360 * 256 * (2 << (levelOfDetail - 1));
            var pixelY = (0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI)) * 256 *
                         (2 << (levelOfDetail - 1));

            return new PointXY((int) Math.Truncate(0.5 + pixelX), (int) Math.Truncate(0.5 + pixelY));
        }
    }
}