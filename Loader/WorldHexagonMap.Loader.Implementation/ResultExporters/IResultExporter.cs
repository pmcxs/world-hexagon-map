using System.Collections.Generic;
using System.Threading.Tasks;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.Loader.ResultExporters
{
    public interface IResultExporter
    {
        Task<bool> ExportResults(IEnumerable<Hexagon> hexagons);

    }
}
