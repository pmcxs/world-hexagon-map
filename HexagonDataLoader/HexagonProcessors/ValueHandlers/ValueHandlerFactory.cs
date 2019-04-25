using System;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers
{
    public class ValueHandlerFactory : IValueHandlerFactory
    {
        public IValueHandler GetInstance(string name)
        {
            //TODO: Plugable logic instead of having this hardcoded?
            switch (name)
            {
                case "value_handler_country_iso":
                    return new CountryISOHandler();
                case "value_handler_globalmap_vegetation":
                    return new GlobalMapVegetationHandler();
                case "value_handler_region":
                    return new RegionHandler();
                case "value_handler_rgb_colour":
                    return new RGBColourHandler();
                case "value_handler_wikimedia_altitude":
                    return new WikimediaAltitudeHandler();
                default:
                    throw new NotSupportedException($"Type {name} does not exist");
                
            }
        }
    }
}