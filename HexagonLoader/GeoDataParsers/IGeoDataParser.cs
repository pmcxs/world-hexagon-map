using System.Collections.Generic;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public interface IGeoDataParser
    {
        IEnumerable<GeoData> ParseGeodataFromSource(layersLoader sourceData, string filePath);
    }
}