namespace WorldHexagonMap.HexagonDataLoader.PostProcessors
{
    public interface IPostProcessorFactory
    {
        IPostProcessor GetInstance(string name);
    }
}