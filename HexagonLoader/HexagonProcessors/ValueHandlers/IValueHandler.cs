using WorldHexagonMap.HexagonDataLoader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}