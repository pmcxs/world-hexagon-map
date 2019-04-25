using System;
using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{

    public interface IGeoDataParser : IDisposable
    {
        IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData, string filePath);
    }
}