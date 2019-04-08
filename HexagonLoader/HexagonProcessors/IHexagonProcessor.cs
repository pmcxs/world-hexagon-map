using System.Collections.Generic;
using WorldHexagonMap.HexagonDataLoader.Domain;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public interface IHexagonProcessor
    {
        IEnumerable<HexagonLoaderResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null);
    }
}