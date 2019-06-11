using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HexagonDataLoader.Core.Contracts
{
    public interface IHexagonDataExporter : IDisposable
    {
        Task<bool> ExportResults(IEnumerable<Hexagon> hexagons, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy);
    }
}