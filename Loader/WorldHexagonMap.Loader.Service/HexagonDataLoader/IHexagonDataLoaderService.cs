using System.Threading.Tasks;

namespace WorldHexagonMap.Loader.Service
{
    public interface IHexagonDataLoaderService
    {
        Task<bool> Process(string path, string exportHandler);

    }
}
