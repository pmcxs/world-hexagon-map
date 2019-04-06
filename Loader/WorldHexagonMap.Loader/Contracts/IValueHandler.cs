using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IValueHandler
    {
        object GetValue(GeoData geoData);
    }
}
