using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ResultExporters
{
    public class ConsoleExporter : IResultExporter
    {
        public async Task<bool> ExportResults(IEnumerable<Hexagon> hexagons)
        {
            Console.WriteLine($"{hexagons.Count()} processed successfully");
            return true;
        }
    }
}