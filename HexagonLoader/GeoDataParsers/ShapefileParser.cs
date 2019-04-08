using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class ShapefileParser : IGeoDataParser
    {

        protected PointXY[][] ConvertGeometryToPointXY(IGeometry geometry)
        {
            PointXY[][] points = null;

            if (geometry.GeometryType == "LineString")
            {
                var lineString = geometry as ILineString;
                if (lineString == null)
                {
                    throw new ArgumentException("Invalid ILineString instance");
                }

                var coordinates = lineString.Coordinates;
                points = new PointXY[1][];
                points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();

            }
            else if (geometry.GeometryType == "Polygon")
            {
                var polygon = geometry as IPolygon;
                if (polygon == null)
                {
                    throw new ArgumentException("Invalid IPolygon instance");
                }

                var coordinates = polygon.Coordinates;
                points = new PointXY[1][];
                points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();

            }
            else if (geometry.GeometryType == "MultiPolygon")
            {
                var multiPolygon = geometry as IMultiPolygon;
                if (multiPolygon == null)
                {
                    throw new ArgumentException("Invalid IMultiPolygon instance");
                }

                int numGeometries = multiPolygon.Geometries.Length;

                points = new PointXY[numGeometries][];

                for (var i = 0; i < numGeometries; i++)
                {
                    var polygon = multiPolygon.Geometries[i] as IPolygon;
                    if (polygon == null)
                    {
                        throw new ArgumentException("Invalid IPolygon instance");
                    }

                    var polygonCoordinates = polygon.Coordinates;
                    points[i] = polygonCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                }

            }
            else if (geometry.GeometryType == "MultiLineString")
            {
                var multiLineString = geometry as IMultiLineString;
                if (multiLineString == null)
                {
                    throw new ArgumentException("Invalid IMultiLineString instance");
                }

                int numGeometries = multiLineString.Geometries.Length;

                points = new PointXY[numGeometries][];

                for (var i = 0; i < numGeometries; i++)
                {
                    var lineString = multiLineString.Geometries[i] as ILineString;
                    if (lineString == null)
                    {
                        throw new ArgumentException("Invalid ILineString instance");
                    }

                    var lineStringCoordinates = lineString.Coordinates;
                    points[i] = lineStringCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                }
            }

            return points;
        }

        public IEnumerable<GeoData> ParseGeodataFromSource(layersLoader sourceData, string filePath)
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
