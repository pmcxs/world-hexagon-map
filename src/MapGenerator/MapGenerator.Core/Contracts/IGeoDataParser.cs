using MapGenerator.Core.Common;
using System;
using System.Collections.Generic;

namespace MapGenerator.Core.Contracts
{

    public interface IGeoDataParser : IDisposable
    {
        IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData);
    }
}