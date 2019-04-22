using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public class ConsoleExporter : IResultExporter
    {
        public Task<bool> ExportResults(IEnumerable<Hexagon> hexagons, HexagonDefinition hexagonDefinition,  MergeStrategy mergeStrategy)
        {
            Console.WriteLine($"{hexagons.Count()} processed successfully");
            return Task.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}