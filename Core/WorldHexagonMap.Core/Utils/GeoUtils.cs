using System;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Core.Utils
{
    
    
    public class BoundingBox {
        public double North;
        public double South;
        public double East;
        public double West;   
    }
    
    public static class GeoUtils
    {
//        public static PointXY CoordinateToPixel(double lon, double lat)
//        {
//            const int levelOfDetail = 10;
//
//            var sinLatitude = Math.Sin(lat * Math.PI / 180);
//            var pixelX = (lon + 180) / 360 * 256 * (2 << (levelOfDetail - 1));
//            var pixelY = (0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI)) * 256 *
//                         (2 << (levelOfDetail - 1));
//
//            return new PointXY((int) Math.Truncate(0.5 + pixelX), (int) Math.Truncate(0.5 + pixelY));
//        }
        
        public static BoundingBox Tile2BoundingBox(int x, int y, int zoom) 
        {
            BoundingBox bb = new BoundingBox
            {
                North = Tile2Lat(y, zoom),
                South = Tile2Lat(y + 1, zoom),
                West = Tile2Lon(x, zoom),
                East = Tile2Lon(x + 1, zoom)
            };
            return bb;
        }

        private static double Tile2Lon(int x, int z) {
            return x / Math.Pow(2.0, z) * 360.0 - 180;
        }

        private static double Tile2Lat(int y, int z) {
            var n=Math.PI-2*Math.PI*y/Math.Pow(2,z);
            return (180/Math.PI*Math.Atan(0.5*(Math.Exp(n)-Math.Exp(-n))));
        }
        
        
        
        
        
    }
}