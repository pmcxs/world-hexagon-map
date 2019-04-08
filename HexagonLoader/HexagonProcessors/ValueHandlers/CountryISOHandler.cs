using WorldHexagonMap.HexagonDataLoader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    //[Export("value_handler_country_iso", typeof(IValueHandler))]
    public class CountryISOHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["iso_a2"];
        }
    }
}