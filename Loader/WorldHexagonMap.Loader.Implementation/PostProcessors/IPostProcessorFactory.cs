using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.PostProcessors
{
    public interface IPostProcessorFactory
    {
        IPostProcessor GetInstance(string name);
    }
}