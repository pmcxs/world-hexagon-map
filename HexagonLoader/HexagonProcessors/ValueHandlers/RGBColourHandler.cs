using System;
using System.Linq;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    //[Export("value_handler_rgb_colour", typeof(IValueHandler))]
    public class RGBColourHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            var r = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(0)]);
            var g = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(1)]);
            var b = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(2)]);

            return GetRGB(r, g, b);
        }

        private string GetRGB(int r, int g, int b)
        {
            return "#" + ComponentToHex(r) + ComponentToHex(g) + ComponentToHex(b);
        }

        private string ComponentToHex(int c)
        {
            var hex = c.ToString("X");
            return hex.Length == 1 ? "0" + hex : hex;
        }
    }
}