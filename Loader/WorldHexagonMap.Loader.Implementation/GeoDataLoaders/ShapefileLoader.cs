using System;
using System.Collections.Generic;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace WorldHexagonMap.Loader.Implementation.GeoDataLoaders
{
    public class ShapefileLoader : DataLoader, IGeoDataLoader
    {

        public IEnumerable<GeoData> LoadGeodataFromSource(layersLoader sourceData, string filePath)
        {
            var reader = new ShapefileDataReader(filePath, new GeometryFactory());

            while (reader.Read())
            {
                var geoData = ConvertReaderToGeoData(reader);

                if (sourceData.filters == null || sourceData.filters.Length == 0)
                {
                    yield return geoData;
                }
                else
                {
                    foreach (var filter in sourceData.filters)
                    {
                        if (!geoData.Values.ContainsKey(filter.field))
                        {
                            throw new Exception(string.Format("Field {0} was not found on shapefile {1}", filter.field, filePath));
                        }

                        if (Convert.ToString(geoData.Values[filter.field]) == filter.value)
                        {
                            yield return geoData;
                        }
                    }
                    
                }
                
            }
        }

        protected GeoData ConvertReaderToGeoData(ShapefileDataReader reader)
        {
            var geoData = new GeoData { Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase) };

            geoData.Points = ConvertGeometryToPointXY(reader.Geometry);

            DbaseFileHeader header = reader.DbaseHeader;

            for (var i=0; i < header.NumFields; i++)
            {
                var field = header.Fields[i];
                geoData.Values[field.Name] = reader.GetValue(i);
            }

            return geoData;
        }

        
    }
}
