using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.PostProcessors
{
    public interface IPostProcessor
    {
        void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons);
    }
}