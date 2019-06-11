using HexagonDataLoader.Core.Common;
using System;
using System.Collections.Generic;

namespace HexagonDataLoader.Core.Contracts
{

    public interface IGeoDataParser : IDisposable
    {
        IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData);
    }
}