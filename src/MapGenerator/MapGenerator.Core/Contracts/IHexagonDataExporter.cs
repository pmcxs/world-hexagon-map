using MapGenerator.Core.Common;
using MapGenerator.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapGenerator.Core.Contracts
{
    public interface IHexagonDataExporter : IDisposable
    {
        Task<bool> ExportResults(IEnumerable<Hexagon> hexagons, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy);
    }
}