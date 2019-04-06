
using System.Composition;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Implementation.ValueHandlers
{
    [Export("value_handler_country_iso", typeof(IValueHandler))]
    public class CountryISOHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["iso_a2"];
        }
    }
}
