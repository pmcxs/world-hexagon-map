using System.Collections.Generic;
using WorldHexagonMap.HexagonDataLoader.Domain;
using WorldHexagonMap.Loader;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{

    public interface IGeoDataParser
    {
        IEnumerable<GeoData> ParseGeodataFromSource(layersLoader sourceData, string filePath);


    }
}