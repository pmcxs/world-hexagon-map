
using System.Composition;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Implementation.ValueHandlers
{
    [Export("value_handler_region", typeof(IValueHandler))]
    public class RegionHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["name"];
        }
    }
}
