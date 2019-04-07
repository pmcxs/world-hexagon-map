using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.ValueHandlers
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}
