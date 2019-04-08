using WorldHexagonMap.HexagonDataLoader;

namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}
