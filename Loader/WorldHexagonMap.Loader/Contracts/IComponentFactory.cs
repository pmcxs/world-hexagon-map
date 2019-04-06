namespace WorldHexagonMap.Loader.Contracts
{
    public interface IComponentFactory
    {
        T CreateInstance<T>(string contractName = "");
    }
}