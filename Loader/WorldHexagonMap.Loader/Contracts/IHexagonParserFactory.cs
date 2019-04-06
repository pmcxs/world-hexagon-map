using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IHexagonParserFactory
    {
        IHexagonParser GetInstance(DataType dataType);
    }
}