using System.Collections.Generic;
using WorldHexagonMap.HexagonDataLoader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public interface IGeoDataParser
    {
        IEnumerable<GeoData> ParseGeodataFromSource(layersLoader sourceData, string filePath);
    }
}