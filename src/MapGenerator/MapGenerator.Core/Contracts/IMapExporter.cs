using MapGenerator.Core.Common;
using System.Collections.Generic;

namespace MapGenerator.Core.Contracts
{
    public interface IMapExporter<T>
    {
        T GenerateMap(List<object> hexagons, HexagonDefinition hexagonDefinition, int hexagonReferenceZoom);
    }
}