using System.Collections.Generic;
using WorldHexagonMap.HexagonDataLoader.HexagonParsers.ValueHandlers;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonParsers
{
    public interface IHexagonProcessor
    {
        IEnumerable<HexagonLoaderResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null);
    }
}