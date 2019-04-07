using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.HexagonParsers
{
    public interface IHexagonParserFactory
    {
        IHexagonParser GetInstance(DataType dataType);
    }
}