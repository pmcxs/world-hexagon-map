using System;
using System.IO;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class GeoDataParserFactory : IGeoDataParserFactory
    {


        public IGeoDataParser GetInstance<T>() where T : IGeoDataParser
        {
            return Activator.CreateInstance<T>();
        }
        
        public IGeoDataParser GetInstance(string source)
        {
            var extension = Path.GetExtension(source);

            switch (extension)
            {
                case ".json":
                case ".geojson":
                    return new GeojsonParser();
                case ".xyz":
                    return new XYZParser();
                case ".tif":
                    return new GeoTiffParser();
                case ".shp":
                    return new ShapefileParser();
                default:
                    throw new NotSupportedException($"Unknown extension: {extension}");
            }
        }
    }
}