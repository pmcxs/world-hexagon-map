using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Domain.Enums;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.Domain;
using WorldHexagonMap.Loader.Domain.Enums;
using Xunit;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.Test
{
    /// <summary>
    ///     Validates that parsing Ways (Paths) into hexagons works properly
    /// </summary>
    public class WayParserTests
    {
        private readonly HexagonService _service = new HexagonService();
        private readonly HexagonDefinition _hexagonDefinition = new HexagonDefinition(100);

        /// <summary>
        ///     Test Data:
        ///     +++++++++
        ///     + U:1,V:-1+
        ///     +           +
        ///     +             +
        ///     +++++++++               +++++++++
        ///     + U:0,V:0 +             + U:2,V:-1+
        ///     +           +           +           +
        ///     +      *       +        +             +
        ///     +       |        +++++++++              +
        ///     +      |      + U:1,V:0 +             +
        ///     +     +-----------------------*     +
        ///     +         +       1     +         +
        ///     +++++++++               +++++++++
        ///     + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///     +             +         +             +
        ///     +               +++++++++               +
        ///     Edge size: 100
        ///     Line #1: (0,-10; 0,20; 300,20)
        /// </summary>
        [Fact]
        public void ParseFileWithLinestrings()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[1][],
                DataType = DataType.Way
            };

            geoData.Points[0] = new[]
            {
                new PointXY(0.0, -10.0),
                new PointXY(0.0, 20.0),
                new PointXY(300.0, 20.0)
            };


            var loader = new PathProcessor(_service, _hexagonDefinition);

            //Act
            var result = loader.ProcessGeoData(geoData).ToArray();

            //Verify
            Assert.Equal(4, result.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), result[0].HexagonLocationUV);
            Assert.Equal(WayMask.BottomRight, (WayMask) result[0].Value);

            Assert.Equal(new HexagonLocationUV(1, 0), result[1].HexagonLocationUV);
            Assert.Equal(WayMask.TopLeft, (WayMask) result[1].Value);

            Assert.Equal(new HexagonLocationUV(1, 0), result[2].HexagonLocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) result[2].Value);

            Assert.Equal(new HexagonLocationUV(2, -1), result[3].HexagonLocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) result[3].Value);
        }


        /// <summary>
        ///     +++++++++
        ///     + U:1,V:-1+
        ///     +           +
        ///     +             +
        ///     +++++++++       *       +++++++++
        ///     + U:0,V:0 +             + U:2,V:-1+
        ///     +     *-----------.     +           +
        ///     +      *-----------'    +              +
        ///     +               +++++++++               +
        ///     +             + U:1,V:0 +             +
        ///     +           +           +           +
        ///     +         +             +         +
        ///     +++++++++               +++++++++
        ///     + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///     +             +         +             +
        ///     +               +++++++++               +
        ///     Edge size: 100
        ///     Polygon: (-10,20; 310,20; 310,-10; -10;-10; -10,20)
        /// </summary>
        [Fact]
        public void ParseFileWithLinestrings_WithSameEdgeCrossingTwice()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[1][],
                DataType = DataType.Way
            };

            geoData.Points[0] = new[]
            {
                new PointXY(0.0, -50.0),
                new PointXY(150.0, -50.0),
                new PointXY(150.0, -10.0),
                new PointXY(0.0, -10.0)
            };

            var loader = new PathProcessor(_service, _hexagonDefinition);

            //Act
            var result = loader.ProcessGeoData(geoData).ToArray();

            //Verify
            Assert.Equal(4, result.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), result[0].HexagonLocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) result[0].Value);

            Assert.Equal(new HexagonLocationUV(1, -1), result[1].HexagonLocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) result[1].Value);

            Assert.Equal(new HexagonLocationUV(0, 0), result[2].HexagonLocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) result[2].Value);

            Assert.Equal(new HexagonLocationUV(1, -1), result[3].HexagonLocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) result[3].Value);
        }


        /// <summary>
        ///     Test Data:
        ///     +++++++++
        ///     + U:1,V:-1+
        ///     +           +
        ///     +             +
        ///     +++++++++        *      +++++++++
        ///     + U:0,V:0 +       |     + U:2,V:-1+
        ///     +           +      |    +           +
        ///     +      *       +    +----------+      +
        ///     +       |        +++++++++      |2      +
        ///     +      |      + U:1,V:0 +      |      +
        ///     +     +-----------------------*     +
        ///     +         +       1     +         +
        ///     +++++++++               +++++++++
        ///     + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///     +             +         +             +
        ///     +               +++++++++               +
        ///     Edge size: 100
        ///     Line #1: (0,-10; 0,20; 300,20)
        ///     Line #2: (300,20; 300,-10; 150,-10; 150;-90)
        /// </summary>
        [Fact]
        public void ParseMultipleFileWithLinestrings()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[2][],
                DataType = DataType.Way
            };

            geoData.Points[0] = new[]
            {
                new PointXY(0.0, -10.0),
                new PointXY(0.0, 20.0),
                new PointXY(300.0, 20.0)
            };

            geoData.Points[1] = new[]
            {
                new PointXY(300.0, 20.0),
                new PointXY(300.0, -10.0),
                new PointXY(150.0, -10.0),
                new PointXY(150.0, -90.0)
            };


            var loader = new PathProcessor(_service, _hexagonDefinition);

            var result = loader.ProcessGeoData(geoData).ToArray();

            Assert.Equal(6, result.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), result[0].HexagonLocationUV);
            Assert.Equal(WayMask.BottomRight, (WayMask) result[0].Value);

            Assert.Equal(new HexagonLocationUV(1, 0), result[1].HexagonLocationUV);
            Assert.Equal(WayMask.TopLeft, (WayMask) result[1].Value);

            Assert.Equal(new HexagonLocationUV(1, 0), result[2].HexagonLocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) result[2].Value);

            Assert.Equal(new HexagonLocationUV(2, -1), result[3].HexagonLocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) result[3].Value);

            Assert.Equal(new HexagonLocationUV(1, -1), result[4].HexagonLocationUV);
            Assert.Equal(WayMask.BottomRight, (WayMask) result[4].Value);

            Assert.Equal(new HexagonLocationUV(2, -1), result[5].HexagonLocationUV);
            Assert.Equal(WayMask.TopLeft, (WayMask) result[5].Value);
        }
    }
}