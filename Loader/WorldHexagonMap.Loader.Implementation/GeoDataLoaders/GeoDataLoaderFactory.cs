using System;
using System.IO;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.Loader.Implementation.GeoDataLoaders
{
    public class GeoDataLoaderFactory : IGeoDataLoaderFactory
    {
        public IGeoDataLoader GetInstance(string source)
        {
            string extension = Path.GetExtension(source);

            switch (extension)
            {
                case ".json":
                case ".geojson":
                    return new GeojsonLoader();
                case ".xyz":
                    return new XYZLoader();
                case ".tif":
                    return new GeoTiffLoader();
                case ".shp":
                    return new ShapefileLoader();
                default:
                    throw new NotImplementedException("Unknown extension: " + extension);
            }
        }
    }
}