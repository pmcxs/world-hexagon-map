
namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers
{
    //[Export("value_handler_region", typeof(IValueHandler))]
    public class RegionHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["name"];
        }
    }
}
