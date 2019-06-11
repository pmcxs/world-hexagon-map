using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Common.Enums;
using HexagonDataLoader.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HexagonDataLoader.HexagonDataExporters
{
    public class ConsoleExporter : IHexagonDataExporter
    {
        public Task<bool> ExportResults(IEnumerable<Hexagon> hexagons, HexagonDefinition hexagonDefinition, MergeStrategy mergeStrategy)
        {
            foreach (var hexagon in hexagons)
            {
                foreach (var key in hexagon.HexagonData.Data.Keys)
                {
                    Console.WriteLine($"({hexagon.LocationUV.U}:{hexagon.LocationUV.U}) {key}:{hexagon.HexagonData[key]}");
                }

            }
            return Task.FromResult(true);
        }

        public void Dispose()
        {
        }
    }
}