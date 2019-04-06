using System.Threading.Tasks;

namespace WorldHexagonMap.Loader.Contracts
{

    public delegate object CreateInstanceHandler(string component = null);


    public interface ILoaderService
    {
        Task<bool> Process(string path, string exportHandler);

    }
}
