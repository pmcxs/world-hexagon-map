using MapGenerator.Core.Common;

namespace MapGenerator.Core.Contracts
{
    public interface IPostProcessor
    {
        void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons);
    }
}