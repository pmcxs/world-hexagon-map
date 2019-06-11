using MapGenerator.Core.Common;
using WorldHexagonMap.Core.Utils;
using Xunit;

namespace MapGenerator.Core.Test.Utils
{
    public class TileSystemTest
    {
        [Theory]
        [InlineData(12, 1941, 1569, -9.404297, 38.754083, -9.316406, 38.68551)]
        [InlineData(6, 30, 24, -11.25, 40.979898, -5.625, 36.597889)]
        public void GetBoundsFromTileTest(int zoom, int x, int y, double west, double north, double east, double south)
        {
            var (pixelXLeft, pixelYTop) = TileSystem.TileXYToPixelXY(x, y);
            var (n, w) = TileSystem.PixelXYToLatLong(pixelXLeft, pixelYTop, zoom);

            var (pixelXRight, pixelYBottom) = TileSystem.TileXYToPixelXY(x + 1, y + 1);
            var (s, e) = TileSystem.PixelXYToLatLong(pixelXRight, pixelYBottom, zoom);

            Assert.Equal(west, w, 6);
            Assert.Equal(north, n, 6);
            Assert.Equal(east, e, 6);
            Assert.Equal(south, s, 6);

        }

        [Theory]
        [InlineData(-87.65, 41.85, 33624, 48730, 9)]
        [InlineData(-87.65, 41.85, 67247, 97459, 10)]
        [InlineData(-87.65, 41.85, 134494, 194918, 11)]
        [InlineData(-6, 41, 126703, 98285, 10)]
        [InlineData(5, 30, 134713, 108154, 10)]
        public void LatLonToPixelXYTest(double lon, double lat, int pixelX, int pixelY, int zoom)
        {

            PointXY pixel = new PointXY();

            (pixel.X, pixel.Y) = TileSystem.LatLongToPixelXY(lat, lon, zoom);

            Assert.Equal(pixelX, pixel.X);
            Assert.Equal(pixelY, pixel.Y);

        }


        [Theory]
        [InlineData(33624, 48730, 9, -87.65, 41.85)]
        public void PixelXYToLatLonTest(int pixelX, int pixelY, int zoom, double lon, double lat)
        {
            var (latitude, longitude) = TileSystem.PixelXYToLatLong(pixelX, pixelY, zoom);

            Assert.Equal(lat, latitude, 2);
            Assert.Equal(lon, longitude, 2);

        }


    }
}