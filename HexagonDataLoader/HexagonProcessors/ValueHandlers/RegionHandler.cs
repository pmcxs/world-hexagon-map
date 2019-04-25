using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    public class RegionHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["name"];
        }
    }
}