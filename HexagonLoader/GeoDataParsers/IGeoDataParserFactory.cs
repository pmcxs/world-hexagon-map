using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public interface IGeoDataParserFactory
    {
        IGeoDataParser GetInstance(string source);

    }
}