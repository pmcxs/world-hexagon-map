﻿using System;
using System.Composition;
using System.Linq;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Implementation.ValueHandlers
{

    [Export("value_handler_rgb_colour", typeof(IValueHandler))]
    public class RGBColourHandler : IValueHandler
    {
        public object GetValue(GeoData geoData)
        {
            int r = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(0)]);
            int g = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(1)]);
            int b = Convert.ToInt32(geoData.Values[geoData.Values.Keys.ElementAt(2)]);

            return GetRGB(r, g, b);
        }

        private string GetRGB(int r, int g, int b)
        {
            return "#" + ComponentToHex(r) + ComponentToHex(g) + ComponentToHex(b);
        }

        private string ComponentToHex(int c)
        {
            string hex = c.ToString("X");
            return hex.Length == 1 ? "0" + hex : hex;
        }


    }
}