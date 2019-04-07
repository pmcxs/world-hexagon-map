using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IPostProcessor
    {
        void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons);
    }
}
