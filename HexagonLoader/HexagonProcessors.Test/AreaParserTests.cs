using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using Xunit;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.Test
{
    /// <summary>
    /// </summary>
    public class AreaParserTests
    {
        private readonly HexagonService _service = new HexagonService();
        private readonly HexagonDefinition _hexagonDefinition = new HexagonDefinition(100);

        /// <summary>
        /// Validates that a disjoint multipolygon is able
        /// to pickup isolated hexagons (as long as their
        /// centers are contained)
        ///
        /// Test Data:
        ///                   +++++++++
        ///                  + U:1,V:-1+
        ///                 +           +
        ///                +             +
        ///       +++++++++        *      +++++++++
        ///      + U:0,V:0 +             + U:2,V:-1+
        ///     +           +           +           +
        ///    +    *-----+  +         +  *-----+    +
        ///   +     |  1  |   +++++++++   |  2  |     +
        ///    +    |     |  + U:1,V:0 +  |     |    +
        ///     +   +-----+ +           + +-----+   +
        ///      +         +             +         +
        ///       +++++++++               +++++++++
        ///      + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///    +             +         +             +
        ///   +               +++++++++               +
        /// Edge size: 100
        /// 
        /// Polygon #1: (-10,20; 50,20; 50,-10; -10,-10; -10,20)
        /// Polygon #2: (250,20; 310,20; 310,-10; -250,-10; 250,20)
        /// 
        /// </summary>
        [Fact]
        public void ParseMultipolygon_Success()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[2][],
                DataType = DataType.Way
            };

            geoData.Points[0] = new[]
            {
                new PointXY(-10.0, 20.0),
                new PointXY(50.0, 20.0),
                new PointXY(50.0, -10.0),
                new PointXY(-10.0, -10.0),
                new PointXY(-10.0, 20.0)
            };

            geoData.Points[1] = new[]
            {
                new PointXY(250.0, 20.0),
                new PointXY(310.0, 20.0),
                new PointXY(310.0, -10.0),
                new PointXY(250.0, -10.0),
                new PointXY(250.0, 20.0)
            };

            var loader = new AreaProcessor(_service, _hexagonDefinition);

            //Act
            var result = loader.ProcessGeoData(geoData).ToArray();

            Assert.Equal(2, result.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), result[0].HexagonLocationUV);
            Assert.Equal(1, result[0].Value);

            Assert.Equal(new HexagonLocationUV(2, -1), result[1].HexagonLocationUV);
            Assert.Equal(1, result[1].Value);
        }

        /// <summary>
        /// Validate that parsing a polygon that goes
        /// through various hexagons but only considers
        /// the ones that have its center intersected.
        /// In this case (0,0) and (2,-1)
        /// 
        /// Test Data:
        ///                   +++++++++
        ///                  + U:1,V:-1+
        ///                 +           +
        ///                +             +
        ///       +++++++++        *      +++++++++
        ///      + U:0,V:0 +             + U:2,V:-1+
        ///     +           +           +           +
        ///    +    *--------+---------+--------+    +
        ///   +     | X       +++++++++        X|     +
        ///    +    |        + U:1,V:0 +        |    +
        ///     +   +-------+-----------+-------+   +
        ///      +         +             +         +
        ///       +++++++++               +++++++++
        ///      + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///    +             +         +             +
        ///   +               +++++++++               +
        /// Edge size: 100
        /// Polygon: (-10,20; 310,20; 310,-10; -10;-10; -10,20)
        /// </summary>
        [Fact]
        public void ParsePolygon_Success()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[1][],
                DataType = DataType.Way
            };

            geoData.Points[0] = new[]
            {
                new PointXY(-10.0, 20.0),
                new PointXY(310.0, 20.0),
                new PointXY(310.0, -10.0),
                new PointXY(-10.0, -10.0),
                new PointXY(-10.0, 20.0)
            };

            var loader = new AreaProcessor(_service, _hexagonDefinition);

            //Act
            var result = loader.ProcessGeoData(geoData).ToArray();

            Assert.Equal(2, result.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), result[0].HexagonLocationUV);
            Assert.Equal(1, result[0].Value);

            Assert.Equal(new HexagonLocationUV(2, -1), result[1].HexagonLocationUV);
            Assert.Equal(1, result[1].Value);
        }
    }
}