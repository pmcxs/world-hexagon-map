namespace WorldHexagonMap.Loader.PostProcessors
{
    public interface IPostProcessorFactory
    {
        IPostProcessor GetInstance(string name);
    }
}