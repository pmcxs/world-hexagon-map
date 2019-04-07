using System.Collections.Generic;
using WorldHexagonMap.Loader.Domain;
using WorldHexagonMap.Loader.ValueHandlers;

namespace WorldHexagonMap.Loader.HexagonParsers
{
    public interface IHexagonParser
    {
        IEnumerable<HexagonLoaderResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null);
    }
}