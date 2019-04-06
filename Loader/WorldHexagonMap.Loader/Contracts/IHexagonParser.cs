using System.Collections.Generic;
using WorldHexagonMap.Loader.Domain;
using WorldHexagonMap.Loader.Domain.Delegates;

namespace WorldHexagonMap.Loader.Contracts
{
    public interface IHexagonParser
    {
        IEnumerable<HexagonLoaderResult> ProcessGeoData(GeoData geoData, IValueHandler valueHandler = null);
    }
}