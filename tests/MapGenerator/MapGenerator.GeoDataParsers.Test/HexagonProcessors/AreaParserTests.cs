using MapGenerator.Core.Common;
using MapGenerator.GeoDataParsers.HexagonProcessors;
using System.Linq;
using Xunit;

namespace MapGenerator.Test.HexagonProcessors
{
    /// <summary>
    /// </summary>
    public class AreaParserTests
    {
        private readonly HexagonProcessor _hexagonProcessor = new HexagonProcessor(new HexagonDefinition(100, 10));


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
                DataType = DataType.Path
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

            //Act
            Hexagon[] hexagons = _hexagonProcessor.ProcessArea(geoData).ToArray();

            Assert.Equal(2, hexagons.Length);
            Assert.Equal(new HexagonLocationUV(0, 0), hexagons[0].LocationUV);
            Assert.Equal(new HexagonLocationUV(2, -1), hexagons[1].LocationUV);
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
                DataType = DataType.Path
            };

            geoData.Points[0] = new[]
            {
                new PointXY(-10.0, 20.0),
                new PointXY(310.0, 20.0),
                new PointXY(310.0, -10.0),
                new PointXY(-10.0, -10.0),
                new PointXY(-10.0, 20.0)
            };

            //Act
            Hexagon[] hexagons = _hexagonProcessor.ProcessArea(geoData).ToArray();

            Assert.Equal(2, hexagons.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), hexagons[0].LocationUV);

            Assert.Equal(new HexagonLocationUV(2, -1), hexagons[1].LocationUV);
        }
    }
}