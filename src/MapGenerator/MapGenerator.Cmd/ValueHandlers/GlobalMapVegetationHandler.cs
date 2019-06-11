using MapGenerator.Core.Common;
using MapGenerator.Core.Contracts;
using System;
using System.Linq;

namespace MapGenerator.Cmd.ValueHandlers
{
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