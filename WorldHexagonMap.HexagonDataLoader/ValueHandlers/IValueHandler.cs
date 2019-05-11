using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.ValueHandlers
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}