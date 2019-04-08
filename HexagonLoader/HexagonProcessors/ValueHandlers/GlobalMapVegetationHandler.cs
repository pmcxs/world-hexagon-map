using System;
using System.Linq;
using WorldHexagonMap.HexagonDataLoader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    //[Export("value_handler_globalmap_vegetation", typeof(IValueHandler))]
    public class GlobalMapVegetationHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            var result = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(0)]);

            if (result < 80 || result > 100) return 0;

            return 1;
        }
    }
}