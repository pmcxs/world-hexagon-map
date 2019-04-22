using System.Threading.Tasks;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    public interface IHexagonDataLoaderPipelineService
    {
        Task<bool> Process(string path, string exportHandler);

    }
}