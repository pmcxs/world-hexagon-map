using System.Collections.Generic;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;
using WorldHexagonMap.HexagonDataLoader.HexagonProcessors.ValueHandlers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public interface IHexagonProcessor
    {
        IEnumerable<HexagonProcessorResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null);
    }
}