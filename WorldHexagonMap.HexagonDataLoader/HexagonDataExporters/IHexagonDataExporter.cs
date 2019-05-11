using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonDataExporters
{
    public interface IHexagonDataExporter : IDisposable
    {
        Task<bool> ExportResults(IEnumerable<Hexagon> hexagons, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy);
    }
}