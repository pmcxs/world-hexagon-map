using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Core.Services
{
    public interface IHexagonService
    {

        HexagonLocationUV GetHexagonLocationUVForPointXY(PointXY point, HexagonDefinition hexagonDefinition);

        IList<PointXY> GetPointsXYOfHexagon(HexagonLocationUV location, HexagonDefinition hexagonDefinition);

        PointXY GetCenterPointXYOfHexagonLocationUV(HexagonLocationUV location, HexagonDefinition hexagonDefinition);

        int GetDistanceBetweenHexagonLocationUVs(HexagonLocationUV source, HexagonLocationUV destination, HexagonDefinition hexagonDefinition);

        IList<HexagonLocationUV> GetHexagonsInsideBoudingBox(PointXY topLeftCorner, PointXY bottomRightCorner, HexagonDefinition hexagonDefinition);

        IEnumerable<TileInfo> GetTilesContainingHexagon(Hexagon hexagon, int minZoomLevel, int maxZoomLevel, HexagonDefinition hexagonDefinition, int tileSize);

        bool IsPointInsideHexagon(PointXY point, Hexagon hexagon, HexagonDefinition hexagonDefinition);

        void GetNeighbours(HexagonDefinition hexagonDefinition);

    }
}
