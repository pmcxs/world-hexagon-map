using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Domain.Enums;
using WorldHexagonMap.Loader.Contracts;

namespace WorldHexagonMap.Loader.Implementation.PostProcessors
{

    /// <summary>
    /// Fixes incoherences on roads
    /// </summary>
    public abstract class WayFixer : IPostProcessor
    {
        protected abstract string ProcessedHexagonDataType { get; }

        private WayMask GetNeighbourHexagonValue(Hexagon[] neighbourHexagons, int index)
        {
            return neighbourHexagons[index].HexagonData[ProcessedHexagonDataType] == null ? 0 : (WayMask)neighbourHexagons[index].HexagonData[ProcessedHexagonDataType];
        }

        public void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {
            if (hexagon.HexagonData[ProcessedHexagonDataType] == null || (int) hexagon.HexagonData[ProcessedHexagonDataType] == 0) return;

            RemoveZigZags(hexagon, neighbourHexagons);

            RemoveTriangles(hexagon, neighbourHexagons);

            RemoveDeadEnds(hexagon, neighbourHexagons);

            RemoveDoubleTriangles(hexagon, neighbourHexagons);

            ConnectRoads(hexagon, neighbourHexagons);
        }

        private void ConnectRoads(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.BottomRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom | WayMask.BottomRight;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom | WayMask.BottomLeft;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.TopRight | WayMask.BottomLeft;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Top)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.TopRight | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.TopLeft;
            }
            
        }

        private void RemoveDoubleTriangles(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopLeft | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.Top | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Top | WayMask.Bottom;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }


            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top |WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.Top | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.Top | WayMask.TopRight | WayMask.Bottom)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
            }
        }


        private void RemoveDeadEnds(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 0) == WayMask.BottomRight))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == WayMask.BottomLeft))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == WayMask.BottomLeft))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == WayMask.TopLeft))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == WayMask.TopLeft))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == WayMask.TopRight))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Top | WayMask.Bottom;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.BottomRight | WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == WayMask.Top))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }
            
        }

        private void RemoveTriangles(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {
            
            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.TopRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.Top | WayMask.TopRight | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.TopRight | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Top | WayMask.TopRight | WayMask.BottomRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopRight | WayMask.BottomRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.TopLeft | WayMask.TopRight | WayMask.Bottom | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopRight | WayMask.BottomRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.TopRight | WayMask.Bottom | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.BottomRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.Top | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.Top | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.BottomRight | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomRight | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Top | WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Top | WayMask.TopRight | WayMask.Bottom)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)) &&
              (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
              (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Top | WayMask.Bottom)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
              (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
              (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.Top | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.BottomLeft;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.TopRight | WayMask.BottomLeft )))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight |WayMask.BottomLeft;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.BottomLeft | WayMask.TopRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.BottomRight | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomLeft | WayMask.TopRight;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.BottomLeft | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight | WayMask.Bottom)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight | WayMask.Bottom | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.Top | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.Top | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight | WayMask.Bottom )))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.Bottom | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.BottomLeft;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
            }

            
        }

        private void RemoveZigZags(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.Top)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top)) &&
              (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
              (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.BottomLeft | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Top | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.BottomRight;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.TopRight)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.TopRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Top | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopRight | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.TopLeft | WayMask.TopRight | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom | WayMask.BottomLeft;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopRight | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.Top | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 0) == (WayMask.TopRight | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.TopLeft | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[0].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.BottomRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopLeft | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.TopRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopRight | WayMask.Bottom)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.Bottom | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.Top)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomLeft)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.BottomRight | WayMask.Bottom)) &&
               (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.Top)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.BottomLeft;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomLeft;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Top | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.BottomRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.TopLeft | WayMask.Top)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 5) == (WayMask.TopLeft | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.BottomLeft;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[5].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight;
            }


            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 1) == (WayMask.TopRight | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.TopLeft | WayMask.BottomLeft)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.Top | WayMask.Bottom;
                neighbourHexagons[1].HexagonData[ProcessedHexagonDataType] = WayMask.TopRight | WayMask.Bottom;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.TopRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Top | WayMask.TopRight | WayMask.BottomRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = null;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.TopRight | WayMask.BottomRight;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.BottomRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.BottomRight;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.BottomLeft | WayMask.BottomRight)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 2) == (WayMask.BottomRight | WayMask.Bottom)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.TopLeft | WayMask.Top)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomLeft | WayMask.TopRight;
                neighbourHexagons[2].HexagonData[ProcessedHexagonDataType] = WayMask.BottomRight | WayMask.BottomLeft;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = null;
            }

            if (((WayMask)hexagon.HexagonData[ProcessedHexagonDataType] == (WayMask.BottomRight | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 3) == (WayMask.Bottom | WayMask.BottomLeft)) &&
                (GetNeighbourHexagonValue(neighbourHexagons, 4) == (WayMask.Top | WayMask.TopRight)))
            {
                hexagon.HexagonData[ProcessedHexagonDataType] = WayMask.BottomLeft | WayMask.TopRight;
                neighbourHexagons[3].HexagonData[ProcessedHexagonDataType] = WayMask.TopLeft | WayMask.Bottom;
                neighbourHexagons[4].HexagonData[ProcessedHexagonDataType] = null;
            }
        }
    }
}
