using System.Threading.Tasks;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp
{
    public interface IHexagonDataLoaderService
    {
        Task<bool> Process(string path, string exportHandler);
    }
}