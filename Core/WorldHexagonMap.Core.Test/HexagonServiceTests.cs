using System;
using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using Xunit;

namespace WorldHexagonMap.Core.Test
{
    public class HexagonServiceTests
    {
        private readonly HexagonService _service = new HexagonService();
        private readonly HexagonDefinition _hexagonDefinition = new HexagonDefinition(100);

        /// <summary>
        ///                      50
        ///                    |---|
        ///                    *********
        ///                   * U:1,V:-1*
        ///              50  *           *
        ///            |---|* (150,-86.7)  *
        ///        *********       +       *
        ///       * U:0,V:0 *             *
        ///      *           *           *
        ///     *    (0,0)    *         *
        ///    *       +       *********
        ///     *             * U:1,V:0 *
        ///      *           *           *
        ///       *         *  (150,86.7) *
        ///        *********       +       *
        ///                 *             *
        ///                  *           *
        ///                   *         *
        ///                    *********
        /// 
        ///  Edge size: 100
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [Theory]
        [InlineData(0,0,0,0)]
        [InlineData(1,-1,150,-86.6)]
        [InlineData(1,0,150,86.6)]
        public void GetCenterPointXYOfHexagon(int u, int v, double x, double y)
        {
            var pointXY = _service.GetCenterPointXYOfHexagonLocationUV(new HexagonLocationUV(u, v), _hexagonDefinition);

            Assert.Equal(x, Math.Round(pointXY.X,2));
            Assert.Equal(y, Math.Round(pointXY.Y,2));

        }

        /// <summary>
        ///      (1)             (2)
        ///        ***************
        ///       *               *
        ///      *                 *
        ///     *        (0,0)      *
        /// (0)*          +          *(3)
        ///     *                   *
        ///      *                 *
        ///       *               *
        ///        ***************
        ///      (5)              (4)
        ///  (0)  -100, 0
        ///  (1)   -50, -86.60254037844388
        ///  (2)    50, -86.60254037844388
        ///  (3)   100, 0
        ///  (4)    50, 86.60254037844388
        ///  (5)   -50, 86.60254037844388
        /// </summary>
        [Fact]
        public void GetPointsXYOfHexagon()
        {
            IList<PointXY> points = _service.GetPointsXYOfHexagon(new HexagonLocationUV(0, 0), _hexagonDefinition);

            Assert.Equal(-100, points[0].X);
            Assert.Equal(0, points[0].Y);

            Assert.Equal(-50, points[1].X);
            Assert.Equal(-86.6, Math.Round(points[1].Y,2));

            Assert.Equal(50, points[2].X);
            Assert.Equal(-86.6, Math.Round(points[2].Y,2));

            Assert.Equal(100, points[3].X);
            Assert.Equal(0, points[3].Y);

            Assert.Equal(50, points[4].X);
            Assert.Equal(86.6, Math.Round(points[4].Y,2));

            Assert.Equal(-50, points[5].X);
            Assert.Equal(86.6, Math.Round(points[5].Y,2));

        }

        /// <summary>
        ///                     50
        ///                   |---|
        ///                   *********
        ///                  * U:1,V:-1*
        ///                 * 150       *
        ///           |-----------|      *
        ///       *********       C       *
        ///      * U:0,V:0 *             *
        ///     *           *           *
        ///    *             *         *
        ///   *       A   | B *********
        ///    *             * U:1,V:0 *
        ///     *           *           *
        ///      *         *             *
        ///       ********* F |   D      G*
        ///                *             *
        ///                 *           *
        ///                  *    E    *
        ///                   *********
        /// Edge size: 100
        /// Height: +- 173
        /// Points:
        /// A - (0,0)
        /// B - (90, 0)
        /// C - (150, -86.6)
        /// D - (150, 86.6)
        /// E - (150, 150)
        /// F - (90,  86.6)
        /// G - (249, 86.6)
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        [Theory]
        [InlineData(0,0,0,0)]
        [InlineData(90,0,0,0)]
        [InlineData(150,-86.6,1,-1)]
        [InlineData(150,86.6,1,0)]
        [InlineData(150,150,1,0)]
        [InlineData(90,86.6,1,0)]
        [InlineData(249,86.6,1,0)]
        public void GetHexagonLocationUVForPointXY(double x, double y, int u, int v)
        {
            var hexagonLocationUV = _service.GetHexagonLocationUVForPointXY(new PointXY(x, y), _hexagonDefinition);
            Assert.Equal(new HexagonLocationUV(u, v), hexagonLocationUV);
        }

