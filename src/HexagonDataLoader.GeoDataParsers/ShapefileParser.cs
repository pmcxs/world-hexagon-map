using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Contracts;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace HexagonDataLoader.GeoDataParsers
{
    public class ShapefileParser : IGeoDataParser
    {
        public IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData)
        {
            string filePath = sourceData.Source;

            var reader = new ShapefileDataReader(filePath, new GeometryFactory());

            while (reader.Read())
            {
                var geoData = ConvertReaderToGeoData(reader);

                if (sourceData.Filters == null || sourceData.Filters.Length == 0)
                    yield return geoData;
                else
                    foreach (var filter in sourceData.Filters)
                    {
                        if (!geoData.Values.ContainsKey(filter.Field))
                            throw new Exception($"Field {filter.Field} was not found on shapefile {filePath}");

                        if (Convert.ToString(geoData.Values[filter.Field]) == filter.Value)
                        {
                            yield return geoData;
                        }
                    }
            }
        }

        private static (PointXY[][], DataType) ConvertGeometryToPointXY(IGeometry geometry)
        {
            PointXY[][] points;

            switch (geometry.GeometryType)
            {
                case "LineString":
                    {
                        var lineString = geometry as ILineString;
                        if (lineString == null) throw new ArgumentException("Invalid ILineString instance");

                        var coordinates = lineString.Coordinates;
                        points = new PointXY[1][];
                        points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();

                        return (points, DataType.Path);
                    }
                case "Polygon":
                    {
                        var polygon = geometry as IPolygon;
                        if (polygon == null) throw new ArgumentException("Invalid IPolygon instance");

                        var coordinates = polygon.Coordinates;
                        points = new PointXY[1][];
                        points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();

                        return (points, DataType.Area);
                    }
                case "MultiPolygon":
                    {
                        var multiPolygon = geometry as IMultiPolygon;
                        if (multiPolygon == null) throw new ArgumentException("Invalid IMultiPolygon instance");

                        var numGeometries = multiPolygon.Geometries.Length;

                        points = new PointXY[numGeometries][];

                        for (var i = 0; i < numGeometries; i++)
                        {
                            var polygon = multiPolygon.Geometries[i] as IPolygon;
                            if (polygon == null) throw new ArgumentException("Invalid IPolygon instance");

                            var polygonCoordinates = polygon.Coordinates;
                            points[i] = polygonCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                        }

                        return (points, DataType.Area);
                    }
                case "MultiLineString":
                    {
                        var multiLineString = geometry as IMultiLineString;
                        if (multiLineString == null) throw new ArgumentException("Invalid IMultiLineString instance");

                        var numGeometries = multiLineString.Geometries.Length;

                        points = new PointXY[numGeometries][];

                        for (var i = 0; i < numGeometries; i++)
                        {
                            var lineString = multiLineString.Geometries[i] as ILineString;
                            if (lineString == null) throw new ArgumentException("Invalid ILineString instance");

                            var lineStringCoordinates = lineString.Coordinates;
                            points[i] = lineStringCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                        }

                        return (points, DataType.Path);
                    }
                default:
                    throw new NotSupportedException($"Unknown geometry type: {geometry.GeometryType}");
            }
        }

        private GeoData ConvertReaderToGeoData(ShapefileDataReader reader)
        {
            var geoData = new GeoData
            { Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase) };


            var (points, dataType) = ConvertGeometryToPointXY(reader.Geometry);

            geoData.Points = points;
            geoData.DataType = dataType;

            var header = reader.DbaseHeader;

            for (var i = 0; i < header.NumFields; i++)
            {
                var field = header.Fields[i];
                geoData.Values[field.Name] = reader.GetValue(i + 1);
            }

            return geoData;
        }

        public void Dispose()
        {
        }
    }
}