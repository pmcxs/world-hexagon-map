using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Domain.Enums;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using Xunit;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.Test
{
    /// <summary>
    ///     Validates that parsing Ways (Paths) into hexagons works properly
    /// </summary>
    public class WayParserTests
    {
        private readonly HexagonDefinition _hexagonDefinition = new HexagonDefinition(100);

        /// <summary>
        /// Test Data:
        ///                   +++++++++
        ///                  + U:1,V:-1+
        ///                 +           +
        ///                +             +
        ///       +++++++++               +++++++++
        ///      + U:0,V:0 +             + U:2,V:-1+
        ///     +           +           +           +
        ///    +      *       +        +             +
        ///   +       |        +++++++++              +
        ///    +      |      + U:1,V:0 +             +
        ///     +     +-----------------------*     +
        ///      +         +       1     +         +
        ///       +++++++++               +++++++++
        ///      + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///    +             +         +             +
        ///   +               +++++++++               +
        /// Edge size: 100
        /// Line #1: (0,-10; 0,20; 300,20)
        /// </summary>
        [Fact]
        public void ParseFileWithLinestrings()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[1][],
                DataType = DataType.Path
            };

            geoData.Points[0] = new[]
            {
                new PointXY(0.0, -10.0),
                new PointXY(0.0, 20.0),
                new PointXY(300.0, 20.0)
            };


            //Act
            Hexagon[] hexagons = HexagonProcessor.ProcessPath(geoData,_hexagonDefinition, 
                new[]
                {
                    new LayersLoaderTarget { Destination = "Road"}
                    
                }).ToArray();

            //Verify
            Assert.Equal(4, hexagons.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), hexagons[0].LocationUV);
            Assert.Equal(WayMask.BottomRight, (WayMask) hexagons[0].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, 0), hexagons[1].LocationUV);
            Assert.Equal(WayMask.TopLeft, (WayMask) hexagons[1].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, 0), hexagons[2].LocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) hexagons[2].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(2, -1), hexagons[3].LocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) hexagons[3].HexagonData["Road"]);
        }


        /// <summary>
        ///                   +++++++++
        ///                  + U:1,V:-1+
        ///                 +           +
        ///                +             +
        ///       +++++++++       *       +++++++++
        ///      + U:0,V:0 +             + U:2,V:-1+
        ///     +     *-----------.     +           +
        ///    +      *-----------'    +              +
        ///   +               +++++++++               +
        ///    +             + U:1,V:0 +             +
        ///     +           +           +           +
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
        public void ParseFileWithLinestrings_WithSameEdgeCrossingTwice()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[1][],
                DataType = DataType.Path
            };

            geoData.Points[0] = new[]
            {
                new PointXY(0.0, -50.0),
                new PointXY(150.0, -50.0),
                new PointXY(150.0, -10.0),
                new PointXY(0.0, -10.0)
            };


            //Act
            Hexagon[] hexagons = HexagonProcessor.ProcessPath(geoData,_hexagonDefinition, 
                new[]
                {
                    new LayersLoaderTarget { Destination = "Road"}
                    
                }).ToArray();

            //Verify
            Assert.Equal(4, hexagons.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), hexagons[0].LocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) hexagons[0].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, -1), hexagons[1].LocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) hexagons[1].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(0, 0), hexagons[2].LocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) hexagons[2].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, -1), hexagons[3].LocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) hexagons[3].HexagonData["Road"]);
        }


        /// <summary>
        /// Validates that processing two linestrings
        /// correctly intersects the various hexagons
        /// 
        /// Test Data:
        ///                   +++++++++
        ///                  + U:1,V:-1+
        ///                 +           +
        ///                +             +
        ///       +++++++++        *      +++++++++
        ///      + U:0,V:0 +       |     + U:2,V:-1+
        ///     +           +      |    +           +
        ///    +      *       +    +----------+      +
        ///   +       |        +++++++++      |2      +
        ///    +      |      + U:1,V:0 +      |      +
        ///     +     +-----------------------*     +
        ///      +         +       1     +         +
        ///       +++++++++               +++++++++
        ///      + U:0,V:1 +             + U:2,v:0 +
        ///     +           +           +           +
        ///    +             +         +             +
        ///   +               +++++++++               +
        /// Edge size: 100
        /// Line #1: (0,-10; 0,20; 300,20)
        /// Line #2: (300,20; 300,-10; 150,-10; 150;-90)
        /// </summary>
        [Fact]
        public void ParseMultipleFileWithLinestrings()
        {
            //Prepare
            var geoData = new GeoData
            {
                Points = new PointXY[2][],
                DataType = DataType.Path
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


            Hexagon[] hexagons = HexagonProcessor.ProcessPath(geoData,_hexagonDefinition, 
                new[]
                {
                    new LayersLoaderTarget { Destination = "Road"}
                    
                }).ToArray();

            Assert.Equal(6, hexagons.Length);

            Assert.Equal(new HexagonLocationUV(0, 0), hexagons[0].LocationUV);
            Assert.Equal(WayMask.BottomRight, (WayMask) hexagons[0].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, 0), hexagons[1].LocationUV);
            Assert.Equal(WayMask.TopLeft, (WayMask) hexagons[1].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, 0), hexagons[2].LocationUV);
            Assert.Equal(WayMask.TopRight, (WayMask) hexagons[2].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(2, -1), hexagons[3].LocationUV);
            Assert.Equal(WayMask.BottomLeft, (WayMask) hexagons[3].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(1, -1), hexagons[4].LocationUV);
            Assert.Equal(WayMask.BottomRight, (WayMask) hexagons[4].HexagonData["Road"]);

            Assert.Equal(new HexagonLocationUV(2, -1), hexagons[5].LocationUV);
            Assert.Equal(WayMask.TopLeft, (WayMask) hexagons[5].HexagonData["Road"]);
        }
    }
}