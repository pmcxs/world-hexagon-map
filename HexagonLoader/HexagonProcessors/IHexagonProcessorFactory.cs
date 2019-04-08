using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public interface IHexagonProcessorFactory
    {
        IHexagonProcessor GetInstance(DataType dataType);
    }
}