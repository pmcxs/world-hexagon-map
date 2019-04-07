using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Loader.PostProcessors
{
    public interface IPostProcessor
    {
        void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons);
    }
}
