using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Contracts;

namespace HexagonDataLoader.Cmd.ValueHandlers
{
    public class CountryISOHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["iso_a2"];
        }
    }
}