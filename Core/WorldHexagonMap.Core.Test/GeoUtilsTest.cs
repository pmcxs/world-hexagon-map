using System;
using WorldHexagonMap.Core.Utils;
using Xunit;

namespace WorldHexagonMap.Core.Test
{
    public class GeoUtilsTest
    {
        [Theory]
        [InlineData(12,1941,1569,-9.404297,38.754083,-9.316406,38.68550976)]
        [InlineData(6,30,24,-11.25,40.979898,-5.625,36.597889)]
        public void GetBoundsFromTileTest(int zoom, int x, int y, double west, double north, double east, double south)
        {
            BoundingBox boundingBox = GeoUtils.Tile2BoundingBox(x, y, zoom);
            Assert.True(Math.Abs(west - boundingBox.West) < 0.000001);
            Assert.True(Math.Abs(east - boundingBox.East) < 0.000001);
            Assert.True(Math.Abs(north - boundingBox.North) < 0.000001);
            Assert.True(Math.Abs(south - boundingBox.South) < 0.000001);
            
        }
    }
}