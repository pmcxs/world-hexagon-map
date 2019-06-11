using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeoAPI.Geometries;
using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Contracts;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace HexagonDataLoader.GeoDataParsers
{
    public class GeojsonParser : IGeoDataParser
    {
        public IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData)
        {
            string filePath = sourceData.Source;

            var jsonData = File.ReadAllText(filePath);

            var reader = new GeoJsonReader();

            var featureCollection = reader.Read<FeatureCollection>(jsonData);

            return featureCollection.Features.Select(ConvertFeatureToGeoData);
        }

        private GeoData ConvertFeatureToGeoData(IFeature feature)
        {
            var geoData = new GeoData
            { Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase) };

            switch (feature.Geometry.GeometryType)
            {
                case "LineString":
                    {
                        var lineString = feature.Geometry as ILineString;
                        if (lineString == null) throw new ArgumentException("Invalid ILineString instance");

                        var coordinates = lineString.Coordinates;
                        geoData.Points = new PointXY[1][];
                        geoData.Points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                        geoData.DataType = DataType.Path;
                        break;
                    }
                case "Polygon":
                    {
                        var polygon = feature.Geometry as IPolygon;
                        if (polygon == null) throw new ArgumentException("Invalid IPolygon instance");

                        var coordinates = polygon.Coordinates;
                        geoData.Points = new PointXY[1][];
                        geoData.Points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                        geoData.DataType = DataType.Area;
                        break;
                    }
                case "MultiPolygon":
                    {
                        var multiPolygon = feature.Geometry as IMultiPolygon;
                        if (multiPolygon == null) throw new ArgumentException("Invalid IMultiPolygon instance");

                        var numGeometries = multiPolygon.Geometries.Length;

                        geoData.Points = new PointXY[numGeometries][];
                        geoData.DataType = DataType.Area;

                        for (var i = 0; i < numGeometries; i++)
                        {
                            var polygon = multiPolygon.Geometries[i] as IPolygon;
                            if (polygon == null) throw new ArgumentException("Invalid IPolygon instance");

                            var polygonCoordinates = polygon.Coordinates;
                            geoData.Points[i] = polygonCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                        }

                        break;
                    }
                case "MultiLineString":
                    {
                        var multiLineString = feature.Geometry as IMultiLineString;
                        if (multiLineString == null) throw new ArgumentException("Invalid IMultiLineString instance");

                        var numGeometries = multiLineString.Geometries.Length;

                        geoData.Points = new PointXY[numGeometries][];
                        geoData.DataType = DataType.Path;

                        for (var i = 0; i < numGeometries; i++)
                        {
                            var lineString = multiLineString.Geometries[i] as ILineString;
                            if (lineString == null) throw new ArgumentException("Invalid ILineString instance");

                            var lineStringCoordinates = lineString.Coordinates;
                            geoData.Points[i] = lineStringCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                        }

                        break;
                    }
            }

            foreach (var attributeName in feature.Attributes.GetNames())
                geoData.Values[attributeName] = feature.Attributes[attributeName];

            return geoData;
        }

        public void Dispose()
        {
        }
    }
}