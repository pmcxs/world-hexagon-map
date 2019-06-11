using MapGenerator.Core.Common;
using MapGenerator.Core.Contracts;

namespace MapGenerator.Cmd.ValueHandlers
{
    public class CountryISOHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["iso_a2"];
        }
    }
}