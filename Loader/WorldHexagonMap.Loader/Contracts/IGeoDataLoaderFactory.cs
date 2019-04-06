using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IGeoDataLoaderFactory
    {
        IGeoDataLoader GetInstance(string source);

    }
}