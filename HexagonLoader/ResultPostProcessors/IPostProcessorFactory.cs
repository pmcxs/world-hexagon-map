namespace WorldHexagonMap.HexagonDataLoader.ResultPostProcessors
{
    public interface IPostProcessorFactory
    {
        IPostProcessor GetInstance(string name);
    }
}