using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IHexagonLoader
    {
        void Process(layers layers, string basePath);
    }
}
