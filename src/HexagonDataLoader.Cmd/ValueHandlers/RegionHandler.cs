using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Contracts;

namespace HexagonDataLoader.Cmd.ValueHandlers
{
    public class RegionHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["name"];
        }
    }
}