using System.Collections.Generic;
using System.IO;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Contracts
{

    public interface IGeoDataLoader
    {
        IEnumerable<GeoData> LoadGeodataFromSource(layersLoader sourceData, string filePath);


    }
}