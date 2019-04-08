using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public interface IHexagonProcessorFactory
    {
        IHexagonProcessor GetInstance(DataType dataType);
    }
}