using System;
using System.Linq;

namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers
{
    //[Export("value_handler_wikimedia_altitude", typeof(IValueHandler))]
    public class WikimediaAltitudeHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            var result = Math.Round( Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(0)]) * 100.0 / 243.0);

            if (result > 100) result = 100;

            if (result < 0) result = 0;

            return result;
        }
    }
}
