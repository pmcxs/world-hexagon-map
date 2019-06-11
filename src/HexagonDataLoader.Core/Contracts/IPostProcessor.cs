using HexagonDataLoader.Core.Common;

namespace HexagonDataLoader.Core.Contracts
{
    public interface IPostProcessor
    {
        void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons);
    }
}