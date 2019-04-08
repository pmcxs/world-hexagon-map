using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers
{
    public interface IHexagonProcessorFactory
    {
        IHexagonProcessor GetInstance(DataType dataType);
    }
}