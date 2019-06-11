using HexagonDataLoader.Core.Common;
using System.Collections.Generic;

namespace HexagonDataLoader.Core.Contracts
{
    public interface IMapExporter<T>
    {
        T GenerateMap(List<object> hexagons, HexagonDefinition hexagonDefinition, int hexagonReferenceZoom);
    }
}