using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    public interface IHexagonRepository
    {
        HexagonData this[HexagonLocationUV hexagonLocationUV] { get; set; }
        bool TryGetValue(HexagonLocationUV hexagonLocationUV, out HexagonData data);
        Hexagon[] GetHexagons();
        HexagonData GetHexagonData(HexagonLocationUV locationUV);
    }
}