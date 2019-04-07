using System;
using System.Collections.Generic;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Core
{
    /// <summary>
    /// Contains all logic related with hexagon calculations
    /// </summary>
    public class HexagonService : IHexagonService
    {
        /// <summary>
        /// Obtains the Hexagon Location (u,v) for a given point (x,y)
        /// </summary>
        /// <remarks>
        /// This algorithm follows a two step approach. The first one is very first and although doesn't have false positives might have false negatices.
        /// Thus, on a second iteration (if the first didn't return true) it checks the vicinity hexagons and uses a more deterministic polygon intersection mechanism.
        /// </remarks>
        /// <param name="point">A given X,Y point</param>
        /// <param name="hexagonDefinition"></param>
        /// aaa
        /// <returns>An hexagon U,V Location</returns>
        public HexagonLocationUV GetHexagonLocationUVForPointXY(PointXY point, HexagonDefinition hexagonDefinition)
        {
            var u = Convert.ToInt32(Math.Round(point.X / hexagonDefinition.NarrowWidth));
            var v = Convert.ToInt32(Math.Round(point.Y / hexagonDefinition.Height - u * 0.5));

            if (IsPointXYInsideHexagonLocationUV(point, new HexagonLocationUV { U = u, V = v }, hexagonDefinition)) 
            {
                return new HexagonLocationUV { U= u, V= v };
            }
            
            var surroundingHexagons = new List<HexagonLocationUV>
            {
                new HexagonLocationUV { U = u, V = v - 1},
                new HexagonLocationUV { U = u, V = v + 1},
                new HexagonLocationUV { U = u - 1, V = v },
                new HexagonLocationUV { U = u + 1, V = v },
                new HexagonLocationUV { U = u - 1, V = v + 1},
                new HexagonLocationUV { U = u + 1, V = v - 1},
            };

            for(var i=0; i< surroundingHexagons.Count; i++) 
            {
                if (IsPointXYInsideHexagonLocationUV(point, surroundingHexagons[i], hexagonDefinition))
                {
                    return surroundingHexagons[i];
                }
            }

            return null;


        }

        /// <summary>
        /// Determines if a specified point (x,y) is inside a given hexagon Location (u,v)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="location"></param>
        /// <param name="hexagonDefinition"></param>
        /// <returns>True if inside the hexagon, false otherwise</returns>
        public bool IsPointXYInsideHexagonLocationUV(PointXY point, HexagonLocationUV location, HexagonDefinition hexagonDefinition)
        {
            var center = GetCenterPointXYOfHexagonLocationUV(location, hexagonDefinition);

            var d = hexagonDefinition.Diameter;
            var dx = Math.Abs(point.X - center.X) / d;
            var dy = Math.Abs(point.Y - center.Y) / d;
            var a = 0.25 * Math.Sqrt(3.0);
            return (dy <= a) && (a * dx + 0.25 * dy <= 0.5 * a);
        }

        public IList<PointXY> GetPointsXYOfHexagon(HexagonLocationUV location, HexagonDefinition hexagonDefinition)
        {
            var center = GetCenterPointXYOfHexagonLocationUV(location, hexagonDefinition);

            return new List<PointXY>
            {
                new PointXY(center.X - hexagonDefinition.Diameter/2.0, center.Y),
                new PointXY(center.X - hexagonDefinition.EdgeSize/2.0, center.Y - hexagonDefinition.Height/2.0),
                new PointXY(center.X + hexagonDefinition.EdgeSize/2.0, center.Y - hexagonDefinition.Height/2.0),
                new PointXY(center.X + hexagonDefinition.Diameter/2.0, center.Y),
                new PointXY(center.X + hexagonDefinition.EdgeSize/2.0, center.Y + hexagonDefinition.Height/2.0),
                new PointXY(center.X - hexagonDefinition.EdgeSize/2.0, center.Y + hexagonDefinition.Height/2.0),
                new PointXY(center.X - hexagonDefinition.Diameter/2.0, center.Y)

            };
        }

        /// <summary>
        /// Obtains the center point (x,y) of an hexagon given its LocationUv (u,v)
        /// </summary>
        /// <param name="location"></param>
        /// <param name="hexagonDefinition"></param>
        /// <returns></returns>
        public PointXY GetCenterPointXYOfHexagonLocationUV(HexagonLocationUV location, HexagonDefinition hexagonDefinition)
        {
            var x = hexagonDefinition.NarrowWidth * location.U;
            var y = hexagonDefinition.Height * (location.U * 0.5 + location.V);

            return new PointXY(x, y);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="hexagonDefinition"></param>
        /// <returns></returns>
        public int GetDistanceBetweenHexagonLocationUVs(HexagonLocationUV source, HexagonLocationUV destination, HexagonDefinition hexagonDefinition)
        {
            int du = destination.U - source.U;
            int dv =  destination.V - source.V;

            if( du * dv > 0) 
            {
                return Math.Abs(du + dv);
            }

            return Math.Max(Math.Abs(du), Math.Abs(dv));
          
        }

        public IList<HexagonLocationUV> GetHexagonsInsideBoudingBox(PointXY topLeftCorner, PointXY bottomRightCorner, HexagonDefinition hexagonDefinition)
        {

            var topLeftUV = GetHexagonLocationUVForPointXY(topLeftCorner, hexagonDefinition);
            var bottomLeftUV = GetHexagonLocationUVForPointXY(new PointXY(topLeftCorner.X, bottomRightCorner.Y), hexagonDefinition);
            var bottomRightUV = GetHexagonLocationUVForPointXY(bottomRightCorner, hexagonDefinition);
            
            var rowsEven = bottomLeftUV.V - topLeftUV.V + 1;

            var topLeftUVCenter = GetCenterPointXYOfHexagonLocationUV(topLeftUV, hexagonDefinition);
            var minX2 = topLeftUVCenter.X + hexagonDefinition.NarrowWidth;

            var topLeftUV2 = GetHexagonLocationUVForPointXY(new PointXY(minX2, topLeftCorner.Y), hexagonDefinition);
            var bottomLeftUV2 = GetHexagonLocationUVForPointXY(new PointXY(minX2, bottomRightCorner.Y), hexagonDefinition);
            var rowsOdd = bottomLeftUV2.V - topLeftUV2.V + 1;

            var columns = bottomRightUV.U - topLeftUV.U + 1;

            var hexagons = new List<HexagonLocationUV>();

            for (var i = 0; i < columns; i++) {

                var rows = (i%2) == 0 ? rowsEven : rowsOdd;

                for (var j = 0 ; j < rows ; j++) {

                    int offset = topLeftUV.V == topLeftUV2.V ? Convert.ToInt32(Math.Floor((double)i/2)) :  Convert.ToInt32(Math.Ceiling((double)i/2));

                    int u = topLeftUV.U + i;
                    int v = topLeftUV.V + j - offset;
                    hexagons.Add(new HexagonLocationUV(u, v));
                }
            }

            return hexagons;

        }


        public IEnumerable<TileInfo> GetTilesContainingHexagon(Hexagon hexagon, int minZoomLevel, int maxZoomLevel, HexagonDefinition hexagonDefinition, int tileSize)
        {
            var tiles = new HashSet<TileInfo>();

            IList<PointXY> points = GetPointsXYOfHexagon(hexagon.LocationUV, hexagonDefinition);

            var topLeft = new PointXY(points[0].X, points[1].Y);
            var topRight = new PointXY(points[3].X, points[1].Y);

            var bottomLeft = new PointXY(points[0].X, points[4].Y);
            var bottomRight = new PointXY(points[3].X, points[4].Y);

            for (int zoomLevel = minZoomLevel; zoomLevel <= maxZoomLevel; zoomLevel++)
            {
                tiles.Add(GetTileOfPoint(topLeft, zoomLevel, tileSize));
                tiles.Add(GetTileOfPoint(topRight, zoomLevel, tileSize));
                tiles.Add(GetTileOfPoint(bottomLeft, zoomLevel, tileSize));
                tiles.Add(GetTileOfPoint(bottomRight, zoomLevel, tileSize));
            }

            return tiles;

        }


        public bool IsPointInsideHexagon(PointXY point, Hexagon hexagon, HexagonDefinition hexagonDefinition)
        {
            return false;
        }

        public void GetNeighbours(HexagonDefinition hexagonDefinition)
        {

        }


        private TileInfo GetTileOfPoint(PointXY pointXY, int zoomLevel, int tileSize)
        {
            double pixelFactor = Math.Pow(2, zoomLevel - 10);

            int tileX = Convert.ToInt32(Math.Floor(pointXY.X * pixelFactor / tileSize));
            int tileY = Convert.ToInt32(Math.Floor(pointXY.Y * pixelFactor / tileSize));

            return new TileInfo(zoomLevel, tileX, tileY);

        }
    }
}