        /// <summary>
        ///       *************
        ///      *             *
        ///     *               *
        ///    *                 *
        ///   *         A       B * C
        ///    *                 *
        ///     *               *
        ///      *             *
        ///       *************
        /// Edge size: 100
        /// Points:
        /// A - (0,0)
        /// B - (90, 0)
        /// C - (110,0)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="isInside"></param>
        [Theory]
        [InlineData(0,0,0,0,true)]
        [InlineData(90,0,0,0,true)]
        [InlineData(110,0,0,0,false)]
        public void IsPointXYInsideHexagonLocationUV(double x, double y, int u, int v, bool isInside)
        {
            var result = _service.IsPointXYInsideHexagonLocationUV(new PointXY(x, y), new HexagonLocationUV(u, v), _hexagonDefinition);
            Assert.Equal(isInside, result);

        }

        /// <summary>
        /// 
        ///                   +++++++++               +++++++++
        ///                  + U:1,V:-1+             + U:3,V:-2+
        ///                 +           +           +           +
        ///                +             +         +             +
        ///       +++++++++               +++++++++               +++++++++
        ///      + U:0,V:0 +             + U:2,V:-1+             + U:4,V:-2
        ///     +     x-----------X--------------x--------x     +           +
        ///    +      |B     +    |C   +         |   +    |    +             +
        ///   +       |       ++++|++++          |    ++++|++++               +
        ///    +      |      + U:1|V:0 +         |   + U:3|V:-1+             +
        ///     +     |-----------x==============x  +     |     +           +
        ///      +    |    +       1     +         +      |      +         +
        ///       ++++|++++               +++++++++       |       +++++++++
        ///      + U:0|V:1 +             + U:2,v:0 +     A|      + U:4,v:-1+
        ///     +     x-----------------------------------x     +           +
        ///    +             +         +             +         +             +
        ///   +               +++++++++               +++++++++               +
        /// </summary>
        [Fact]
        public void GetHexagonsInsideBoundingBox()
        {
            IList<HexagonLocationUV> resultsA = _service.GetHexagonsInsideBoudingBox(new PointXY(0, -20), new PointXY(450, 100), _hexagonDefinition);

            Assert.Equal(resultsA[0], new HexagonLocationUV(0, 0));
            Assert.Equal(resultsA[1], new HexagonLocationUV(0, 1));
            Assert.Equal(resultsA[2], new HexagonLocationUV(1, -1));
            Assert.Equal(resultsA[3], new HexagonLocationUV(1, 0));
            Assert.Equal(resultsA[4], new HexagonLocationUV(2, -1));
            Assert.Equal(resultsA[5], new HexagonLocationUV(2, 0));
            Assert.Equal(resultsA[6], new HexagonLocationUV(3, -2));
            Assert.Equal(resultsA[7], new HexagonLocationUV(3, -1));

            IList<HexagonLocationUV> resultsB = _service.GetHexagonsInsideBoudingBox(new PointXY(0, -20), new PointXY(320, 20), _hexagonDefinition);

            Assert.Equal(resultsB[0], new HexagonLocationUV(0, 0));
            Assert.Equal(resultsB[1], new HexagonLocationUV(1, -1));
            Assert.Equal(resultsB[2], new HexagonLocationUV(1, 0));
            Assert.Equal(resultsB[3], new HexagonLocationUV(2, -1));

            IList<HexagonLocationUV> resultsC = _service.GetHexagonsInsideBoudingBox(new PointXY(150, -20), new PointXY(320, 20), _hexagonDefinition);

            Assert.Equal(resultsC[0], new HexagonLocationUV(1, -1));
            Assert.Equal(resultsC[1], new HexagonLocationUV(1, 0));
            Assert.Equal(resultsC[2], new HexagonLocationUV(2, -1));

        }


        /// <summary>
        /// 
        ///                   +++++++++               +++++++++
        ///                  + U:1,V:-1+             + U:3,V:-2+
        ///                 +           +           +           +
        ///                +             +         +             +
        ///       +++++++++               +++++++++               +++++++++
        ///      + U:0,V:0 +             + U:2,V:-1+             + U:4,V:-2
        ///     +     x-----------X--------------x--------x     +           +
        ///    +      |B     +    |C   +         |   +    |    +             +
        ///   +       |       ++++|++++          |    ++++|++++               +
        ///    +      |      + U:1|V:0 +         |   + U:3|V:-1+             +
        ///     +     |-----------x==============x  +     |     +           +
        ///      +    |    +       1     +         +      |      +         +
        ///       ++++|++++               +++++++++       |       +++++++++
        ///      + U:0|V:1 +             + U:2,v:0 +     A|      + U:4,v:-1+
        ///     +     x-----------------------------------x     +           +
        ///    +             +         +             +         +             +
        ///   +               +++++++++               +++++++++               +
        /// </summary>
        [Fact]
        public void GetTilesContainingHexagons()
        {



        }
        
    }
}
