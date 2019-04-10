using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    public class CountryISOHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["iso_a2"];
        }
    }
}