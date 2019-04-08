using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}