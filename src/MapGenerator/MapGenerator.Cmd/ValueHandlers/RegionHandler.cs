using MapGenerator.Core.Common;
using MapGenerator.Core.Contracts;

namespace MapGenerator.Cmd.ValueHandlers
{
    public class RegionHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            return geoData.Values["name"];
        }
    }
}