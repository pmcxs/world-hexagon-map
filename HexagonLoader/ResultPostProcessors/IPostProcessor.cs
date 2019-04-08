using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ResultPostProcessors
{
    public interface IPostProcessor
    {
        void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons);
    }
}